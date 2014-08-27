using System;

namespace NeatChainFx.Tests
{
    public class Execution
    {
        public static bool ThrewException(Action action)
        {
            var exceptionIsThrown = false;
            try
            {
                action.Invoke();
            }
            catch (Exception)
            {

                exceptionIsThrown = true;
            }

            return exceptionIsThrown;
        }
    }
}