using System;

namespace NeatChainFx
{
    public class HandlerExecutionEventArgs : EventArgs
    {
        public Type HandlerType { set; get; }
        public DateTime CurrentTime { set; get; }
        public string HandlerName { get; set; }
        public string HandlerFullName { get; set; }
    }
}