using System.Collections.Generic;
using System.Linq;

namespace NeatChain.Tests.TestHandlers
{
    public class Number3Handler : AChainMemberThatCanHandleArgumentType<int>
    {

        protected override bool ExecutionCondition(List<int> arg)
        {
            return (arg.ToList().FirstOrDefault() == 3);
        }

        protected override List<dynamic> Execute(List<int> arg)
        {
            return new List<dynamic> { arg.ToList().First() * 100 };
        }
    }
}