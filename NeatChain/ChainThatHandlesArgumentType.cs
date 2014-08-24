using System.Collections.Generic;

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
            return ThatAcceptsArgumentType<TArgument>.ToBeHandledBy.TheseHandlers(executionStrategy, receivers);
        }

        public static ChainFactory<TArgument>._Then SetUp<TArgument>(params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            return SetUp(ExecutionStrategy.AllHandlersFoundThatCanProcessTheArgumentAreExecuted, receivers);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument">Type of argument passed</typeparam>
        /// <typeparam name="TResult">Type of each result, whose list will be returned</typeparam>
        /// <param name="arg">actual argument of type TArgument</param>
        /// <param name="receivers">The handlers</param>
        /// <returns></returns>
        public static List<TResult> Execute<TArgument, TResult>(TArgument arg, params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            var chainSetUp = SetUp(ExecutionStrategy.AllHandlersFoundThatCanProcessTheArgumentAreExecuted, receivers);
            List<TResult> result;
            chainSetUp.ExecutionChainSucceeded(out result, arg);
            return result;
        }
    }
}