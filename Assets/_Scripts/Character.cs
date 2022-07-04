namespace _Scripts
{
    public interface Character
    {
        float Health { get; set; }

        void TakeDamage(float amount);
    
        void AdjustRotation();

        void CheckMovement();

        void Flip();

        public void SetCannonAngle(float angle);
    }
}
