using System;
using System.Collections.Generic;

namespace NeatChainFx.Tests.TestHandlers
{
    public class Number3Handler : NeatChainHandler<int>
    {
        protected override List<Action<int, int>> SetValidations(NeatChainCondition chainCondition, List<Action<int, int>> validations)
        {
            NeatChain.@where(() =>
            {
                validations.Add((arg, index) => chainCondition.Requires(arg).IsNotNull());
                validations.Add((arg, index) => chainCondition.Requires(arg).IsAn<int>());
            });

            return validations;
        }

        protected override bool HasResponsibilityToExecute(int arg, List<int> args)
        {
            return (arg == 3);
        }

        protected override List<dynamic> Execute(int arg, List<int> args)
        {
            return new List<dynamic> { NeatChain.@where(() => arg * 100) };
        }
    }
}