using UnityEditor;
using UnityEngine;

namespace TerraformingTerrain2d
{
    public class TerrainGizmo
    {
        private readonly TerrainGizmoData _gizmoData;
        private readonly ScalarField _scalarField;
        private readonly GridData _gridData;
        private readonly IsoValue _isoValue;

        public TerrainGizmo(TerrainGizmoData gizmoData, ScalarField scalarField, GridData gridData, IsoValue isoValue)
        {
            _scalarField = scalarField;
            _gizmoData = gizmoData;
            _gridData = gridData;
            _isoValue = isoValue;
        }

        public void Show(Matrix4x4 modelMatrix)
        {
            #if UNITY_EDITOR
            Matrix4x4 gizmoMatrix = Gizmos.matrix;
            Color color = Gizmos.color;
            Matrix4x4 handlesMatrix = Handles.matrix;

            Gizmos.color = new Color(1f, 0.47f, 0f);
            Gizmos.matrix = modelMatrix;
            Handles.matrix = modelMatrix;

            ShowGrid();

            Gizmos.color = color;
            Gizmos.matrix = gizmoMatrix;
            Handles.matrix = handlesMatrix;
            #endif
        }

        private void ShowGrid()
        {
            for (int i = 0; i < _gridData.XDensity; ++i)
            {
                for (int j = 0; j < _gridData.YDensity; ++j)
                {
                    Vector2 worldPosition = _gridData.GridToWorldPosition(i, j);
                    Vector2 cellPosition = worldPosition + Vector2.up * _gridData.Scale / 3f;
                    Color sphereColor = _scalarField[i, j] < _isoValue.Value ? Color.green : Color.red;

                    TryShowSpheres(worldPosition, sphereColor);
                    TryShowValues(cellPosition, _scalarField[i, j]);
                }
            }
        }

        private void TryShowSpheres(Vector2 position, Color sphereColor)
        {
            if (_gizmoData.ShowSpheres)
            {
                Gizmos.color = sphereColor;
                Gizmos.DrawSphere(position, _gizmoData.SphereRadius);
            }
        }

        private void TryShowValues(Vector2 position, float value)
        {
            if (_gizmoData.ShowValues)
            {
                #if UNITY_EDITOR
                Handles.Label(position, value.ToString("F2"));
                #endif
            }
        }
    }
}