using _Scripts.Buffs;
using UnityEngine;
using UnityEngine.Serialization;

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

        public Sprite[] stars;

        public Sprite weaponLockedSprite;

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

        public ScriptableBuff burningBuff;
        public ScriptableBuff infectedBuff;
        public ScriptableBuff speedBuff;
        public ScriptableBuff stunnedBuff;
        public ScriptableBuff cursedBuff;
        public ScriptableBuff frozenBuff;
        public ScriptableBuff healingBuff;

    }
}
