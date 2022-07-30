using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace stoogebag
{
    public static class RandomExtensions
    {
        public static Random random = new Random();
        public static T RandomSample<T>(this IProbabilityDistribution<T> dist)
        {
            var total = dist.GetTotalWeight();

            var x = random.NextDouble();
            var val = 0f;
            foreach (var (item, weight) in dist.Weights)
            {
                val += weight/total;
                if (val > x) return item;
            }

            throw new Exception("something is wrong lmao");
        }
        
        //from https://stackoverflow.com/questions/5817490/implementing-box-mueller-random-number-generator-in-c-sharp
        public static float RandomGaussian(float mean = 0f, float sigma = 1f)
        {
            float u, v, S;
 
            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                v = 2.0f * UnityEngine.Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);
 
            // Standard Normal Distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
 
            // Normal Distribution centered between the min and max value
            // and clamped following the "three-sigma rule"
            //float mean = (minValue + maxValue) / 2.0f;
            //float sigma = (maxValue - mean) / 3.0f;
            //return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
            return std * sigma + mean;
        }
        //from https://stackoverflow.com/questions/5817490/implementing-box-mueller-random-number-generator-in-c-sharp
        public static float RandomGaussianInInterval(float minValue = 0, float maxValue = 1)
        {
            float u, v, S;
 
            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                v = 2.0f * UnityEngine.Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);
 
            // Standard Normal Distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
 
            // Normal Distribution centered between the min and max value
            // and clamped following the "three-sigma rule"
            float mean = (minValue + maxValue) / 2.0f;
            float sigma = (maxValue - mean) / 3.0f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }
        
    }

    public interface IProbabilityDistribution<T>
    {
        public float GetTotalWeight();
        public IEnumerable<(T, float)> Weights { get; }
    }
    public class SimpleProbabilityDist<T> :IProbabilityDistribution<T>
    {
        private List<T> _values;

        public List<T> Values
        {
            get
            {
                if(_values == null) _values = Weights.Select(t => t.Item1).ToList();
                return _values;
            }
        }

        IEnumerable<(T, float)> IProbabilityDistribution<T>.Weights => Weights;
        public List<(T, float)> Weights;

        public float GetTotalWeight()
        {
            return Weights.Sum(t => t.Item2);
        }

    }

}