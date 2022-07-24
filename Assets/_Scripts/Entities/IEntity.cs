using _Scripts.Buffs;

namespace _Scripts.Entities
{
    public interface IEntity
    {
        float Health { get; set; }

        void TakeDamage(float amount);
    
        void AdjustRotation();

        void CheckMovement();

        void Flip();

        public void SetCannonAngle(float angle);

        public void AddBuff(TimedBuff buff);

        // TODO: merge?
        public void TickBuffs();

        public void IncreaseMovementSpeed(float amount);
    }
}
