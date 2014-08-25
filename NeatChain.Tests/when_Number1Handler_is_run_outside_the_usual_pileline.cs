using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeatChainFx.Tests.TestHandlers;
using System.Collections.Generic;
using System.Linq;

namespace NeatChainFx.Tests
{
    public class InjectExecuteLabelTest : InjectExecuteAndReturnType<int> { }

    [TestClass]
    public class when_Number1Handler_is_run_outside_the_usual_pileline
    {
        [TestMethod]
        public void it_should_only_be_able_to_handle_arguments_that_has_the_value_of_one()
        {
            var inputIsValid = true;
            var number1Handler = new Number1Handler();
            number1Handler.ValidateInputArguments(1, (message, e) => { inputIsValid = false; });
            Assert.IsTrue(inputIsValid);
            Assert.IsTrue(number1Handler.HasResponsibilityToExecute(1));
            Assert.AreEqual(100, number1Handler.Execute(1).FirstOrDefault());
        }

        [TestMethod]
        public void it_should_not_not_be_responsible_to_handle_values_other_than_one()
        {
            var argList = new List<int> { 2, 3, -1 };

            argList.ForEach(arg =>
            {
                var inputIsValid = true;
                var number1Handler = new Number1Handler();
                number1Handler.ValidateInputArguments(arg, (message, e) => { inputIsValid = false; });
                //input is valid quite alright
                Assert.IsTrue(inputIsValid);
                //but its not its own responsibility to handle it
                Assert.IsFalse(number1Handler.HasResponsibilityToExecute(arg));
                //even though theoratically it can still process it, but in the pipeline, this will not be alloed to happen
                Assert.AreEqual(100 * arg, number1Handler.Execute(arg).FirstOrDefault());
            });
        }

        [TestMethod]
        public void injectExecute_should_inject_a_new_execution_at_point_of_call()
        {
            NeatChain.EnableFakeExecutions = true;
            //look for anywhere the label 'InjectExecuteLabelTest' is and fake out the execution
            NeatChain.SetFake.InjectExecute<InjectExecuteLabelTest, int>(() => 5000);
            var inputIsValid = true;
            var number1Handler = new Number1Handler();
            number1Handler.ValidateInputArguments(1, (message, e) => { inputIsValid = false; });
            Assert.IsTrue(inputIsValid);
            Assert.IsTrue(number1Handler.HasResponsibilityToExecute(1));
            var result = number1Handler.Execute(1).FirstOrDefault();
            // so instead of 100, the result is 5000
            Assert.AreEqual(5000, result);

            // we can overwrite fake
            NeatChain.SetFake.InjectExecute<InjectExecuteLabelTest, int>(() => 888);

            result = number1Handler.Execute(1).FirstOrDefault();
            // so instead of 5000, the result is 888
            Assert.AreEqual(888, result);

            //and if we disable faking, we get original value
            NeatChain.EnableFakeExecutions = false;
            result = number1Handler.Execute(1).FirstOrDefault();
            // so instead of 888, the result is back to 100
            Assert.AreEqual(100, result);
        }
    }
}