using UnityEngine;

namespace Simulations
{
    public class MissileController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlaneController>() == null) return;

            print("Trigger Enter Plane");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlaneController>() == null) return;

            print("Trigger Exit  Plane");
        }
    }
}