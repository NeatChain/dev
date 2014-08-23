using System.Collections.Generic;
using System.Linq;

namespace NeatChain.Tests.TestHandlers
{
    public class Number2Handler : AChainMemberThatCanHandleArgumentType<int>
    {

        protected override bool ExecutionCondition(List<int> arg)
        {
            return (arg.ToList().FirstOrDefault() == 2);
        }

        protected override List<dynamic> Execute( List<int> arg)
        {
            return new List<dynamic> {arg.ToList().First()*100};
        }
    }
}