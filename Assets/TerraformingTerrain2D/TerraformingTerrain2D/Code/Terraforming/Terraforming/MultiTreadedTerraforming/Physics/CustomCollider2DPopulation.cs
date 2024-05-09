using System;
using Unity.Collections;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public struct CustomCollider2DPopulation
    {
        [NativeDisableParallelForRestriction] [WriteOnly]
        private NativeArray<Vector2> _colliderVertices;

        [NativeDisableParallelForRestriction] [WriteOnly]
        private NativeArray<PhysicsShape2D> _shapes;

        private NativeCounter.Concurrent _physicsShapeCounter;
        private readonly int _shapesCount;
        private readonly int _stride;

        public CustomCollider2DPopulation(NativeArray<Vector2> colliderVertices, NativeArray<PhysicsShape2D> shapes,
            NativeCounter.Concurrent physicsShapeCounter, int shapesCount = 0) : this()
        {
            _physicsShapeCounter = physicsShapeCounter;
            _colliderVertices = colliderVertices;
            _shapesCount = shapesCount;
            _shapes = shapes;
            _stride = 6; // marching squares max vertices
        }

        public void AddCollider(in Span<Vector2> vertices)
        {
            PhysicsPointsCalculator calculator = new(stackalloc Vector2[vertices.Length]);
            calculator.Evaluate(in vertices);

            if (calculator.Count >= 2)
            {
                int shapeIndex = _shapesCount + _physicsShapeCounter.Increment();

                for (int i = 0; i < calculator.Count; ++i)
                {
                    _colliderVertices[shapeIndex * _stride + i] = calculator.Output[i];
                }

                _shapes[shapeIndex] = new PhysicsShape2D()
                {
                    shapeType = calculator.Count == 2 ? PhysicsShapeType2D.Edges : PhysicsShapeType2D.Polygon,
                    vertexStartIndex = shapeIndex * _stride,
                    vertexCount = calculator.Count
                };
            }
        }
    }
}