namespace NeatChain
{
    public static class NeatChain
    {
        public static class ThatAcceptsArgumentType<TArgument>
        {
            public static ChainFactory<TArgument> ToBeHandledBy = new ChainFactory<TArgument>();
        }

        public static ChainFactory<TArgument>._Then SetUp<TArgument>(ExecutionStrategy executionStrategy, params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            return ThatAcceptsArgumentType<TArgument>.ToBeHandledBy.TheseHandlers(executionStrategy,receivers);
        }
    }
}