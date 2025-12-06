using System;

namespace AstekUtility.DesignPattern.GenericPiplelineChains
{
     public abstract class FluentChain<TIn, TOut, TDerived> where TDerived : FluentChain<TIn, TOut, TDerived>
    {
        public IProcessor<TIn, TOut> Processor;

        protected FluentChain(IProcessor<TIn, TOut> processor)
        {
            Processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        protected TNextSelf Then<TNext, TNextSelf, TProcessor>(
            TProcessor nextProcessor,
            ChainFactory<TIn, TNext, TNextSelf> factory)
            where TNextSelf : FluentChain<TIn, TNext, TNextSelf>
            where TProcessor : class, IProcessor<TOut, TNext>
        {
            if (nextProcessor == null) throw new ArgumentNullException(nameof(nextProcessor));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return factory(new Combined<TIn, TOut, TNext>(Processor, nextProcessor));
        }

        //Executes the chain immediately
        public TOut Run(TIn input)
        {
            if(Processor == null) throw new InvalidOperationException("Processor is not initialized.Use Chain.Start() to begin a chain");
            return Processor.Process(input);
        }
        
        //Turn the pipeline into a reusable function
        public ProcessorDelegate<TIn, TOut> Compile()
        {
            if(Processor == null) throw new InvalidOperationException("Processor is not initialized.Use Chain.Start() to begin a chain");
            return input => Processor.Process(input);
        }
    }
    //Factory delegate type for creating chain instances
    public delegate TChain ChainFactory<out TIn, in TOut, out TChain>(IProcessor<TIn, TOut> processor) where TChain : FluentChain<TIn, TOut, TChain>;
}