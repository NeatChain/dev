using System;
using System.Collections.Generic;

namespace NeatChainFx
{
    public static class NeatChain
    {




        /// <summary>
        /// Warning!! Only enable this in unit test
        /// </summary>
        internal static bool InterceptCodeEnabled { set; get; }

        private static readonly List<InjectExecuteMaping> InjectExecuteFakeMapings = new List<InjectExecuteMaping>();

        private static void SetInjectExecuteFake<TInterceptionLabelAt, TInterceptionReturnType>(Func<TInterceptionReturnType> splitExecutBody)
            where TInterceptionLabelAt : InterceptionReturnType<TInterceptionReturnType>
            where TInterceptionReturnType : new()
        {
            var typeFullName = typeof(TInterceptionLabelAt).FullName;

            var exists = false;
            InjectExecuteFakeMapings.ForEach((x) =>
            {
                if (x.AUniqueInjectExecuteLabelFullName == typeFullName)
                {
                    x.SplitExecutBody = splitExecutBody;
                    exists = true;
                }
            }
        );
            if (exists) return;
            InjectExecuteFakeMapings.Add(new InjectExecuteMaping()
            {
                SplitExecutBody = splitExecutBody,

                AUniqueInjectExecuteLabelFullName = typeFullName
            });
        }

        public static class Intercept
        {
            public static void CodeAt<TInterceptionLabelAt, TInterceptionReturnType>(Func<TInterceptionReturnType> splitExecutBody)
                where TInterceptionLabelAt : InterceptionReturnType<TInterceptionReturnType>
                where TInterceptionReturnType : new()
            {

                    if (typeof(TInterceptionLabelAt).FullName == typeof(InterceptionReturnType<TInterceptionReturnType>).FullName)
                    {
                        throw new Exception("You cannot use 'InjectExecuteLabel' class as an InjectExecute Label. Please create and use only instances of it");
                    }
                    SetInjectExecuteFake<TInterceptionLabelAt, TInterceptionReturnType>(splitExecutBody);
         

            }

            public static void CodeAt<TInterceptionLabelAt>(Action splitExecutBody)
                where TInterceptionLabelAt : InterceptionReturnType<object>
            {

                CodeAt<TInterceptionLabelAt, object>(() =>
                    {
                        splitExecutBody.Invoke();
                        return true;
                    });
           
            }
            public static void RemoveCodeAt<TInterceptionLabelAt>()
                where TInterceptionLabelAt : InterceptionReturnType<object>
            {

                CodeAt<TInterceptionLabelAt>(() =>{});

            }
           
        }

        public static void CodeAt<TInterceptionLabelAt>(Action splitExecutBody) where TInterceptionLabelAt : InterceptionReturnType<object>
        {
            CodeAt<TInterceptionLabelAt, object>(() =>
            {
                splitExecutBody.Invoke();
                return true;
            });
        }

        public static TInterceptionReturnType CodeAt<TInterceptionLabelAt, TInterceptionReturnType>(Func<TInterceptionReturnType> splitExecutBody) where TInterceptionLabelAt : InterceptionReturnType<TInterceptionReturnType>
        {
            if (!InterceptCodeEnabled)
            {
                return splitExecutBody.Invoke();
            }
            var TypeOfLabel = typeof(TInterceptionLabelAt).FullName;

            var availableFakeExecution = InjectExecuteFakeMapings.Find(x => x.AUniqueInjectExecuteLabelFullName == TypeOfLabel);

            if (availableFakeExecution == null)
                return splitExecutBody.Invoke();

            try
            {
                object result;
                try
                {
                    result = ((Func<TInterceptionReturnType>)availableFakeExecution.SplitExecutBody).Invoke();
                }
                catch (Exception e)
                {
                    throw new Exception("An exception occured while trying to run fake execution provided for " + TypeOfLabel + ". Please see inner exception for details", e);
                }

                return (TInterceptionReturnType)result;
            }
            catch (Exception e)
            {
                throw new Exception("An exception occured while trying to cast result of the fake execution provided for " + TypeOfLabel + ". Please see inner exception for details", e);
            }
        }

        private static bool NotificationsAboutAHandlerExecution { set; get; }

        private static readonly List<HanlderExecutionNotification> HanlderExecutionNotifications = new List<HanlderExecutionNotification>();

        public static void SetHandlerExecutionNotification<TInterceptionReturnType>(Action<HandlerExecutionEventArgs> onHandlerExecutionStarted, Action<HandlerExecutionEventArgs> onHandlerExecutionEnded = null)
        {
            var typeToSet = typeof(TInterceptionReturnType);

            HanlderExecutionNotifications.Add(new HanlderExecutionNotification()
            {
                OnHandlerExecutionStarted = onHandlerExecutionStarted,
                HandlerType = typeToSet,
                OnHandlerExecutionEnded = onHandlerExecutionEnded
            });

            if (NotificationsAboutAHandlerExecution) return;
            NotificationsAboutAHandlerExecution = true;
            OnHandlerExecutionStarted += NeatChain_OnHandlerExecutionStarted;

            OnHandlerExecutionEnded += NeatChain_OnHandlerExecutionEnded;
        }

        private static void NeatChain_OnHandlerExecutionEnded(object sender, HandlerExecutionEventArgs e)
        {
            HanlderExecutionNotifications.ForEach(x =>
            {
                if (e.HandlerFullName == x.HandlerType.FullName)
                    if (x.OnHandlerExecutionEnded != null)
                    {
                        x.OnHandlerExecutionEnded.Invoke(e);
                    }
            });
        }

        private static void NeatChain_OnHandlerExecutionStarted(object sender, HandlerExecutionEventArgs e)
        {
            HanlderExecutionNotifications.ForEach(x =>
            {
                if (e.HandlerFullName != x.HandlerType.FullName) return;
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

        public static ChainFactory<TArgument>._Then SetUp<TArgument>(ExecutionStrategy executionStrategy, params NeatChainHandler<TArgument>[] receivers)
        {
            return ThatAcceptsArgumentType<TArgument>.ToBeHandledBy.TheseHandlers(executionStrategy, receivers);
        }

        public static ChainFactory<TArgument>._Then SetUp<TArgument>(params NeatChainHandler<TArgument>[] receivers)
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
        public static _Execute<TArgument> SetUpWithArgument<TArgument>(TArgument arg, params NeatChainHandler<TArgument>[] receivers)
        {
            return new _Execute<TArgument>(arg, receivers);
        }

        public class _Execute<TArgument>
        {
            private TArgument Arg { set; get; }

            private NeatChainHandler<TArgument>[] Receivers { set; get; }

            public _Execute(TArgument arg, params NeatChainHandler<TArgument>[] receivers)
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