using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeatChainFx.Tests.TestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeatChainFx.Tests
{
    [TestClass]
    public class When_a_chain_is_created
    {
        /// <summary>
        ///     You can make things as explicit as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed1()
        {
            var testArg = new List<int> { 1, 2, 3 };
            var chainSetUp = NeatChain.ThatAcceptsArgumentType<int>.ToBeHandledBy
                .AtMostOneOfTheseHandlers(
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
        ///     You can make things as simple as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed2()
        {
            var testArg = new List<int> { 1, 2, 3 };
            var chainSetUp = NeatChain.Build(
                ExecutionStrategy.NotMoreThanOneHandler,
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
        ///     You can make things as 'one liner' as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed4()
        {
            var result = NeatChain.Build(1, new Number1Handler()).Execute<int>();

            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(100, result.First());
        }

        /// <summary>
        ///     You can make things as 'one liner' as this
        /// </summary>
        [TestMethod]
        public void use_of_fakes1()
        {
            using (new CodeInjection())
            {
                var result = NeatChain.Build(1, new Number1Handler()).Execute<int>();

                Assert.IsTrue(result.Count == 1);
                // if no fakes are set up, then original values are used
                Assert.AreEqual(100, result.First());

                // if fakes are set up and EnableFakeExecutions is true, fakes are used
                NeatChain.Inject.CodeAt<SampleTestCode, int>(() => 5000);

                result = NeatChain.Build(1, new Number1Handler()).Execute<int>();
                Assert.AreEqual(5000, result.First());
            }
        }
        [TestMethod]
        public void use_of_fakes2_strongly_typed_code_removal_without_conditional_compilation()
        {
            using (new CodeInjection())
            {
                var result = NeatChain.Build(2, new Number2Handler()).Execute<int>();

                Assert.IsTrue(result.Count == 1);
                // if no fakes are set up, then original values are used
                Assert.AreEqual(200, result.First());

                // if fakes are set up and EnableFakeExecutions is true, fakes are used
                // it will execute as though the code was never written
                //GlobalLabel is an internally defined label
                NeatChain.Inject.EmptyCodeAt<DefaultCodeThatReturnsVoid>();

                result = NeatChain.Build(2, new Number2Handler()).Execute<int>();
                Assert.AreEqual(0, result.First());
            }
        }
        /// <summary>
        ///     Or you can make things as complicated as this
        /// </summary>
        [TestMethod]
        public void it_should_execute_only_the_member_of_the_chain_that_can_handle_the_argument_passed5()
        {
            // you can listen for all events
            NeatChain.OnHandlerExecutionStarted += NeatChain_OnHandlerExecutionStarted;
            NeatChain.OnHandlerExecutionEnded += NeatChain_OnHandlerExecutionEnded;

            var timesNumber1HandlerStarted = 0;
            var timesNumber1HandlerEnded = 0;
            var timesNumber2HandlerStarted = 0;
            var timesNumber2HandlerEnded = 0;
            var timesNumber3HandlerStarted = 0;
            var timesNumber3HandlerEnded = 0;

            // or for only events relating to a specific handler
            NeatChain.SetHandlerExecutionNotification<Number1Handler>((e) =>
            {
                timesNumber1HandlerStarted++;
            }, (e) =>
            {
                timesNumber1HandlerEnded++;
            });

            NeatChain.SetHandlerExecutionNotification<Number2Handler>((e) =>
            {
                timesNumber2HandlerStarted++;
            }, (e) =>
            {
                timesNumber2HandlerEnded++;
            });

            NeatChain.SetHandlerExecutionNotification<Number2Handler>((e) =>
            {
                timesNumber3HandlerStarted++;
            }, (e) =>
            {
                timesNumber3HandlerEnded++;
            });

            const string exceptionMessageCreatedInTheConverter = "exceptionMessageCreatedInTheConverter";
            var exceptionWasRaised = false;
            var converterWasCalled = false;
            var testArg = new List<int> { 1, 2, 3 };
            var chainSetUp = NeatChain.Build(
                ExecutionStrategy.NotMoreThanOneHandler,
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
                }, objectsReturned =>
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

            Assert.IsTrue(timesNumber1HandlerEnded == 1);
            Assert.IsTrue(timesNumber1HandlerStarted == 1);

            Assert.IsTrue(timesNumber2HandlerEnded == 1);
            Assert.IsTrue(timesNumber2HandlerStarted == 1);

            Assert.IsTrue(timesNumber3HandlerEnded == 1);
            Assert.IsTrue(timesNumber3HandlerStarted == 1);
        }

        private void NeatChain_OnHandlerExecutionEnded(object sender, HandlerExecutionEventArgs e)
        {
        }

        private void NeatChain_OnHandlerExecutionStarted(object sender, HandlerExecutionEventArgs e)
        {
        }
    }
}