using System;
using UnityEngine;
namespace Astek.DesignPattern.GenericPiplelineChains
{
    public class ThresholdFilter : IProcessor<float, bool>
    {
        readonly Func<float> getThreshold;

        public ThresholdFilter(Func<float> getThreshold)
        {
            this.getThreshold = getThreshold;
        }

        public bool Process(float score) => score >= getThreshold();
    }
    public class DistanceScorer : IProcessor<float, float>
    {
        public float Process(float distance) => 1f / (1f + distance);
    }
    public class DistanceFromPlayer : IProcessor<Vector3, float>
    {
        private readonly Transform player;

        public DistanceFromPlayer(Transform player)
        {
            this.player = player;
        }

        public float Process(Vector3 position) => Vector3.Distance(position, player.position);
    }
}