using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class CameraShake : MonoBehaviour
    {
        private const float delay = 0.15f;

        public void ShakeCamera(float duration, float magnitude)
        {
            StopAllCoroutines();
            StartCoroutine(Shake(duration, magnitude));
        }

        private IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 originalPos = transform.localPosition;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                float z = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = originalPos + new Vector3(x, y, z);

                yield return new WaitForSeconds(delay);
                elapsed += delay;
            }

            transform.localPosition = originalPos;
        }
    }
}