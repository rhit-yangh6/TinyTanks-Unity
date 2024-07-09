using UnityEngine;

namespace _Scripts.Projectiles
{
    public class PuzzleFallProjectile : DerivedProjectile
    {
        [SerializeField] private Sprite[] sprites;
        
        private SpriteRenderer _spriteRenderer;
        
        private void Update()
        {
            Spin(1.1f);
        }

        public void ChangeSprite(int num)
        {
            SpriteRenderer.sprite = sprites[num];
        }
    }
}