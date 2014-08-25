namespace NeatChainFx
{
    public class ChainCondition
    {
        public Requires<T> Requires<T>(T arg)
        {
            return new Requires<T>(arg);
        }
    }
}