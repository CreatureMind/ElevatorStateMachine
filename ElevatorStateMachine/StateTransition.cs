namespace ElevatorStateMachine;

public class StateTransition
{
    public State DestinationState;
    public int Priority;
    public StateTransitionConditionOpaque[] conditions;

    public bool CompareWith<E>(StateMachine<E> machine) where E : struct, Enum
    {
        foreach (var condition in conditions)
        {
            if (!condition.Compare(machine)) return false;
        }

        return true;
    }

    public StateTransition(State destinationState, int priority, params StateTransitionConditionOpaque[] conditions)
    {
        DestinationState = destinationState;
        Priority = priority;
        this.conditions = conditions;
    }

    public StateTransition(State destinationState, params StateTransitionConditionOpaque[] conditions) : this(destinationState, 0, conditions) { }
}