using TerraformingTerrain2d;
using UnityEngine;

namespace DemosShared
{
    [RequireComponent(typeof(TerraformingTerrain2D))]
    public class TerraformingTerrain2DRestart : MonoBehaviour, IRestart
    {
        [SerializeField] private TerraformingTerrain2D _terrain;

        void IRestart.Restart()
        {
            _terrain.Clear();
        }
    }
}