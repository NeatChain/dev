namespace NeatChain
{
    public enum ExecutionStrategy
    {
        Unknown = 0,
        OnlyTheFirsHandlerFoundThatCanProcessTheArgumentIsExecuted,
        AllHandlersFoundThatCanProcessTheArgumentAreExecuted
    }
}