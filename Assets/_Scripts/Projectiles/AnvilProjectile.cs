using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class AnvilProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks slamMmFeedbacks;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private MMFeedbacks secondActivateFeedbacks;
        [SerializeField] private float gravityScaleMultiplier = 5.0f;
        [SerializeField] private float fallDamageMultiplier = 1.4f;
        [SerializeField] private float secondPhaseFallDamageMultiplier = 1.1f;
        
        // References
        protected override float Damage => Level >= 2 ? damage * 1.1f : damage;
        protected override float MaxMagnitude => Level >= 3 ? maxMagnitude * 1.1f : maxMagnitude;
        
        // Other Variables
        private bool _isActivated, _isSecondPhaseActivated;

        private void Start()
        {
            foreach (var ps in gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }
        }
        
        private void Update()
        {
            if (!_isActivated)
            {
                Direct();
            }
            
            if (Input.GetMouseButtonDown(0) && _isActivated && Level == 5 && !_isSecondPhaseActivated)
            {
                _isSecondPhaseActivated = true;
                secondActivateFeedbacks.PlayFeedbacks();
            }
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                activateMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.gravityScale *= gravityScaleMultiplier;
            transform.rotation = Quaternion.identity;
        }

        public void SecondActivate()
        {
            rigidBody2D.AddForce(Vector2.down * 30.0f);
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            
            if (_isActivated)
            {
                slamMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
            DealDamage();
        }

        public override void DealDamage()
        {
            var pos = transform.position;

            var damageDealt = _isActivated ? (_isSecondPhaseActivated ? 
                Damage * secondPhaseFallDamageMultiplier * fallDamageMultiplier :
                Damage * fallDamageMultiplier) : Damage;

            if (Level == 6 && _isActivated)
            {
                DamageHandler.i.HandleDamage(pos, Radius, damageDealt, DamageHandler.DamageType.Circular, 
                    false, GameAssets.i.stunnedBuff);    
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, damageDealt, DamageHandler.DamageType.Circular);
            }
            
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                (Level >= 4 && _isActivated) ? Radius * 1.5f : Radius, 1, DestroyTypes.Circular);
        }
    }
}
