using UnityEngine;

namespace DemosShared
{
    public class Ball : MonoBehaviour, IPlay, IRestart
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        
        void IPlay.Play()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        void IRestart.Restart()
        {
            _rigidbody.bodyType = RigidbodyType2D.Static;
        }
    }
}