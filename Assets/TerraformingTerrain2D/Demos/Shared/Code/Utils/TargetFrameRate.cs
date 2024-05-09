using UnityEngine;

namespace DemosShared
{
    public class TargetFrameRate : MonoBehaviour
    {
        [SerializeField] private int _targetRate = 60;
        
        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _targetRate;
        }
    }
}