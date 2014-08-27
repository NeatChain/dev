using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeatChainFx.Tests.TestHandlers;

namespace NeatChainFx.Tests
{
    [TestClass]
    public class using_the_new_where_syntax
    {
        [TestMethod]
        public void it_should_inject_properly()
        {
            using (new CodeInjection())
            {
                var result = NeatChain.Build(3, new Number3Handler()).Execute<int>();

                Assert.IsTrue(result.Count == 1);
                // if no fakes are set up, then original values are used
                Assert.AreEqual(300, result.First());

                // if fakes are set up and EnableFakeExecutions is true, fakes are used
               
                NeatChain.Inject.@where(() => 5000);
                result = NeatChain.Build(3, new Number3Handler()).Execute<int>();
                Assert.AreEqual(5000, result.First());



                var number3HandlerInstance1 = new Number3Handler();
                var separateResult1 = number3HandlerInstance1.Execute(7);

                Assert.AreEqual(5000, separateResult1.First());
                Assert.IsTrue(Execution.ThrewException(() => number3HandlerInstance1.ValidateInputArguments(null)));
                Assert.IsFalse(Execution.ThrewException(() => number3HandlerInstance1.ValidateInputArguments(3)));



                NeatChain.Inject.@where(() =>{});

                var number3HandlerInstance = new Number3Handler();
                var separateResult = number3HandlerInstance.Execute(7);

                Assert.AreEqual(5000, separateResult.First());
                Assert.IsFalse(Execution.ThrewException(() => number3HandlerInstance.ValidateInputArguments(null)));
                Assert.IsFalse(Execution.ThrewException(() => number3HandlerInstance.ValidateInputArguments(3)));
            }


            var number3HandlerInstance2 = new Number3Handler();
            var separateResult2 = number3HandlerInstance2.Execute(7);

            Assert.AreEqual(700, separateResult2.First());
            Assert.IsTrue(Execution.ThrewException(() => number3HandlerInstance2.ValidateInputArguments(null)));
            Assert.IsFalse(Execution.ThrewException(() => number3HandlerInstance2.ValidateInputArguments(3)));
           

        }
    }
}