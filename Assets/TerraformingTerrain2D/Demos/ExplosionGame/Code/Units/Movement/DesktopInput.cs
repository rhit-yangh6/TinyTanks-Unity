using UnityEngine;

namespace ExplosionGame
{
    public class DesktopInput : IInput
    {
        public Vector2 Direction => new(Input.GetAxisRaw("Horizontal"), 0);
    }
}