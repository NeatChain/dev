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
            return SetUp(ExecutionStrategy.AllHandlersFoundThatHaveTheResponsibilitiesAreExecuted, receivers);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument">Type of argument passed</typeparam>
        /// <typeparam name="TResult">Type of each result, whose list will be returned</typeparam>
        /// <param name="arg">actual argument of type TArgument</param>
        /// <param name="receivers">The handlers</param>
        /// <returns></returns>
        public static _Execute<TArgument> SetUpWithArgument<TArgument>(TArgument arg, params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            return new _Execute<TArgument>(arg,receivers);
        }

        public class _Execute<TArgument>
        {
            private TArgument Arg { set; get; }
            AChainMemberThatCanHandleArgumentType<TArgument>[] Receivers { set; get; }
            public _Execute(TArgument arg, params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
            {
                Arg = arg;
                Receivers = receivers;
            }

            public  List<TResult> Execute< TResult>()
            {
                var chainSetUp = SetUp(ExecutionStrategy.AllHandlersFoundThatHaveTheResponsibilitiesAreExecuted, Receivers);
                List<TResult> result;
                chainSetUp.ExecutionChainSucceeded(out result, Arg);
                return result;
            }
        }
    }
}