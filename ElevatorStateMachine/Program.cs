using ElevatorStateMachine;

Console.WriteLine("=== Elevator Simulation ===");
Console.WriteLine("Commands: open, close, goto <floor>, exit");
Console.WriteLine("Example: 'goto 5' moves to floor 5");
Console.WriteLine();

Elevator elevator = new Elevator();

while (true)
{
    Console.WriteLine("\nEnter command:");
    string command = Console.ReadLine() ?? "";

    if (command == "open")
    {
        elevator.OpenDoors();
    }
    else if (command == "close")
    {
        elevator.CloseDoors();
    }
    else if (command.StartsWith("goto "))
    {
        if (int.TryParse(command.Substring(5), out int floor))
        {
            elevator.GoToFloor(floor);
        }
        else
        {
            Console.WriteLine("Invalid floor number. Use: goto <floor>");
        }
    }
    else if (command == "exit")
    {
        Console.WriteLine("Exiting simulation...");
        break;
    }
    else
    {
        Console.WriteLine("Unknown command. Available: open, close, goto <floor>, exit");
    }
}

