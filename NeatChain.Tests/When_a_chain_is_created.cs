using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NeatChain.Tests.TestHandlers;

namespace NeatChain.Tests
{
    [TestClass]
    public class When_a_chain_is_created
    {
        [TestMethod]
        public void and_execution_strategy_requires_OnlyFirstMatchingHandlerIsExecuted_it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed1()
        {
            var testArg = new List<int> { 1, 2, 3 };

            var chainSetUp = NeatChain.ThatAcceptsArgumentType<int>.ToBeHandledBy.AtMostOneOfTheseHandlers(
                new Number1Handler(),
                new Number2Handler(), 
                new Number3Handler());

            testArg.ForEach(arg =>
            {
                List< int> result;
                Assert.IsTrue(chainSetUp.ExecutionChainSucceeded(out result,arg));
                Assert.IsTrue( result.Count==1);
                Assert.AreEqual(arg * 100, result.First());
            });
        }

        //Short Hand Form

        [TestMethod]
        public void and_execution_strategy_requires_OnlyFirstMatchingHandlerIsExecuted_it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed2()
        {
            var testArg = new List<int> { 1, 2, 3 };

            var chainSetUp = NeatChain.SetUp(
                ExecutionStrategy.OnlyFirstMatchingHandlerIsExecuted,
                new Number1Handler(),
                new Number2Handler(),
                new Number3Handler());

            testArg.ForEach(arg =>
            {
                List<int> result;
                Assert.IsTrue(chainSetUp.ExecutionChainSucceeded(out result, arg));
                Assert.IsTrue(result.Count == 1);
                Assert.AreEqual(arg * 100, result.First());
            });
        }
    }
}