using UnityEngine;

namespace _Scripts.GameEngine.Map
{
    public class Parallax : MonoBehaviour
    {
        private float length, startPos;
        public GameObject cam;
        public float parallaxEffect;

        // Start is called before the first frame update
        void Start()
        {
            startPos = transform.position.x;
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                length = GetComponent<SpriteRenderer>().bounds.size.x / 3;
            }
            else
            {
                RectTransform rt = (RectTransform)gameObject.transform;
                length = rt.rect.width / 3;
            }
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += new Vector3(-Time.deltaTime * parallaxEffect + ((transform.position.x < startPos - length) ? length  * 3 /* Full Length */: 0), 0, 0);
        }
    }
}
