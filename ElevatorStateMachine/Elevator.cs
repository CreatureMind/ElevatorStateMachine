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
    private int currentFloor = 1;
    private int targetFloor = -1;

    public Elevator()
    {
        machine = new(this);

        // Initialize parameters before adding transitions
        machine.SetInt("targetFloor", -1);

        // Transitions from IdleClosed
        machine.AddTransition(ElevatorStates.IdleClosed, ElevatorStates.OpeningDoors, new TriggerCondition("openDoors"));
        machine.AddTransition(ElevatorStates.IdleClosed, ElevatorStates.Moving, new IntCondition("targetFloor", IntCondition.CompareType.NotEqual, -1));

        // Transitions from IdleOpened
        machine.AddTransition(ElevatorStates.IdleOpened, ElevatorStates.ClosingDoors, new TriggerCondition("closeDoors"));

        // Transitions from OpeningDoors
        machine.AddTransition(ElevatorStates.OpeningDoors, ElevatorStates.IdleOpened, new TriggerCondition("doorsFullyOpen"));

        // Transitions from ClosingDoors
        machine.AddTransition(ElevatorStates.ClosingDoors, ElevatorStates.IdleClosed, new TriggerCondition("doorsFullyClosed"));

        // Transitions from Moving
        machine.AddTransition(ElevatorStates.Moving, ElevatorStates.IdleClosed, new TriggerCondition("arrivedAtFloor"));

        machine.SetState(ElevatorStates.IdleClosed);
    }

    public void OpenDoors()
    {
        machine.RunTrigger("openDoors");
    }

    public void CloseDoors()
    {
        machine.RunTrigger("closeDoors");
    }

    public void GoToFloor(int floor)
    {
        if (floor == currentFloor)
        {
            Console.WriteLine($"Already at floor {floor}");
            return;
        }

        targetFloor = floor;
        machine.SetInt("targetFloor", floor);
    }

    [StateMachineCallback(ElevatorStates.OpeningDoors, StateCallback.Enter)]
    public void OnOpeningDoorsStart()
    {
        Console.WriteLine("Opening doors...");
        Thread.Sleep(1000);
        machine.RunTrigger("doorsFullyOpen");
    }

    [StateMachineCallback(ElevatorStates.IdleOpened, StateCallback.Enter)]
    public void OnIdleOpened()
    {
        Console.WriteLine("Doors are open. Elevator is idle.");
    }

    [StateMachineCallback(ElevatorStates.ClosingDoors, StateCallback.Enter)]
    public void OnClosingDoorsStart()
    {
        Console.WriteLine("Closing doors...");
        Thread.Sleep(1000);
        machine.RunTrigger("doorsFullyClosed");
    }

    [StateMachineCallback(ElevatorStates.IdleClosed, StateCallback.Enter)]
    public void OnIdleClosed()
    {
        Console.WriteLine($"Doors are closed. Elevator is idle on floor {currentFloor}.");
    }

    [StateMachineCallback(ElevatorStates.Moving, StateCallback.Enter)]
    public void OnMovingStart()
    {
        Console.WriteLine($"Moving from floor {currentFloor} to floor {targetFloor}...");

        int direction = targetFloor > currentFloor ? 1 : -1;
        while (currentFloor != targetFloor)
        {
            Thread.Sleep(1500);
            currentFloor += direction;
            Console.WriteLine($"Passing floor {currentFloor}...");
        }

        Console.WriteLine($"Arrived at floor {targetFloor}");
        machine.SetInt("targetFloor", -1);
        machine.RunTrigger("arrivedAtFloor");
    }
}