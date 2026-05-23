namespace ElevatorStateMachine;

public abstract class StateTransitionConditionOpaque
{
    protected string ParamName { get; private set; }

    public abstract bool Compare<E>(StateMachine<E> machine) where E : struct, Enum;

    public StateTransitionConditionOpaque(string paramName) => this.ParamName = paramName;
}

public abstract class StateTransitionCondition<T> : StateTransitionConditionOpaque
{
    private T expectedValue;

    public override bool Compare<E>(StateMachine<E> machine) => CompareExact(machine, expectedValue);

    protected abstract bool CompareExact<E>(StateMachine<E> machine, T expectedValue) where E : struct, Enum;

    public StateTransitionCondition(string paramName, T expectedValue) : base(paramName) => this.expectedValue = expectedValue;
}

public class BoolCondition : StateTransitionCondition<bool>
{
    public BoolCondition(string paramName, bool expectedValue) : base(paramName, expectedValue) { }

    protected override bool CompareExact<E>(StateMachine<E> machine, bool expectedValue)
    {
        if (machine.TryGetParameter(ParamName, out bool value))
            return value == expectedValue;

        return false;
    }
}

public class TriggerCondition : StateTransitionConditionOpaque
{
    public TriggerCondition(string paramName) : base(paramName) { }
    public override bool Compare<E>(StateMachine<E> machine)
    {
        return machine.TryGetParameter(ParamName, out Trigger trigger) && trigger.Active;
    }
}

public class IntCondition : StateTransitionCondition<int>
{
    public enum CompareType { Equal, NotEqual, Greater, Less, GreaterOrEqual, LessOrEqual }

    private CompareType compare;

    public IntCondition(string paramName, CompareType compare, int expectedValue) : base(paramName, expectedValue) => this.compare = compare;
    protected override bool CompareExact<E>(StateMachine<E> machine, int expectedValue)
    {
        if (!machine.TryGetParameter(ParamName, out int value))
            return false;

        switch (compare)
        {
            case CompareType.Equal:
                return value == expectedValue;
            case CompareType.NotEqual:
                return value != expectedValue;
            case CompareType.Greater:
                return value > expectedValue;
            case CompareType.Less:
                return value < expectedValue;
            case CompareType.GreaterOrEqual:
                return value >= expectedValue;
            case CompareType.LessOrEqual:
                return value <= expectedValue;
            default:
                return false;
        }
    }
}

public class FloatCondition : StateTransitionCondition<float>
{
    public enum CompareType { Greater, Less, GreaterOrEqual, LessOrEqual }

    private CompareType compare;

    public FloatCondition(string paramName, CompareType compare, float expectedValue) : base(paramName, expectedValue) => this.compare = compare;
    protected override bool CompareExact<E>(StateMachine<E> machine, float expectedValue)
    {
        if (!machine.TryGetParameter(ParamName, out float value))
            return false;

        switch (compare)
        {
            case CompareType.Greater:
                return value > expectedValue;
            case CompareType.Less:
                return value < expectedValue;
            case CompareType.GreaterOrEqual:
                return value >= expectedValue;
            case CompareType.LessOrEqual:
                return value <= expectedValue;
            default:
                return false;
        }
    }
}

public class CustomCondition<T> : StateTransitionConditionOpaque
{
    private Func<T, bool> conditionFunc;
    public CustomCondition(string paramName, Func<T, bool> conditionFunc) : base(paramName) => this.conditionFunc = conditionFunc;
    public override bool Compare<E>(StateMachine<E> machine)
    {
        if (!machine.TryGetParameter(ParamName, out T val))
            return false;

        return conditionFunc(val);
    }
}