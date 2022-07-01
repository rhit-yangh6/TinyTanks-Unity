using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class DamagePopup : MonoBehaviour
    {
        public static DamagePopup Create(Vector2 position, int amount, bool isCriticalHit)
        {
            Transform damagePopupTransform = Instantiate(GameAssets.i.damagePopupPrefab, position, Quaternion.identity);
            
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(amount, isCriticalHit);
            
            return damagePopup;
        }

        private TextMeshPro textMesh;
        private float disappearTimer;
        private Color textColor;

        private void Awake()
        {
            textMesh = transform.GetComponent<TextMeshPro>();
        }
    
        public void Setup(int damageAmount, bool isCriticalHit)
        {
            textMesh.SetText("-" + damageAmount);
            if (!isCriticalHit)
            {
                textMesh.fontSize = 36;
                textColor = Color.yellow;
            }
            else
            {
                textMesh.fontSize = 45;
                textColor = Color.red;
            }

            textMesh.color = textColor;
            disappearTimer = 1f;
        }

        private void Update()
        {
            float moveYSpeed = 8f;
            transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

            disappearTimer -= Time.deltaTime;
            if (disappearTimer < 0)
            {
                float disappearSpeed = 3f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
