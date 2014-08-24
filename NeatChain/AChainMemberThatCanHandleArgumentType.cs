using System;
using System.Collections.Generic;
using System.Linq;

namespace NeatChainFx
{
    public abstract class AChainMemberThatCanHandleArgumentType<TArgument>
    {
        protected AChainMemberThatCanHandleArgumentType<TArgument> NextReceiver { set; get; }

        public void SetNextReceiver(AChainMemberThatCanHandleArgumentType<TArgument> nextReceiver)
        {
            NextReceiver = nextReceiver;
        }

        /// <summary>
        /// Provide a list of validations that will be called for each passedin argument
        /// </summary>
        /// <param name="itIsRequired"></param>
        /// <returns></returns>
        protected abstract List<Action<TArgument, int>> GetValidationDefinitions(ItIsRequired itIsRequired);

        private void ValidateInputArguments(List<TArgument> args)
        {
            var index = 0;
            args.ForEach(
                arg =>
                {
                    GetValidationDefinitions(new ItIsRequired()).ForEach(x => x.Invoke(arg, index));
                    index++;
                });
        }

        protected long CallPosition { set; get; }

        protected abstract bool ItHasTheResponsibility(TArgument arg, List<TArgument> args);

        protected abstract List<dynamic> Execute(TArgument arg, List<TArgument> args);

        protected List<dynamic> LastSetOfResponses { set; get; }

        public void ExecuteOnlyFirstMatchingHandlerInChain(out  List<dynamic> responses, List<TArgument> args)
        {
            if (ItHasTheResponsibility(args.FirstOrDefault(), args))
            {
                ValidateInputArguments(args);

                responses = Execute(args.FirstOrDefault(), args);
                return;
            }
            NextReceiver.ExecuteOnlyFirstMatchingHandlerInChain(out responses, args);
        }

        public void ExecuteAllMatchingHandlerInChain(out  List<dynamic> response, List<TArgument> args)
        {
            if (ItHasTheResponsibility(args.FirstOrDefault(), args))
            {
                ValidateInputArguments(args);

                if (LastSetOfResponses == null)
                {
                    LastSetOfResponses = new List<dynamic>();
                }

                LastSetOfResponses.AddRange(Execute(args.FirstOrDefault(), args));
            }
            NextReceiver.LastSetOfResponses = LastSetOfResponses;
            NextReceiver.ExecuteOnlyFirstMatchingHandlerInChain(out response, args);
        }
    }
}