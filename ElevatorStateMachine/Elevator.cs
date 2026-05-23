using System.Reflection.PortableExecutable;

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

        machine.SetBool("isDoorOpen", true);

        machine.AddTransition(ElevatorStates.IdleClosed, ElevatorStates.OpeningDoors, new BoolCondition("isDoorOpen", true));
    }

    [StateMachineCallback(ElevatorStates.OpeningDoors, StateCallback.Enter)]
    public void OnOpeningDoorsStart()
    {
        Console.WriteLine("meow");
    }
}