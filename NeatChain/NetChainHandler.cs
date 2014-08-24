using System;
using System.Collections.Generic;
using System.Linq;

namespace NeatChainFx
{
    public abstract class NetChainHandler<TArgument>
    {

        private List<dynamic> _Execute(TArgument arg,List<TArgument> args)
        {
            
            NeatChain.OnHandlerExecutionStartedEvent(new HandlerExecutionEventArgs()
            {
                CurrentTime = DateTime.UtcNow,
                HandlerType = GetType(),
                HandlerName = GetType().Name,
                  HandlerFullName = GetType().FullName
            });
            List<dynamic> result;
            try
            {
                 result = Execute(arg, args);
            }
            finally
            {
                NeatChain.OnHandlerExecutionEndedEvent(new HandlerExecutionEventArgs()
                {
                    CurrentTime = DateTime.UtcNow,
                    HandlerType = GetType(),
                    HandlerName = GetType().Name,
                    HandlerFullName = GetType().FullName
                });
            }


            return result;
        }


        #region Public API
        public List<dynamic> Execute(List<TArgument> args)
        {
            return _Execute(args.FirstOrDefault(), args);
        }

        public List<dynamic> Execute(TArgument arg)
        {
            return _Execute(arg, new List<TArgument>() { arg });
        }

        /// <summary>
        /// Public API
        /// </summary>
        /// <param name="args"></param>
        /// <param name="exceptionHandler"></param>
        public void ValidateInputArguments(List<TArgument> args, Action<string, Exception> exceptionHandler=null)
        {
            try
            {
                var index = 0;
                args.ForEach(
                    arg =>
                    {
                        SetValidations(new ChainCondition(), new List<Action<TArgument, int>>())
                            .ForEach(x => x.Invoke(arg, index));
                        index++;
                    });
            }
            catch (Exception e)
            {
                if (exceptionHandler == null)
                {
                    throw;
                }

                exceptionHandler.Invoke("An exception occured during a validation process",e);
            }

        }

        /// <summary>
        /// Public API
        /// </summary>
        /// <param name="args"></param>
        /// <param name="exceptionHandler"></param>
        public void ValidateInputArguments(TArgument args, Action<string, Exception> exceptionHandler = null)
        {
            ValidateInputArguments(new List<TArgument>() { args }, exceptionHandler);
        }

     
        /// <summary>
        /// Public API
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool HasResponsibilityToExecute( List<TArgument> args)
        {
            return HasResponsibilityToExecute(args.FirstOrDefault(), args);
        }
        /// <summary>
        /// Public API
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool HasResponsibilityToExecute(TArgument arg)
        {
            return HasResponsibilityToExecute(arg, new List<TArgument>(){arg});
        }

        #endregion

        


        protected NetChainHandler<TArgument> NextReceiver { set; get; }

        internal void SetNextReceiver(NetChainHandler<TArgument> nextReceiver)
        {
            NextReceiver = nextReceiver;
        }

        /// <summary>
        /// Provide a list of validations that will be called for each passedin argument
        /// </summary>
        /// <param name="chainCondition"></param>
        /// <param name="validations"></param>
        /// <returns></returns>
        protected abstract List<Action<TArgument, int>> SetValidations(ChainCondition chainCondition, List<Action<TArgument, int>> validations);

     
        protected long CallPosition { set; get; }

        protected abstract bool HasResponsibilityToExecute(TArgument firstArg, List<TArgument> args);

        protected abstract List<dynamic> Execute(TArgument firstArg, List<TArgument> args);

        protected List<dynamic> LastSetOfResponses { set; get; }


      

        internal void ExecuteOnlyFirstMatchingHandlerInChain(out  List<dynamic> responses, List<TArgument> args)
        {
            if (HasResponsibilityToExecute( args))
            {
                ValidateInputArguments(args);

                responses = Execute(args);
                return;
            }
            NextReceiver.ExecuteOnlyFirstMatchingHandlerInChain(out responses, args);
        }

        internal void ExecuteAllMatchingHandlerInChain(out  List<dynamic> response, List<TArgument> args)
        {
            if (HasResponsibilityToExecute( args))
            {
                ValidateInputArguments(args);

                if (LastSetOfResponses == null)
                {
                    LastSetOfResponses = new List<dynamic>();
                }

                LastSetOfResponses.AddRange(Execute(args));
            }
            NextReceiver.LastSetOfResponses = LastSetOfResponses;
            NextReceiver.ExecuteOnlyFirstMatchingHandlerInChain(out response, args);
        }
    }
}