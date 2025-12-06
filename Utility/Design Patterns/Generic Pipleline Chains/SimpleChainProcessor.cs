using System;
namespace AstekUtility.DesignPattern.GenericPiplelineChains
{
    public class Chain<TIn, TOut>
    {
        private readonly IProcessor<TIn, TOut> _processor;

        public Chain(IProcessor<TIn, TOut> processor)
        {
            _processor = processor;
        }
        public static Chain<TIn, TOut> Start(IProcessor<TIn, TOut> processor) => new Chain<TIn, TOut>(processor);

        public Chain<TIn, TNext> Then<TNext>(IProcessor<TOut, TNext> next)
        {
            Combined<TIn, TOut, TNext> combined = new Combined<TIn, TOut, TNext>(_processor, next);
            return new Chain<TIn, TNext>(combined);
        }

        //Executes the chain immediately
        public TOut Run(TIn input) => _processor.Process(input);

        //Turn the pipeline into a reusable function
        public ProcessorDelegate<TIn, TOut> Compile() => input => _processor.Process(input);
    }
    public delegate TOut ProcessorDelegate<in TIn, out TOut>(TIn input);
}