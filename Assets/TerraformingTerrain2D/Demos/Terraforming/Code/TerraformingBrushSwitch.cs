using DemosShared;
using UnityEngine;

namespace PaintingGame
{
    public class TerraformingBrushSwitch : MonoBehaviour, IPlay, IRestart
    {
        void IPlay.Play()
        {
            gameObject.SetActive(false);
        }

        void IRestart.Restart()
        {
            gameObject.SetActive(true);
        }
    }
}