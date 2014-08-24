﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeatChain.Tests.TestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeatChain.Tests
{
    [TestClass]
    public class When_a_chain_is_created
    {
        /// <summary>
        /// You can make things as explicit as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed1()
        {
            var testArg = new List<int> { 1, 2, 3 };
            var chainSetUp = NeatChain.ThatAcceptsArgumentType<int>.ToBeHandledBy.AtMostOneOfTheseHandlers(
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

        /// <summary>
        /// You can make things as simple as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed2()
        {
            var testArg = new List<int> { 1, 2, 3 };
            var chainSetUp = NeatChain.SetUp(
                ExecutionStrategy.OnlyTheFirsHandlerFoundThatCanProcessTheArgumentIsExecuted,
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

        /// <summary>
        /// You can make things as easy as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed3()
        {
            var chainSetUp = NeatChain.SetUp(new Number1Handler());

            List<int> result;
            Assert.IsTrue(chainSetUp.ExecutionChainSucceeded(out result, 1));
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(100, result.First());

            //though a simple case , there are benefits, such as
            //1. defensive A: you are provided with a neat opinionated pattern of input argument validation
            //2. defensive B: you are provided with the capability to determine if it is the responsibility of the execute method
            //    to process the argument, if not, argument will not even be validated and the execute method will not be called.
            //3. method execution still passes through the same pipeline as chain execution, thus same neat exception handling
        }

        /// <summary>
        /// You can make things as 'one liner' as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed4()
        {
            var result = NeatChain.Execute<int, int>(1, new Number1Handler());

            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(100, result.First());
        }

        /// <summary>
        /// Or you can make things as complicated as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed5()
        {
            const string exceptionMessageCreatedInTheConverter = "exceptionMessageCreatedInTheConverter";
            var exceptionWasRaised = false;
            var converterWasCalled = false;
            var testArg = new List<int> { 1, 2, 3 };
            var chainSetUp = NeatChain.SetUp(
                ExecutionStrategy.OnlyTheFirsHandlerFoundThatCanProcessTheArgumentIsExecuted,
                new Number1Handler(),
                new Number2Handler(),
                new Number3Handler());
            testArg.ForEach(arg =>
            {
                List<int> result;
                Assert.IsTrue(chainSetUp.ExecutionChainSucceeded(out result, arg, (message, e) =>
                {
                    // if an exception occures, here will be used as the catch block
                    exceptionWasRaised = true;
                    Assert.AreEqual(exceptionMessageCreatedInTheConverter, e.Message);
                }, (objectsReturned) =>
                {
                    converterWasCalled = true;
                    // the assumption is that each handler may not return the type you want,
                    // e.g a web service call that returns json or xml
                    // usally dyanmic data is returned from each handler, this provides
                    // the opportunity to provide a custom converter to the expected type
                    // else a direct cast will be used
                    var tmpResult = new List<int>();
                    objectsReturned.ForEach(x => tmpResult.Add((int)x));

                    Assert.IsTrue(tmpResult.Count == 1);
                    Assert.AreEqual(arg * 100, tmpResult.First());
                    //lets throw an exception here to simulate something going wrong in a given handler
                    throw new Exception(exceptionMessageCreatedInTheConverter);
                    return tmpResult;
                }));
                Assert.IsTrue(result.Count == 0);
                Assert.IsTrue(exceptionWasRaised);
                Assert.IsTrue(converterWasCalled);
            });
        }
    }
}