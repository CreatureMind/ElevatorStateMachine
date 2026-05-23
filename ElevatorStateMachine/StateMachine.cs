using System.Reflection;

namespace ElevatorStateMachine;

public class Trigger
{
    public bool Active { get; private set; } = false;

    public void Activate() => Active = true;
    public void Reset() => Active = false;

    public Trigger(bool startActive) => Active = startActive;
}

public class StateMachine<E> where E : struct, Enum
{
    private readonly State[] _states;

    public event Action<State> OnStateChanged;

    private Dictionary<string, object> _parameters = new();

    private State _currentState;
    public State CurrentState
    {
        get => _currentState;
        private set
        {
            _currentState?.Exit();
            _currentState = value;
            _currentState?.Enter();

            OnStateChanged?.Invoke(_currentState);

            CheckTransitions();
        }
    }
    
    public StateMachine(object callbacksBinder)
    {
        var enumValues = Enum.GetValues<E>();
        
        _states = new State[enumValues.Length];
        
        for (int i = 0; i < _states.Length; i++)
        {
            _states[i] = new State((int)(object)enumValues[i]);
        }

        BindCallbacks(callbacksBinder);

        if (_states.Length != 0)
            CurrentState = _states[0];
    }

    public State? GetState(E state)
    {
        foreach (var currState in _states)
        {
            if ((int)(object)state != currState.Id) continue;

            return currState;
        }

        return null;
    }
    
    public void SetState(E stateEnum)
    {
        var state = GetState(stateEnum);
        if (state == null) return;

        CurrentState = state;
    }

    // can be way more efficent with source generators but this is good enough for now ya
    public void BindCallbacks(object target)
    {
        var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();

            if (parameters.Length > 1) continue;

            if (parameters.Length == 1 && !parameters[0].ParameterType.Equals(typeof(StateMachine<E>))) continue;

            var attributes = method.GetCustomAttributes(typeof(StateMachineCallbackAttribute), true);
        
            foreach (StateMachineCallbackAttribute attr in attributes)
            {
                if (attr.State is not E stateEnum) continue;
                   
                var state = GetState(stateEnum);
                if (state == null) continue;

                Action action;
                if (parameters.Length == 1)
                {
                    action = () =>
                    {
                        ((Action<StateMachine<E>>)Delegate.CreateDelegate(typeof(Action<StateMachine<E>>), target, method)).Invoke(this);
                    };
                }
                else
                {
                    action = (Action)Delegate.CreateDelegate(typeof(Action), target, method);
                }

                if (attr.CallbackType == StateCallback.Enter)
                {
                    state.OnEnter += action;
                }
                else if (attr.CallbackType == StateCallback.Exit)
                {
                    state.OnExit += action;
                }
            }
        }
    }

    public bool TryGetParameter<T>(string name, out T param)
    {
        param = default;
        if (!_parameters.TryGetValue(name, out var paramO)) return false;

        if (paramO is not T paramVal) return false;

        param = paramVal;

        return true;
    }

    public bool TryGetBool(string name, out bool value) => TryGetParameter(name, out value);
    public bool TryGetInt(string name, out int value) => TryGetParameter(name, out value);
    public bool TryGetFloat(string name, out float value) => TryGetParameter(name, out value);
    public bool TryGetTrigger(string name, out Trigger value) => TryGetParameter(name, out value);

    public void SetParameter<T>(string name, T value)
    {
        if (_parameters.ContainsKey(name))
        {
            if (_parameters[name] is not T) return;

            _parameters[name] = value;
            OnParamChanged();
            return;
        }

        _parameters.Add(name, value);
        OnParamChanged();
    }

    public void SetBool(string name, bool value) => SetParameter(name, value);
    public void SetInt(string name, int value) => SetParameter(name, value);
    public void SetFloat(string name, float value) => SetParameter(name, value);
    public void RunTrigger(string name)
    {
        Trigger current;

        if (TryGetParameter(name, out current))
        {
            current.Activate();
            OnParamChanged();
        }
        else
        {
            current = new Trigger(true);
            SetParameter(name, current);
        }

        current.Reset();
    }

    private void OnParamChanged()
    {
        CheckTransitions();
    }

    private void CheckTransitions(int iterations = 0)
    {
        var toTransitionTo = CurrentState.EvaluateTransitionsWith(this);
        if (toTransitionTo == null) return;

        if (iterations >= 15)
        {
            Console.WriteLine($"WARNING!! Detected potential infinit condition loop in StateMachine<{typeof(E).Name}>!!\n atempted to go from state {CurrentState.Id} to state {toTransitionTo.DestinationState.Id}.");
            return;
        }

        CurrentState = toTransitionTo.DestinationState;

        CheckTransitions(iterations + 1);
    }

    public void AddTransition(E from, E to, int priority, params StateTransitionConditionOpaque[] conditions)
    {
        var fromState = GetState(from);
        var toState = GetState(to);
        if (fromState == null || toState == null) return;
        fromState.AddTransition(new StateTransition(toState, priority, conditions));

        CheckTransitions();
    }

    public void AddTransition(E from, E to, params StateTransitionConditionOpaque[] conditions)
    {
        AddTransition(from, to, 0, conditions);
    }
}