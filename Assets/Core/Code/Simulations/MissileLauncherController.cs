using System.Collections.Generic;
using UnityEngine;

namespace Simulations
{
    public class MissileLauncherController : MonoBehaviour
    {
        [SerializeField] private List<Missile> missiles;
        [SerializeField] private float activationRange = 50f;
        private Transform planeTransform;
        private int activateMissleIndex = 0;

        public void Init(Transform planeTransform)
        {
            this.planeTransform = planeTransform;
        }

        private void Update()
        {
            if (!planeTransform) return;

            float distanceToPlane = Vector3.Distance(transform.position, planeTransform.position);
            if (distanceToPlane <= activationRange) ActivateNextMissile();
        }

        private void ActivateNextMissile()
        {
            missiles[activateMissleIndex].Activate(planeTransform);
            activateMissleIndex = (activateMissleIndex + 1) % missiles.Count;
            planeTransform = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, activationRange);
        }
    }
}