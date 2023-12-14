using UnityEngine;

namespace Simulations
{
    public class SimulationController : MonoBehaviour
    {
        [SerializeField] private KeyCode startSimulationButton = KeyCode.Space;
        [SerializeField] private KeyCode endSimulationButton = KeyCode.Backspace;

        public KeyCode EndSimulationButton => endSimulationButton;
        public KeyCode StartSimulationButton => startSimulationButton;

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Input.GetKeyDown(startSimulationButton)) Debug.Log("Simulation started.");
            if (Input.GetKeyDown(endSimulationButton)) Debug.Log("Simulation ended.");
        }
    }
}