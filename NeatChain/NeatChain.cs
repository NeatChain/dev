using System;
using System.Collections.Generic;

namespace NeatChainFx
{
    public static class NeatChain
    {
        private static bool _notificationsAboutAHandlerExecution = false;

        private static readonly List< HanlderExecutionNotification> HanlderExecutionNotifications=new List<HanlderExecutionNotification>();

        public static void SetHandlerExecutionNotification<T>(Action<HandlerExecutionEventArgs> onHandlerExecutionStarted, Action<HandlerExecutionEventArgs> onHandlerExecutionEnded = null) 
        {
            var typeToSet = typeof (T);

            //if (HanlderExecutionNotifications.Exists(x => x.HandlerType.Name == typeToSet.Name))
            //{
            //    throw new Exception("Notification for " + typeToSet.Name + " has already been registered");
            //}
            HanlderExecutionNotifications.Add(new HanlderExecutionNotification()
            {
                OnHandlerExecutionStarted = onHandlerExecutionStarted,
                HandlerType = typeToSet,
                OnHandlerExecutionEnded = onHandlerExecutionEnded
            });
            


            if (_notificationsAboutAHandlerExecution) return;
            _notificationsAboutAHandlerExecution = true;
            OnHandlerExecutionStarted += NeatChain_OnHandlerExecutionStarted;

            OnHandlerExecutionEnded += NeatChain_OnHandlerExecutionEnded;

        }

        static void NeatChain_OnHandlerExecutionEnded(object sender, HandlerExecutionEventArgs e)
        {
           HanlderExecutionNotifications.ForEach(x=> {
               if (e.HandlerFullName == x.HandlerType.FullName)
                   if (x.OnHandlerExecutionEnded != null)
                   { 
                       x.OnHandlerExecutionEnded.Invoke(e);
                   }
                                                        
           });
        }

        static void NeatChain_OnHandlerExecutionStarted(object sender, HandlerExecutionEventArgs e)
        {
            HanlderExecutionNotifications.ForEach(x => {
                if (e.HandlerFullName == x.HandlerType.FullName)
                    if (x.OnHandlerExecutionStarted != null)
                    {
  x.OnHandlerExecutionStarted.Invoke(e);
                    }
                                                         
            });
        }

        // Declare the event using EventHandler<T>
        public static event EventHandler<HandlerExecutionEventArgs> OnHandlerExecutionStarted;
        internal static void OnHandlerExecutionStartedEvent(HandlerExecutionEventArgs e)
        {
            var handler = OnHandlerExecutionStarted;
            if (handler != null)
            {
                handler(new object(), e);
            }
        }


        public static event EventHandler<HandlerExecutionEventArgs> OnHandlerExecutionEnded;
        internal static void OnHandlerExecutionEndedEvent(HandlerExecutionEventArgs e)
        {
            var handler = OnHandlerExecutionEnded;
            if (handler != null)
            {
                handler(new object(), e);
            }
        }


        

        public static class ThatAcceptsArgumentType<TArgument>
        {
            public static ChainFactory<TArgument> ToBeHandledBy = new ChainFactory<TArgument>();
        }

        public static ChainFactory<TArgument>._Then SetUp<TArgument>(ExecutionStrategy executionStrategy, params NetChainHandler<TArgument>[] receivers)
        {
            return ThatAcceptsArgumentType<TArgument>.ToBeHandledBy.TheseHandlers(executionStrategy, receivers);
        }

        public static ChainFactory<TArgument>._Then SetUp<TArgument>(params NetChainHandler<TArgument>[] receivers)
        {
            return SetUp(ExecutionStrategy.AllHandlersFoundThatHaveTheResponsibilitiesAreExecuted, receivers);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TArgument">Type of argument passed</typeparam>
        /// <typeparam name="TResult">Type of each result, whose list will be returned</typeparam>
        /// <param name="arg">actual argument of type TArgument</param>
        /// <param name="receivers">The handlers</param>
        /// <returns></returns>
        public static _Execute<TArgument> SetUpWithArgument<TArgument>(TArgument arg, params NetChainHandler<TArgument>[] receivers)
        {
            return new _Execute<TArgument>(arg, receivers);
        }

        public class _Execute<TArgument>
        {
            private TArgument Arg { set; get; }

            private NetChainHandler<TArgument>[] Receivers { set; get; }

            public _Execute(TArgument arg, params NetChainHandler<TArgument>[] receivers)
            {
                Arg = arg;
                Receivers = receivers;
            }

            public List<TResult> Execute<TResult>()
            {
                var chainSetUp = SetUp(ExecutionStrategy.AllHandlersFoundThatHaveTheResponsibilitiesAreExecuted, Receivers);
                List<TResult> result;
                chainSetUp.ExecutionChainSucceeded(out result, Arg);
                return result;
            }
        }
    }
}