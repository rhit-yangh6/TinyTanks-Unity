using UnityEngine;
using UnityEngine.UI;

namespace ExplosionGame
{
    public class ClusterGrenadeUI : MonoBehaviour
    {
        [SerializeField] private Text _text;

        public void Unpin()
        {
            transform.SetParent(null);
            transform.rotation = Quaternion.identity;
        }

        public void Move(Vector3 grenadePosition)
        {
            transform.position = grenadePosition + Vector3.up / 3f;
        }

        public void UpdateValue(int value)
        {
            _text.text = value.ToString();
        }
    }
}