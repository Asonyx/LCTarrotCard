using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LCTarrotCard.Util {
    public class ProbabilityRangedList<T> {
        
        private readonly List<RangedItem<T>> rangedItems = new List<RangedItem<T>>();


        /**
         * range: the distance between min and max range
         * weight: the weight of the item, higher means more likely to be chosen
         * overlap (a percentage): how much the range can overlap with other items' ranges
         */
        public void Add(T item, float range, float weight, float value) {
            float minRange = value - range / 2;
            float maxRange = value + range / 2;
            if (rangedItems.Count > 0) {
                RangedItem<T> lastItem = rangedItems[rangedItems.Count - 1];
                if (minRange < lastItem.MaxRange) {
                    minRange = lastItem.MaxRange;
                    maxRange = minRange + range;
                }
            }
            rangedItems.Add(new RangedItem<T>(item, minRange, maxRange, weight));
            CheckRanges();

        }
        
        public T GetRandom(float value, float softMaxTemperature = 1f) {
            Dictionary<int, float> weightsMap = new Dictionary<int, float>();
            foreach (RangedItem<T> rangedItem in rangedItems) {
                float weight = rangedItem.GetWeightAt(value);
                if (weight <= float.Epsilon) continue;
                weightsMap.Add(rangedItems.IndexOf(rangedItem), weight);
            }

            if (weightsMap.Count == 0) return default;
            List<float> weights = weightsMap.Values.ToList();
            List<float> softMaxWeights = SoftMax(weights, softMaxTemperature);
            float rng = Random.Range(0f, 1f);
            float cumulative = 0f;
            for (int i = 0; i < softMaxWeights.Count; i++) {
                cumulative += softMaxWeights[i];
                if (rng <= cumulative) {
                    return rangedItems[weightsMap.Keys.ToList()[i]].Item;
                }
            }
            throw new ArithmeticException("No item found, report this bug");
        }
        
        private static List<float> SoftMax(List<float> values, float temperature = 1f) {
            if (temperature <= float.Epsilon) {
                int maxIndex = -1;
                float maxValue = float.NegativeInfinity;

                for (int i = 0; i < values.Count; i++) {
                    if (!(values[i] > maxValue)) continue;
                    maxValue = values[i];
                    maxIndex = i;
                }

                List<float> result = new List<float>(new float[values.Count]);
                if (maxIndex != -1) {
                    result[maxIndex] = 1.0f;
                }
                return result;
            }
            List<float> expValues = values.Select(v => Mathf.Exp(v / temperature)).ToList();
            float sumExp = expValues.Sum();
            return expValues.Select(v => v / sumExp).ToList();
        }

        /**
         * Check that ranges are correctly set (no gaps, in order)
         */
        private void CheckRanges() {
            float minRange = -1;
            float maxRange = -1;
            bool redo = false;
            foreach (RangedItem<T> item in rangedItems) {
                if (item.MinRange < minRange) redo = true;
                if (item.MinRange > maxRange && !Mathf.Approximately(minRange, -1)) redo = true;
                minRange = item.MinRange;
                maxRange = item.MaxRange;
            }

            if (!redo) return;
            List<RangedItem<T>> newRangedItems = new List<RangedItem<T>>();
            // Doing an n² sort because list are expected to be small
            while (rangedItems.Count > 0) {
                RangedItem<T> minItem = rangedItems[0];
                foreach (RangedItem<T> item in rangedItems.Where(item => item.MinRange < minItem.MinRange)) {
                    minItem = item;
                }
                newRangedItems.Add(minItem);
                rangedItems.Remove(minItem);
            }
            rangedItems.AddRange(newRangedItems);
        }
        
        


        /**
         * Private class because the range will be managed internally to avoid bad range usage
         */
        private class RangedItem<TK> {
            public TK Item;
            public float MinRange;
            public float MaxRange;
            public float Weight;

            public RangedItem(TK item, float minRange, float maxRange, float weight) {
                Item = item;
                MinRange = minRange;
                MaxRange = maxRange;
                Weight = weight;
            }
            
            public bool IsInRange(float value) {
                return value >= MinRange && value <= MaxRange;
            }
            
            public float GetWeightAt(float value) {
                if (!IsInRange(value)) return 0;
                return Weight / Distance(value);
            }

            public float Distance(float value) {
                float mid = (MinRange + MaxRange) / 2;
                return Mathf.Abs(mid - value) + 1; // +1 to avoid division by zero
            }
        }
    }
}