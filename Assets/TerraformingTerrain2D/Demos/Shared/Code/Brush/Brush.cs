using TerraformingTerrain2d;
using UnityEngine;

namespace DemosShared
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class Brush : MonoBehaviour
    {
        [SerializeField] private BrushView _view;
        [SerializeField] private TerraformingMode _leftClickMode;
        [SerializeField] private TerraformingMode _rightClickMode;
        [SerializeField] private float _zoomSpeed = 20f;
        private TerraformingTerrain2D[] _terrains;

        private void Start()
        {
            _terrains = Utils.FindObjects<TerraformingTerrain2D>();
            _view.Initialize(this);
        }

        public void SetModeClick(TerraformingMode mode)
        {
            _leftClickMode = mode;
        }

        private void Update()
        {
            MoveBrush();

            if (Input.GetMouseButton(0))
            {
                Terraform(_leftClickMode);
            }

            if (Input.GetMouseButton(1))
            {
                Terraform(_rightClickMode);
            }

            _view.Radius += Input.mouseScrollDelta.y * _zoomSpeed * Time.deltaTime;
        }

        private void MoveBrush()
        {
            float z = Camera.main.nearClipPlane;
            Vector3 input = new(Input.mousePosition.x, Input.mousePosition.y, z);

            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(input);
        }

        private void Terraform(TerraformingMode mode)
        {
            foreach (TerraformingTerrain2D terrain in _terrains)
            {
                terrain.Terraform(transform.position, _view.Radius, mode);
            }
        }
    }
}