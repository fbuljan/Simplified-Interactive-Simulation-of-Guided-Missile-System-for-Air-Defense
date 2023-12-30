using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Simulations
{
    public class PlaneController : MonoBehaviour
    {
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float maxHeight = 100f;
        [SerializeField] private LayerMask terrainLayer;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject cameraRoot;
        [SerializeField] private List<GameObject> smokes;
        private Vector3 targetPosition;
        private Vector3 hitPosition;
        private Quaternion targetFallRotation;
        private bool isHit = false;
        private float currentSpeed = 0f;

        public event Action OnPlaneCrashed;

        public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
        public GameObject CameraRoot => cameraRoot;
        public bool IsHit => isHit;
        public Vector3 HitPosition => hitPosition;

        private void Update()
        {
            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            if (isHit)
            {
                FallDown();
                return;
            }

            transform.LookAt(targetPosition);
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

            float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
            if (transform.position.y < terrainHeight + maxHeight)
                transform.position = new Vector3(transform.position.x, terrainHeight + maxHeight, transform.position.z);

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f, terrainLayer))
                targetPosition = new Vector3(targetPosition.x, hit.point.y + maxHeight, targetPosition.z);
        }

        private void FallDown()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetFallRotation, 0.22f * Time.deltaTime);
            smokes.ForEach(smoke => smoke.transform.localRotation = Quaternion.Inverse(transform.rotation) * Quaternion.Euler(-90, 0, 0));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 50f);
        }

        public void OnHit()
        {
            isHit = true;
            hitPosition = transform.position;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddForce(Vector3.down * 2500f, ForceMode.Acceleration);
            rb.AddForce(transform.forward * 3000f, ForceMode.Acceleration);
            targetFallRotation = Quaternion.Euler(30f, Random.Range(20, 120f), Random.Range(20f, 120f));
            smokes.ForEach(smoke => smoke.SetActive(true));
            if (cameraRoot.transform.childCount > 0) cameraRoot.transform.GetChild(0).parent = null;
            InvokeRepeating(nameof(SpawnMoreSmoke), 0.5f, 0.1f);
        }

        private void SpawnMoreSmoke()
        {
            GameObject smoke = Instantiate(smokes[0], transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(smoke, 7f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Terrain _)) return;

            OnPlaneCrashed?.Invoke();
            Instantiate(explosion, transform.position, Quaternion.identity);
            Camera mainCamera = Camera.main;
            mainCamera.transform.eulerAngles = new Vector3(45, 0, 0);
            mainCamera.transform.position = new Vector3(1500, 2000, -500);
            Destroy(gameObject);
        }
    }
}