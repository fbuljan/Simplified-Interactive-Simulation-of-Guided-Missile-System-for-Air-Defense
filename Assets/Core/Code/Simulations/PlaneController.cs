using UnityEngine;

namespace Simulations
{
    public class PlaneController : MonoBehaviour
    {
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float maxHeight = 100f;
        [SerializeField] private LayerMask terrainLayer;
        private float currentSpeed = 0f;
        private Vector3 targetPosition;

        public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }

        private void Update()
        {
            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            transform.LookAt(targetPosition);
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

            float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
            if (transform.position.y < terrainHeight + maxHeight)
                transform.position = new Vector3(transform.position.x, terrainHeight + maxHeight, transform.position.z);

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f, terrainLayer))
                targetPosition = new Vector3(targetPosition.x, hit.point.y + maxHeight, targetPosition.z);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 50f);
        }
    }
}