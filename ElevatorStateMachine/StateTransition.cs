namespace ElevatorStateMachine;

public struct StateTransition
{
    public State DestinationState;
    public int Priority;
    public Func<bool> Condition;
}