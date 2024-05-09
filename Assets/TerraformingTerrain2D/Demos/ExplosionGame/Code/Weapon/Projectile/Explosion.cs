using System.Collections;
using UnityEngine;

namespace ExplosionGame
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _effect;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _destroy = true;

        public void Play()
        {
            _effect.Play();
            _audioSource.Play();

            StartCoroutine(PlayAnimation());
        }

        private IEnumerator PlayAnimation()
        {
            _effect.Play();
            _audioSource.Play();

            if (_destroy)
            {
                yield return new WaitWhile(() => _effect.isPlaying);

                Destroy(gameObject);
            }
        }
    }
}