using System.Collections.Generic;

namespace NeatChain
{
    public abstract class AChainMemberThatCanHandleArgumentType<TArgument>
    {
        protected AChainMemberThatCanHandleArgumentType<TArgument> NextReceiver { set; get; }

        public void SetNextReceiver(AChainMemberThatCanHandleArgumentType<TArgument> nextReceiver)
        {
            NextReceiver = nextReceiver;
        }

        protected long CallPosition { set; get; }

        protected abstract bool ExecutionCondition(List<TArgument> arg);

        protected abstract List<dynamic> Execute(List<TArgument> arg);

        protected List<dynamic> LastResponse { set; get; }

        public void ExecuteOnlyFirstMatchingHandlerInChain(out  List<dynamic> response, List<TArgument> arg)
        {
            if (ExecutionCondition(arg))
            {
                response = Execute(arg);
                return;
            }
            NextReceiver.ExecuteOnlyFirstMatchingHandlerInChain(out response, arg);
        }
        public void ExecuteAllMatchingHandlerInChain(out  List<dynamic> response, List<TArgument> arg)
        {
         
            if (ExecutionCondition(arg))
            {

                if (LastResponse == null)
                {
                    LastResponse=new List<dynamic>();
                }

                LastResponse.AddRange(Execute(arg));
            }
            NextReceiver.LastResponse = LastResponse;
            NextReceiver.ExecuteOnlyFirstMatchingHandlerInChain(out response, arg);
        }
    }
}