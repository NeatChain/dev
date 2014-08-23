using System;
using System.Collections.Generic;

namespace NeatChain{
    public class DefaultReceiver<TArgument> : AChainMemberThatCanHandleArgumentType<TArgument>
    {
       

        protected override bool ExecutionCondition(List<TArgument> arg)
        {
            return NextReceiver == null;
        }

        protected override List<dynamic> Execute( List<TArgument> arg)
        {
            throw new Exception("No objects found in the chain. Please plugin at least one object");
        }
    }
}