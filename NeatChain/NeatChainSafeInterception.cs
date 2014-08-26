using System;

namespace NeatChainFx
{
    public class NeatChainCodeInterception:IDisposable
    {

        public NeatChainCodeInterception()
        {
            NeatChain.InterceptCodeEnabled = true;
        }

        public void Dispose()
        {
            NeatChain.InterceptCodeEnabled = false;
        }
    }
}