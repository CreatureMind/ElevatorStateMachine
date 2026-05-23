namespace ElevatorStateMachine;

public class StateMachine<T> where T : struct, Enum
{
    private readonly State[] _states;

    public event Action<State> OnStateChanged;

    private State _currentState;
    public State CurrentState
    {
        get => _currentState;
        private set
        {
            _currentState = value;

            OnStateChanged?.Invoke(_currentState);
        }
    }

    public StateMachine()
    {
        var enumValues = Enum.GetValues<T>();
        
        _states = new State[enumValues.Length];
        
        for (int i = 0; i < _states.Length; i++)
        {
            _states[i] = new State((int)(object)enumValues[i]);
        }
        
        if (_states.Length != 0)
            CurrentState = _states[0];
    }

    public State? GetState(T state)
    {
        foreach (var currState in _states)
        {
            if ((int)(object)state != currState.Id) continue;

            return currState;
        }

        return null;
    }

    public void SetState(T stateEnum)
    {
        var state = GetState(stateEnum);
        if (state == null) return;

        CurrentState = state;
    }


}