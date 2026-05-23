namespace ElevatorStateMachine;

public class State
{
    public int Id { get; private set; }
    public List<State> NextStates { get; set; } = [];
    public StateTransition Transition { get; set; }

    public State(int id)
    {
        this.Id = id;
    }
    
}