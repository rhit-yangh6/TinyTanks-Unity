using System;
using Unity.Collections;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [Serializable]
    public class MultithreadingUsage
    {
        [SerializeField] private bool _useMultithreading;

        public bool UseMultithreading => _useMultithreading;
        
        public void CheckCompability(Transform transform)
        {
            if (_useMultithreading)
            {
                CustomCollider2D customCollider2D = transform.GetComponentInChildren<CustomCollider2D>();

                try
                {
                    CheckCompability(customCollider2D);
                }
                catch
                {
                    _useMultithreading = false;
                    Debug.LogWarning("Multithreaded version doesn't work at this Unity version. Please update Unity to at least: 2022.3.13f1 2023.1.20f1 2023.2.0b17 2023.3.0a13");
                }
            }
        }

        private void CheckCompability(CustomCollider2D customCollider2D)
        {
            NativeArray<PhysicsShape2D> shapes = new(2, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            NativeArray<Vector2> vertices = new(8, Allocator.Temp);
            
            vertices[0] = new Vector2(1, 1);
            vertices[1] = new Vector2(1, 2);
            vertices[2] = new Vector2(2, 1);
            vertices[3] = new Vector2(2, 2);
            vertices[4] = new Vector2(1, 1) * 2;
            vertices[5] = new Vector2(1, 2) * 2;
            vertices[6] = new Vector2(2, 1) * 2;
            vertices[7] = new Vector2(2, 2) * 2;
            shapes[0] = new PhysicsShape2D { shapeType = PhysicsShapeType2D.Polygon, vertexStartIndex = 0, vertexCount = 3 };
            shapes[1] = new PhysicsShape2D { shapeType = PhysicsShapeType2D.Polygon, vertexStartIndex = 4, vertexCount = 3 };
            
            customCollider2D.SetCustomShapes(shapes, vertices);
            
            shapes.Dispose();
            vertices.Dispose();
        }
    }   
}