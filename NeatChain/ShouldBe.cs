using System;

namespace NeatChainFx
{
    public class ShouldBe<T>
    {
        protected Action<string> Failed = (message) =>
        {
            throw new Exception(message);
        };

        protected T Arg { set; get; }

        public ShouldBe(T arg)
        {
            Arg = arg;
        }

        public void IsNotNull(string message=null)
        {
            if (Arg == null)
            {
                Failed.Invoke(message??"Parameter is expected not to be null");
            }
        }

        public void IsAn<T>(string message = null)
        {
            IsA<T>(message);
        }

        public void IsA<T>(string message = null)
        {
            if (!(Arg is T))
            {
                Failed.Invoke(message ?? "Parameter is expected to be of the expected type ");
            }
        }
    }
}