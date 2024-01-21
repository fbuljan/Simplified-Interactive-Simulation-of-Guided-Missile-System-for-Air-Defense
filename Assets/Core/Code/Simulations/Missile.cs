using System;
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
        [SerializeField] private GameObject cameraRoot;
        private Transform planeTransform = null;
        private Transform planeTransformCache;
        private Vector3 manualDirection;
        private bool isActivated = false;
        private bool cameraPositionBool = false;
        private float currentSpeed = 0f;
        private float currentAngularSpeed = 0f;

        public GameObject CameraRoot => cameraRoot;
        public bool IsActivated => isActivated;

        public event Action OnPlaneTransformAttached;

        private void Start()
        {
            fire.SetActive(false);
            smoke.SetActive(false);
        }

        private void Update()
        {
            MoveTowardsPlane();

            if (cameraPositionBool)
            {
                cameraRoot.transform.localPosition = new Vector3(0, 65, -65);
                cameraRoot.transform.localEulerAngles = new Vector3(45, 0, 0);
                return;
            }

            cameraRoot.transform.localPosition = new Vector3(-5, 150, 120);
            cameraRoot.transform.localEulerAngles = new Vector3(110, 0, 0);
        }

        private void MoveTowardsPlane()
        {
            if (!isActivated) return;

            Vector3 automatedDirection = planeTransform == null ? Vector3.zero : (planeTransform.position - transform.position).normalized;
            Vector3 direction = planeTransform == null ? manualDirection : automatedDirection;
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            transform.SetPositionAndRotation(transform.position + currentSpeed * Time.deltaTime * transform.forward, Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * currentAngularSpeed));
        }

        public void Activate(Transform planeTransform, Vector3 direction = default)
        {
            isActivated = true;
            float delay = direction == default ? 0f : 6.5f;
            manualDirection = direction;
            planeTransformCache = planeTransform;
            Invoke(nameof(SetPlaneTransform), delay);
            float angularSpeedDelay = direction == default ? 3.5f : 0f;
            Invoke(nameof(SetAngularSpeed), angularSpeedDelay);
            Invoke(nameof(SetCameraPosition), 3f);
            ActivateEffects();
        }

        private void SetPlaneTransform()
        {
            planeTransform = planeTransformCache;
            OnPlaneTransformAttached?.Invoke();
        }

        private void SetAngularSpeed()
        {
            currentAngularSpeed = angularSpeed;
        }

        private void SetCameraPosition()
        {
            cameraPositionBool = true;
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
            if (other.TryGetComponent<Terrain>(out _))
            {
                Explode(transform.position);
                return;
            }

            if (!other.TryGetComponent<PlaneController>(out var planeController)) return;

            planeController.OnHit();
            Explode(planeController.transform.position);
        }

        private void Explode(Vector3 explodePosition)
        {
            Instantiate(explosion, explodePosition, Quaternion.identity);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (cameraRoot.transform.childCount > 0) cameraRoot.transform.GetChild(0).parent = null;
        }
    }
}