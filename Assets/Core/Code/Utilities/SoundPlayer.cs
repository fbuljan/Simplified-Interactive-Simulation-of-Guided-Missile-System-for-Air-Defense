using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool loop = false;
        [SerializeField] private bool playOnStart = false;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = loop;
            if (playOnStart) PlaySound();
        }

        public void PlaySound()
        {
            audioSource.Play();
        }

        public void StopSound()
        {
            audioSource.Stop();
        }
    }
}