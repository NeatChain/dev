using System;
using System.Collections.Generic;

namespace NeatChainFx{
    public class DefaultReceiver<TArgument> : NetChainHandler<TArgument>
    {
        protected override List<Action<TArgument, int>> SetValidations(ChainCondition chainCondition, List<Action<TArgument, int>> validations)
        {
           return validations;
        }

       

        protected override bool HasResponsibilityToExecute(TArgument arg, List<TArgument> args)
        {
            return NextReceiver == null;
        }

        protected override List<dynamic> Execute(TArgument firstArg, List<TArgument> arg)
        {
            throw new Exception("No objects found in the chain. Please plugin at least one object");
        }
    }
}