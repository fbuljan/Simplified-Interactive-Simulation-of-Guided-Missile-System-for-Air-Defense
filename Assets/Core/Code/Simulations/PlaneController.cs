using UnityEngine;

namespace Simulations
{
    public class PlaneController : MonoBehaviour
    {
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float maxHeight = 100f;
        [SerializeField] private LayerMask terrainLayer;
        [SerializeField] private GameObject explosion;
        private float currentSpeed = 0f;
        private Vector3 targetPosition;
        private Quaternion targetFallRotation;
        private bool isHit = false;

        public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }

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
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 50f);
        }

        public void OnHit()
        {
            isHit = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddForce(Vector3.down * 2500f, ForceMode.Acceleration);
            targetFallRotation = Quaternion.Euler(30f, Random.Range(20, 120f), Random.Range(20f, 120f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Terrain terrain)) return;

            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}