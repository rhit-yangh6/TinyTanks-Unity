namespace _Scripts.Projectiles
{
    public class MinigunSecondaryProjectile: DerivedProjectile
    {
        protected override void Start()
        {
            base.Start();
            StartCoroutine(TemporarilyDisableCollider());
        }
        
        private void Update() { Direct(); }
    }
}