using System;
using System.Collections.Generic;

namespace NeatChainFx{
    public class DefaultReceiver<TArgument> : AChainMemberThatCanHandleArgumentType<TArgument>
    {
        protected override List<Action<TArgument, int>> GetValidationDefinitions(ItIsRequired itIsRequired)
        {
           return new List<Action<TArgument, int>>();
        }

       

        protected override bool ItHasTheResponsibility(TArgument arg, List<TArgument> args)
        {
            return NextReceiver == null;
        }

        protected override List<dynamic> Execute(TArgument firstArg, List<TArgument> arg)
        {
            throw new Exception("No objects found in the chain. Please plugin at least one object");
        }
    }
}