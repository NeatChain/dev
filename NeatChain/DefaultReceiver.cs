using System;
using System.Collections.Generic;

namespace NeatChain{
    public class DefaultReceiver<TArgument> : AChainMemberThatCanHandleArgumentType<TArgument>
    {
        protected override List<Action<TArgument, int>> GetValidationDefinitions(ItIsRequired itIsRequired)
        {
           return new List<Action<TArgument, int>>();
        }

        protected override bool IsAbleToProcessArguments(TArgument firstArg, List<TArgument> arg)
        {
            return NextReceiver == null;
        }

        protected override List<dynamic> Execute(TArgument firstArg, List<TArgument> arg)
        {
            throw new Exception("No objects found in the chain. Please plugin at least one object");
        }
    }
}