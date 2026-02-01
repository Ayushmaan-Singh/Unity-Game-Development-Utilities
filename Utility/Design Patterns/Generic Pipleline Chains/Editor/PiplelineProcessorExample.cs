using System;
using UnityEngine;
namespace Astek.DesignPattern.GenericPiplelineChains
{
    public class PiplelineProcessorExample : MonoBehaviour
    {
        private ProcessorDelegate<Vector3, bool> shouldAttack;
        private ProcessorDelegate<Vector3, bool> highlightClose;

        [SerializeField, Range(0.01f, 1f)] private float distanceThreshold = 0.1f;
        public Transform target;

        private void Start()
        {
            //Simple Chain Example
            ProcessorDelegate<Vector3, bool> simpleChain = Chain<Vector3, float>.Start(new DistanceFromPlayer(transform))
                                                                                .Then(new DistanceScorer())
                                                                                .Then(new ThresholdFilter(() => distanceThreshold))
                                                                                .Compile();

            //Fluent Chain Example #1
            shouldAttack = Chain.Start(new DistanceFromPlayer(transform))
                                .Then(new DistanceScorer())
                                .WithMaxDistance(15f)
                                .Then(new ThresholdFilter(() => distanceThreshold))
                                .LogTo("Score")
                                .LogTo("Proximity")
                                .Compile();
            
            //Fluent Chain Example #2
            highlightClose = Chain.Start(new DistanceFromPlayer(transform))
                                  .Then(new DistanceScorer())
                                  .Then(new ThresholdFilter(() => distanceThreshold))
                                  .Then(new HighlightIfClose(transform))
                                  .Compile();
        }
    }
}