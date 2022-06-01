using System;
using System.Collections.Generic;
using System.Linq;

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