using UnityEngine;
using Utilities;

namespace Simulations
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float angularSpeed = 20f;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject smoke;
        [SerializeField] private GameObject fire;
        [SerializeField] private SoundPlayer startFlyingSoundPlayer;
        private Transform planeTransform;
        private bool isActivated = false;
        private float currentSpeed = 0f;
        private float currentAngularSpeed = 0f;

        private void Start()
        {
            fire.SetActive(false);
            smoke.SetActive(false);
        }

        private void Update()
        {
            MoveTowardsPlane();
        }

        private void MoveTowardsPlane()
        {
            if (!isActivated) return;

            Vector3 direction = (planeTransform.position - transform.position).normalized;
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            transform.SetPositionAndRotation(transform.position + currentSpeed * Time.deltaTime * transform.forward, Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * currentAngularSpeed));
        }

        public void Activate(Transform planeTransform)
        {
            isActivated = true;
            this.planeTransform = planeTransform;
            Invoke(nameof(SetAngularSpeed), 5f);
            ActivateEffects();
        }

        private void SetAngularSpeed()
        {
            currentAngularSpeed = angularSpeed;
        }

        private void ActivateEffects()
        {
            startFlyingSoundPlayer.PlaySound();
            fire.SetActive(true);
            Invoke(nameof(ActivateSmoke), 2f);
        }

        private void ActivateSmoke()
        {
            smoke.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<PlaneController>(out var planeController)) return;

            Instantiate(explosion, planeController.transform.position, Quaternion.identity);
            planeController.OnHit();
            Destroy(gameObject);
        }
    }
}