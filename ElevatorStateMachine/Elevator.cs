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
    private StateMachine<ElevatorStates> machine;
    
    public Elevator()
    {
        machine = new(this);

        machine.SetBool("wasCalled", false);
        machine.AddTransition(ElevatorStates.IdleClosed, ElevatorStates.IdleOpened);
    }

    [StateMachineCallback(ElevatorStates.IdleClosed, StateCallback.Enter)]
    public void EnterYaFr()
    {
        Console.WriteLine("meow");
    }

    [StateMachineCallback(ElevatorStates.IdleClosed, StateCallback.Exit)]
    public void EnterYaFrExit()
    {
        Console.WriteLine("exited closed");
    }

    [StateMachineCallback(ElevatorStates.IdleOpened, StateCallback.Enter)]
    public void OpenYa()
    {
        Console.WriteLine("openya");
    }
}