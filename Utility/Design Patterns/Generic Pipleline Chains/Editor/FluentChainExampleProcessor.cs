using UnityEngine;
namespace AstekUtility.DesignPattern.GenericPiplelineChains
{
    public static class Chain
    {
        public static DistanceChain FromPlayer(Transform player) => new DistanceChain(new DistanceFromPlayer(player));
        public static DistanceChain Start(DistanceFromPlayer processor) => new DistanceChain(processor);
        public static DistanceChain Start<TProcessor>(TProcessor processor) where TProcessor : class, IProcessor<Vector3, float>
            => new DistanceChain(processor);
    }

    public class HighlightIfClose : IProcessor<bool, bool>
    {
        readonly Transform _transform;
        
        public  HighlightIfClose(Transform transform) => _transform = transform;

        public bool Process(bool isCloseEnough)
        {
            var renderer = _transform.GetComponent<Renderer>();
            renderer.material.color = isCloseEnough ? Color.red : Color.white;
            return isCloseEnough;
        }
    }

    public class FilteredChain : FluentChain<Vector3, bool, FilteredChain>
    {
        public FilteredChain(IProcessor<Vector3, bool> processor) : base(processor) { }

        static FilteredChain Create(IProcessor<Vector3, bool> processor) => new FilteredChain(processor);

        public FilteredChain LogTo(string system)
        {
            Debug.Log($"#{system}# Filtered Chain!");
            return this;
        }
        public FilteredChain Then<TProcessor>(TProcessor next) where TProcessor : class, IProcessor<bool, bool>
            => Then<bool, FilteredChain, TProcessor>(next, Create);
    }

    public class ScoredChain : FluentChain<Vector3, float, ScoredChain>
    {
        public ScoredChain(IProcessor<Vector3, float> processor) : base(processor) { }

        static FilteredChain CreateFilteredChain(IProcessor<Vector3, bool> processor) => new FilteredChain(processor);
        public ScoredChain WithMaxDistance(float maxDistance)
        {
            Processor = new Combined<Vector3, float, float>(Processor, new ClampByMaxDistance(maxDistance));
            return new ScoredChain(Processor);
        }
        public FilteredChain Then<TProcessor>(TProcessor filter) where TProcessor : class, IProcessor<float, bool>
            => Then<bool, FilteredChain, TProcessor>(filter, CreateFilteredChain);
    }
    internal class ClampByMaxDistance : IProcessor<float, float>
    {
        private readonly float _maxDistanceScoreThreshold;

        public ClampByMaxDistance(float maxDistance)
        {
            _maxDistanceScoreThreshold = 1f / (1f + maxDistance);
        }

        public float Process(float score) => score < _maxDistanceScoreThreshold ? 0f : score;
    }

    public class DistanceChain : FluentChain<Vector3, float, DistanceChain>
    {
        public DistanceChain(IProcessor<Vector3, float> processor) : base(processor) { }

        static ScoredChain CreateScoredChain(IProcessor<Vector3, float> processor) => new ScoredChain(processor);

        public ScoredChain Then<TProcessor>(TProcessor scorer) where TProcessor : class, IProcessor<float, float>
            => Then<float, ScoredChain, TProcessor>(scorer, CreateScoredChain);
    }
}