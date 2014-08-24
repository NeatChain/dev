namespace NeatChainFx
{
    public class ItIsRequired
    {
        public ShouldBe<T> That<T>(T arg)
        {
            return new ShouldBe<T>(arg);
        }
    }
}