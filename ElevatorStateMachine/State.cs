namespace ElevatorStateMachine;

public class State
{
    public int Id { get; private set; }
    private SortedSet<StateTransition> Transition { get; set; } = new(Comparer<StateTransition>.Create((a, b) => a.Priority.CompareTo(b.Priority)));

    public event Action OnEnter;
    public event Action OnExit;

    public void Enter()
    {
        OnEnter?.Invoke();
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }

    public State(int id)
    {
        this.Id = id;
    }

    public StateTransition? EvaluateTransitionsWith<E>(StateMachine<E> stateMachine) where E : struct, Enum
    {
        foreach (var transiton in Transition)
        {
            if (transiton.CompareWith(stateMachine))
                return transiton;
        }

        return null;
    }
    

    public void AddTransition(StateTransition transition)
    {
        Transition.Add(transition);
    }
}