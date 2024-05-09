using UnityEngine;

namespace DemosShared
{
    public interface IOnTriggerEnter2D : IUnityCallback
    {
        void Entered(Collider2D collider2D);
    }
}