using UnityEngine;

namespace Simulations
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float angularSpeed = 20f;
        private Transform planeTransform;
        private bool isActivated = false;
        private float currentSpeed = 0f;

        private void Update()
        {
            if (isActivated) MoveTowardsPlane();
        }

        private void MoveTowardsPlane()
        {
            Vector3 direction = (planeTransform.position - transform.position).normalized;
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            transform.SetPositionAndRotation(transform.position + currentSpeed * Time.deltaTime * transform.forward, Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * angularSpeed));
        }

        public void Activate(Transform planeTransform)
        {
            isActivated = true;
            this.planeTransform = planeTransform;
            //activate effects
        }

        private void OnTriggerEnter(Collider other)
        {
            PlaneController planeController = other.GetComponent<PlaneController>();
            if (planeController == null) return;

            Debug.Log("Trigger Enter Plane");
            //explode and destroy self, turn off effects
            //trigger plane destrauction
        }
    }
}