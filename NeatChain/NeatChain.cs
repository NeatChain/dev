using System;
using System.Collections.Generic;

namespace NeatChainFx
{
    public static class NeatChain
    {

        
        public static T @where<T>(Func<T> body)
        {
          return   NeatChain.CodeAt<DefaultCodeThatReturns, dynamic>(() =>(dynamic) body.Invoke());
        }

        public static void @where(Action body)
        {
            NeatChain.CodeAt<DefaultCodeThatReturns>(body.Invoke);
        }


        /// <summary>
        /// Warning!! Only enable this in unit test
        /// </summary>
        internal static bool EnableCodeInjection { set; get; }
        internal static bool EnableInjectionOverWrite { set; get; }
        internal static  List<InjectExecuteMaping> InjectExecuteFakeMapings = new List<InjectExecuteMaping>();

        private static void SetInjectExecuteFake<TInterceptionLabelAt, TInterceptionReturnType>(Func<TInterceptionReturnType> splitExecutBody)
            where TInterceptionLabelAt : CodeThatReturns<TInterceptionReturnType>
            where TInterceptionReturnType : new()
        {
            var typeFullName = typeof(TInterceptionLabelAt).FullName;

            var exists =  InjectExecuteFakeMapings.Exists((x) =>x.AUniqueInjectExecuteLabelFullName == typeFullName);
           
            if (exists)
            {
                if (!EnableInjectionOverWrite)
                {
                        return;
                }
                var index = 0;
                InjectExecuteFakeMapings.ForEach((x) =>
                {
                    if (x.AUniqueInjectExecuteLabelFullName == typeFullName)
                    {

                        InjectExecuteFakeMapings[index].SplitExecutBody = splitExecutBody;
                    }
                    index++;
                });
                return;
            }
          
            InjectExecuteFakeMapings.Add(new InjectExecuteMaping()
            {
                SplitExecutBody = splitExecutBody,

                AUniqueInjectExecuteLabelFullName = typeFullName
            });
        }

        public static class Inject
        {
            private static readonly Exception IncorrectUseOfCodeInjectionException = new  Exception("Please Inject Code only within the  using (new CodeInjection()) block");

            public static void CodeAt<TInterceptionLabelAt, TInterceptionReturnType>(Func<TInterceptionReturnType> splitExecutBody)
                where TInterceptionLabelAt : CodeThatReturns<TInterceptionReturnType>
                where TInterceptionReturnType : new()
            {
                if (!NeatChain.EnableCodeInjection)
                {
                    throw IncorrectUseOfCodeInjectionException;
                }

               

                    if (typeof(TInterceptionLabelAt).FullName == typeof(CodeThatReturns<TInterceptionReturnType>).FullName)
                    {
                        throw new Exception("You cannot use 'InjectExecuteLabel' class as an InjectExecute Label. Please create and use only instances of it");
                    }
                    SetInjectExecuteFake<TInterceptionLabelAt, TInterceptionReturnType>(splitExecutBody);
         

            }

            public static void CodeAt<TInterceptionLabelAt>(Action splitExecutBody)
                where TInterceptionLabelAt : CodeThatReturns<object>
            {

                if (!NeatChain.EnableCodeInjection)
                {
                    throw    IncorrectUseOfCodeInjectionException;
                }

                CodeAt<TInterceptionLabelAt, object>(() =>
                    {
                        splitExecutBody.Invoke();
                        return true;
                    });
           
            }
            public static void EmptyCodeAt<TInterceptionLabelAt>()
                where TInterceptionLabelAt : CodeThatReturns<object>
            {


                if (!NeatChain.EnableCodeInjection)
                {
                    throw IncorrectUseOfCodeInjectionException;
                }


                CodeAt<TInterceptionLabelAt>(() =>{});

            }




           

            public static void @where<T>(Func<T> body)
            {
                CodeAt<DefaultCodeThatReturns,object>(() => body.Invoke());
            }

            public static void @where(Action body)
            {
                
                CodeAt<DefaultCodeThatReturnsVoid, object>(() =>
                {
                    body.Invoke();
                   
                    return 0;
                });
            }








           
        }

        public static void CodeAt<TInterceptionLabelAt>(Action splitExecutBody) where TInterceptionLabelAt : CodeThatReturns<object>
        {
            CodeAt<TInterceptionLabelAt, object>(() =>
            {
                splitExecutBody.Invoke();
                return true;
            });
        }

        public static TInterceptionReturnType CodeAt<TInterceptionLabelAt, TInterceptionReturnType>(Func<TInterceptionReturnType> splitExecutBody) where TInterceptionLabelAt : CodeThatReturns<TInterceptionReturnType>
        {
            if (!EnableCodeInjection)
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

        public static ChainFactory<TArgument>._Then Build<TArgument>(ExecutionStrategy executionStrategy, params NeatChainHandler<TArgument>[] receivers)
        {
            return ThatAcceptsArgumentType<TArgument>.ToBeHandledBy.TheseHandlers(executionStrategy, receivers);
        }

        //public static ChainFactory<TArgument>._Then SetUp<TArgument>(params NeatChainHandler<TArgument>[] receivers)
        //{
        //    return Build<TArgument>(ExecutionStrategy.AllPossibleHandlers, receivers);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TArgument">Type of argument passed</typeparam>
        /// <typeparam name="TResult">Type of each result, whose list will be returned</typeparam>
        /// <param name="arg">actual argument of type TArgument</param>
        /// <param name="receivers">The handlers</param>
        /// <returns></returns>
        public static _Execute<TArgument> Build<TArgument>(TArgument arg, params NeatChainHandler<TArgument>[] receivers)
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

            public List<TResult> Execute<TResult>(ExecutionStrategy? executionStrategy=null)
            {
                var chainSetUp = Build(executionStrategy??ExecutionStrategy.AllPossibleHandlers, Receivers);
                List<TResult> result;
                chainSetUp.ExecutionChainSucceeded(out result, Arg);
                return result;
            }
        }
    }
}