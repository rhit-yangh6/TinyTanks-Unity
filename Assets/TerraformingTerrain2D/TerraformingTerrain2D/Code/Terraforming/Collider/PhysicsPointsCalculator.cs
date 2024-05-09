using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public ref struct PhysicsPointsCalculator
    {
        public readonly Span<Vector2> Output;
        private readonly double _distanceBetweenPoints;

        public PhysicsPointsCalculator(Span<Vector2> output)
        {
            Output = output;
            Count = 0;
            _distanceBetweenPoints = 6.25E-06;
        }

        public int Count { get; private set; }

        public void Evaluate(in Span<Vector2> input)
        {
            for (int i = 0; i < input.Length; ++i)
            {
                bool pointIsValid = true;

                for (int j = i + 1; j < input.Length && pointIsValid; ++j)
                {
                    Vector2 vertex1 = input[i];
                    Vector2 vertex2 = input[j];

                    if ((vertex2 - vertex1).sqrMagnitude <= _distanceBetweenPoints)
                    {
                        pointIsValid = false;
                    }
                }

                if (pointIsValid)
                {
                    Output[Count++] = input[i];
                }
            }

            if (Count == 2 && Vector2.Distance(Output[0], Output[1]) < 0.01f) // edge condition
            {
                Count = 0;
            }
        }
    }
}