using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class TerraformingTerrainCollider
    {
        private readonly CustomCollider2D _customCollider2D;
        private readonly List<Vector2> _physicsPoints;
        private readonly PhysicsShapeGroup2D _shape;
        private readonly Vector2[] _boxVertices;

        public TerraformingTerrainCollider(CustomCollider2D customCollider2D)
        {
            _customCollider2D = customCollider2D;
            _shape = new PhysicsShapeGroup2D();
            _physicsPoints = new List<Vector2>(6);
            _boxVertices = new Vector2[4];
        }

        public void ClearCustomShapes()
        {
            _customCollider2D.ClearCustomShapes();
        }

        public void ClearShape()
        {
            _shape.Clear();
        }

        public void SetShapes()
        {
            _customCollider2D.SetCustomShapes(_shape);
        }

        public void SetShapes(NativeArray<PhysicsShape2D> shapes, NativeArray<Vector2> vertices, int shapeCount)
        {
            if (shapeCount != 0)
            {
                NativeArray<PhysicsShape2D> slice = shapes.GetSubArray(0, shapeCount);
                _customCollider2D.SetCustomShapes(slice, vertices);                
            }
        }

        public void UpdateOffset()
        {
            Transform transform = _customCollider2D.transform;
            Vector2 offset = new(transform.position.x / transform.lossyScale.x, transform.position.y / transform.lossyScale.y);
            offset = Quaternion.Inverse(transform.rotation) * offset;
            _customCollider2D.offset = offset;
        }

        public void AddRectangle(Vector2 min, Vector2 max)
        {
            _boxVertices[0] = min;
            _boxVertices[1] = new Vector2(min.x, max.y);
            _boxVertices[3] = max;
            _boxVertices[2] = new Vector2(max.x, min.y);

            AddColliderPart(_boxVertices);
        }

        public void AddColliderPart(Vector2[] points)
        {
            _physicsPoints.Clear();

            Span<Vector2> input = new(points);
            PhysicsPointsCalculator calculator = new(stackalloc Vector2[input.Length]);
            calculator.Evaluate(in input);

            for (int i = 0; i < calculator.Count; ++i)
            {
                _physicsPoints.Add(calculator.Output[i]);
            }

            TryAddPolygon();
            TryAddEdge();
        }

        private void TryAddPolygon()
        {
            if (_physicsPoints.Count >= 3)
            {
                _shape.AddPolygon(_physicsPoints);
            }
        }

        private void TryAddEdge()
        {
            if (_physicsPoints.Count == 2)
            {
                _shape.AddEdges(_physicsPoints);
            }
        }
    }
}