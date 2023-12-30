using UnityEngine;

namespace Utilities
{
    public class SelfDestroyer : MonoBehaviour
    {
        [SerializeField] private float delay = 10f;

        private void OnEnable()
        {
            Destroy(gameObject, delay);
        }
    }
}