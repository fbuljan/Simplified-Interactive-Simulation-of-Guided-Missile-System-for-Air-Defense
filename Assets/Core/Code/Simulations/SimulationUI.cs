using TMPro;
using UnityEngine;

namespace Simulations
{
    public class SimulationUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text inputInstructions;
        [SerializeField] private SimulationController simulationController;

        private void Start()
        {
            GenerateInputInstructions();
        }

        private void GenerateInputInstructions()
        {
            inputInstructions.text = $"Start simulation: {simulationController.StartSimulationButton}.\n" +
                $"End simulation: {simulationController.EndSimulationButton}.";
        }
    }
}