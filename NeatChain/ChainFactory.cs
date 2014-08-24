using System;
using System.Collections.Generic;
using System.Linq;

namespace NeatChainFx
{
    public class ChainFactory<TArgument>
    {
        public ChainFactory()
        {
            Init();
        }

        private List<AChainMemberThatCanHandleArgumentType<TArgument>> ExistingReceivers { set; get; }

        private void Init()
        {
            ExistingReceivers = new List<AChainMemberThatCanHandleArgumentType<TArgument>>
            {
                new DefaultReceiver<TArgument>()
            };
        }

        private _Then CreateChainReceivers(
            params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            Init();
            receivers.ToList().ForEach(x =>
            {
                ExistingReceivers[ExistingReceivers.Count - 1].SetNextReceiver(x);
                ExistingReceivers.Add(x);
            });

            return new _Then(this);
        }

        public ExecutionStrategy ExecutionStrategy { set; get; }

        public _Then TheseHandlers( ExecutionStrategy executionStrategy,
            params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            ExecutionStrategy = executionStrategy;
            return CreateChainReceivers(receivers);
        }


        public _Then AtMostOneOfTheseHandlers(
            params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            
            return TheseHandlers(ExecutionStrategy.OnlyTheFirsHandlerFoundWhoHasTheResponsibilityIsExecuted,receivers);
        }

        public _Then AtMostAllOfTheseHandlers(
           params AChainMemberThatCanHandleArgumentType<TArgument>[] receivers)
        {
            return TheseHandlers(ExecutionStrategy.AllHandlersFoundThatHaveTheResponsibilitiesAreExecuted, receivers);
        }

        public class _Then
        {
            private ChainFactory<TArgument> Parent { set; get; }

            public _Then(ChainFactory<TArgument> parent)
            {
                if (parent == null) throw new ArgumentNullException("parent");
                Parent = parent;
            }
            public bool ExecutionChainSucceeded<TResponse>(out  List<TResponse> response, params TArgument[] arg  )
            {
                return ExecutionChainSucceeded(out response, arg.ToList());
            }

            public bool ExecutionChainSucceeded<TResponse>(out  List<TResponse> response ,  Action<string,Exception> ExceptionHandler=null, params TArgument[] arg )
            {
                return ExecutionChainSucceeded(out response, arg.ToList(),ExceptionHandler);
            }

            public bool ExecutionChainSucceeded<TResponse>(out  List<TResponse> response, Action<string, Exception> ExceptionHandler = null,
             Func<List<dynamic>, List<TResponse>> dynamicResponseToExpectedTypeCaster = null)
            {
                return ExecutionChainSucceeded(out response, null,ExceptionHandler, dynamicResponseToExpectedTypeCaster);
            }

            public bool ExecutionChainSucceeded<TResponse>(out  List<TResponse> response, TArgument arg, Action<string, Exception> exceptionHandler = null,
                Func<List<dynamic>, List<TResponse>> dynamicResponseToExpectedTypeCaster = null)
            {
                return ExecutionChainSucceeded(out response, new List<TArgument>()
                {
                    arg
                },exceptionHandler, dynamicResponseToExpectedTypeCaster);
            }

            public bool ExecutionChainSucceeded<TResponse>(out List<TResponse> response, List<TArgument> arg = null,
                Action<string,Exception> exceptionHandler=null,
            Func<List<dynamic>, List<TResponse>> dynamicResponseToExpectedTypeCaster = null)
            {
                arg = arg ?? new List<TArgument>();
                var status=false;
                List<dynamic> _response;
                response = new List<TResponse>();
                try
                {
                    var noStrategySpecifiedException = new Exception("No execution strategy was specified");

                    switch (Parent.ExecutionStrategy)
                    {
                        case ExecutionStrategy.OnlyTheFirsHandlerFoundWhoHasTheResponsibilityIsExecuted:
                            Parent.ExistingReceivers.First().ExecuteOnlyFirstMatchingHandlerInChain(out _response, arg);
                            break;

                        case ExecutionStrategy.AllHandlersFoundThatHaveTheResponsibilitiesAreExecuted:
                            Parent.ExistingReceivers.First().ExecuteAllMatchingHandlerInChain(out _response, arg);
                            break;

                        case ExecutionStrategy.Unknown:
                            throw noStrategySpecifiedException;
                        default:
                            throw noStrategySpecifiedException;
                    }

                    try
                    {
                        if (dynamicResponseToExpectedTypeCaster == null)
                        {
                            var list = response;
                            _response.ForEach(x => list.Add((TResponse)x));
                        }

                        else
                        {
                            response = dynamicResponseToExpectedTypeCaster.Invoke(_response);
                        }
                    }
                    catch (Exception e)
                    {
                        const string message = "A receiver successfully executed your request, but the response could not be cast into the type you expected";
                        var unableToCastException = new Exception(message);

                        if (exceptionHandler == null)
                            throw unableToCastException;
                      
                        exceptionHandler.Invoke(message,e);

                    }
                    status = true;
                }
                catch (Exception e)
                {
                    const string message = "one of the handlers threw an exception";
                    var handlerException = new Exception(message);

                    if (exceptionHandler == null)
                        throw handlerException;

                    exceptionHandler.Invoke(message, e);

                    
                }

                return status;
            }
        }
    }
}