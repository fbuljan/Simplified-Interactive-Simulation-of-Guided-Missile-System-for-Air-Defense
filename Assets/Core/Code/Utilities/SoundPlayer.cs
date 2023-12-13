using UnityEngine;
using UnityEngine.Events;

namespace Utilities
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool loop = false;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = loop;
        }

        public void PlaySound()
        {
            audioSource.Play();
        }
    }
}