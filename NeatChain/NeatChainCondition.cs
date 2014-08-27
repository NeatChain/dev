namespace NeatChainFx
{
    public class NeatChainCondition
    {
        public Requires<T> Requires<T>(T arg)
        {
            return new Requires<T>(arg);
        }
    }
}