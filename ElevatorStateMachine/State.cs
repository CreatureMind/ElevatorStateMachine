namespace ElevatorStateMachine;

public class State
{
    public int Id { get; private set; }
    private SortedSet<StateTransition> Transition { get; set; } = new(Comparer<StateTransition>.Create((a, b) =>
    {
        int priorityComparison = a.Priority.CompareTo(b.Priority);
        if (priorityComparison != 0) return priorityComparison;
        // If priorities are equal, compare by destination state ID to avoid duplicates being ignored
        return a.DestinationState.Id.CompareTo(b.DestinationState.Id);
    }));

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