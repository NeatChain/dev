using System;
using System.Collections.Generic;

namespace NeatChainFx
{
    public class CodeInjection:IDisposable
    {

        public CodeInjection(bool injectionOverWritingEnabled=true)
        {
            NeatChain.EnableInjectionOverWrite = injectionOverWritingEnabled;
            NeatChain.EnableCodeInjection = true;
        }

        public void Dispose()
        {
            NeatChain.InjectExecuteFakeMapings=new List<InjectExecuteMaping>();
            NeatChain.EnableInjectionOverWrite = true;
            NeatChain.EnableCodeInjection = false;
        }
    }
}