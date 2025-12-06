namespace AstekUtility.DesignPattern.GenericPiplelineChains
{
    public interface IProcessor<in TIn, out TOut>
    {
        TOut Process(TIn input);
    }
    
    public class Combined<A, B, C> : IProcessor<A, C>
    {
        private readonly IProcessor<A, B> _first;
        private readonly IProcessor<B, C> _second;

        public Combined(IProcessor<A, B> first, IProcessor<B, C> second)
        {
            _first = first;
            _second = second;
        }
        public C Process(A input) => _second.Process(_first.Process(input));
    }
}