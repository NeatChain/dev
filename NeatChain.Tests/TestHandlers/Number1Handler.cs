using System;
using System.Collections.Generic;

namespace NeatChain.Tests.TestHandlers
{
    public class Number1Handler : AChainMemberThatCanHandleArgumentType<int>
    {
        protected override List<Action<int, int>> GetValidationDefinitions(ItIsRequired itIsRequired)
        {
            return new List<Action<int, int>>
            {
                (arg, index) => itIsRequired.That(arg).IsNotNull(),
                (arg, index) => itIsRequired.That(arg).IsAn<int>()
            };
        }

        protected override bool ItHasTheResponsibility(int arg, List<int> args)
        {
            return (arg == 1);
        }

        protected override List<dynamic> Execute(int arg, List<int> args)
        {
            return new List<dynamic> {arg*100};
        }
    }
}