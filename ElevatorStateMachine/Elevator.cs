namespace ElevatorStateMachine;

public enum ElevatorStates
{
    IdleClosed,
    IdleOpened,
    OpeningDoors,
    ClosingDoors,
    Moving
}

public class Elevator
{
    private StateMachine<ElevatorStates> machine = new();

    public Elevator()
    {
        machine.OnStateChanged += CheckState;
    }

    public void CheckState(State state)
    {
        switch (state.Id)
        {
            case (int)ElevatorStates.IdleClosed:
                
                break;
        }
    }
}