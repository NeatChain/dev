using System;

namespace NeatChainFx
{
    internal class HanlderExecutionNotification
    {
        public Action<HandlerExecutionEventArgs> OnHandlerExecutionStarted { set; get; }
        public Action<HandlerExecutionEventArgs> OnHandlerExecutionEnded { set; get; }
        public Type HandlerType { set; get; }
    }
}