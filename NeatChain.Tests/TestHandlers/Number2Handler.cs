using System;
using System.Collections.Generic;
using System.Linq;

namespace NeatChain.Tests.TestHandlers
{
    public class Number2Handler : AChainMemberThatCanHandleArgumentType<int>
    {
        protected override List<Action<int, int>> GetValidationDefinitions(ItIsRequired itIsRequired)
        {
            return new List<Action<int, int>>()
            {
                (arg, index) => itIsRequired.That(arg).IsNotNull(),
                (arg, index) => itIsRequired.That(arg).IsAn<int>()
            };
        }

        protected override bool IsAbleToProcessArguments(int arg, List<int> args)
        {
            return (arg == 2);
        }

        protected override List<dynamic> Execute(int arg, List<int> args)
        {
            return new List<dynamic> { arg * 100 };
        }
    }
}