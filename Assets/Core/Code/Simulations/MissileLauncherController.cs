using System.Collections.Generic;
using UnityEngine;

namespace Simulations
{
    public class MissileLauncherController : MonoBehaviour
    {
        [SerializeField] private List<Missile> missiles;
        [SerializeField] private float activationRange = 50f;
        [SerializeField] private KeyCode activateManualControl = KeyCode.M;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float aimingRotationSpeed = 20f;
        private Transform planeTransform;
        private int activateMissleIndex = 0;
        private bool isManuallyControlled = false;

        public Missile ActiveMissile => missiles[0];
        public KeyCode ActivateManualControl => activateManualControl;
        public Transform CameraRoot => cameraRoot;

        public bool IsManuallyControlled { get => isManuallyControlled; set => isManuallyControlled = value; }

        public void Init(Transform planeTransform)
        {
            this.planeTransform = planeTransform;
        }

        private void Update()
        {
            if (!planeTransform) return;

            if (Input.GetKeyDown(activateManualControl)) isManuallyControlled = !isManuallyControlled;

            float distanceToPlane = Vector3.Distance(transform.position, planeTransform.position);

            if (isManuallyControlled)
            {
                Aim();
                if (Input.GetMouseButtonDown(0)) ActivateMissileManualy();
                return;
            }

            if (distanceToPlane <= activationRange) ActivateNextMissile();
        }

        private void ActivateMissileManualy()
        {
            Vector3 direction = Camera.main.transform.forward;
            ActiveMissile.transform.position = CameraRoot.transform.position - (2 * direction) + (2 * Camera.main.transform.right);
            ActiveMissile.transform.forward = direction;
            ActivateNextMissile(direction);
        }

        private void Aim()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            float yaw = horizontalInput * aimingRotationSpeed * Time.deltaTime;
            float pitch = -verticalInput * aimingRotationSpeed * Time.deltaTime;

            cameraRoot.Rotate(Vector3.up, yaw, Space.World);
            cameraRoot.Rotate(Vector3.right, pitch, Space.Self);
        }

        private void ActivateNextMissile(Vector3 direction = default)
        {
            missiles[activateMissleIndex].Activate(planeTransform, direction);
            activateMissleIndex = (activateMissleIndex + 1) % missiles.Count;
            planeTransform = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, activationRange);
        }
    }
}