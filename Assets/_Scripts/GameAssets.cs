using JetBrains.Annotations;
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

        public Sprite[] stars;

    }
}
