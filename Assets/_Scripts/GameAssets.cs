using _Scripts.Buffs;
using Destructible2D.Examples;
using UnityEngine;

namespace _Scripts
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets i
        {
            get
            {
                if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return _i;
            }
        }

        public Transform damagePopupPrefab;

        public Sprite[] diceNumbers;
        public Sprite[] puzzlePieces;

        public Sprite[] stars;

        public Sprite weaponLockedSprite;
        public Sprite weaponEmptySprite;

        public GameObject regularExplosionFX;
        public GameObject explosionFX;
        public GameObject gunpowderlessExplosionFX;
        public GameObject squareExplosionFX;
        public GameObject virusExplosionFX;
        public GameObject healFX;
        public GameObject blackExplosionFX;
        public GameObject swordShadowFX;
        public GameObject sacrificeFX;
        public GameObject shockwaveFX;
        public GameObject coconutTreeFX;
        public GameObject targetFX;
        public GameObject electricLineFX;
        public GameObject chargeFX;
        public GameObject curseFX;
        public GameObject deathFX;

        public ScriptableBuff burningBuff;
        public ScriptableBuff infectedBuff;
        public ScriptableBuff speedBuff;
        public ScriptableBuff stunnedBuff;
        public ScriptableBuff cursedBuff;
        public ScriptableBuff frozenBuff;
        public ScriptableBuff healingBuff;
        public ScriptableBuff oilyBuff;
        public ScriptableBuff superchargedBuff;

        public GameObject d2dCircleExplosion;
        public GameObject d2dSquareExplosion;
        public GameObject d2dStarExplosion;
        public GameObject d2dOvalExplosion;
        public GameObject d2dSparkExplosion;
        public GameObject d2dPuzzleExplosion;
    }
}
