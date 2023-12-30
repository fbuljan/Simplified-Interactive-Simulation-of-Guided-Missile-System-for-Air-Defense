using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;

namespace Simulations
{
    public class SimulationUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text inputInstructions;
        [SerializeField] private SimulationController simulationController;
        [SerializeField] private TMP_Text notification;
        private Color originalColor = Color.black;

        private void OnEnable()
        {
            simulationController.OnSimulationMilestone += OnSimulationMilestone;
        }

        private void OnDisable()
        {
            simulationController.OnSimulationMilestone -= OnSimulationMilestone;
        }

        private void OnSimulationMilestone(bool start)
        {
            string text = start ? "Simulation started!" : "Simulation ended!";
            notification.text = text;
            notification.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(FadeOutText(3f));
        }

        private void Start()
        {
            notification.gameObject.SetActive(false);
        }

        private void Update()
        {
            GenerateInputInstructions();
        }

        private void GenerateInputInstructions()
        {
            if (simulationController.SimulationRunning)
            {
                inputInstructions.text = $"End simulation: {simulationController.EndSimulationButton}.\n" +
                    $"Change camera mode: {simulationController.CameraButton}.";
                return;
            }

            inputInstructions.text = $"Start simulation: {simulationController.StartSimulationButton}.\n";
            if (simulationController.ReplayAvailable) inputInstructions.text += $"Replay last simulation: {simulationController.ReplayButton}.";
        }

        private IEnumerator FadeOutText(float fadeOutTime)
        {
            notification.color = originalColor;
            float startAlpha = originalColor.a;

            for (float t = 0; t < 1; t += Time.deltaTime / fadeOutTime)
            {
                if (notification != null)
                {
                    float newAlpha = Mathf.Lerp(startAlpha, 0, t);
                    notification.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
                    yield return null;
                }
            }

            notification.gameObject.SetActive(false);
            notification.color = originalColor;
        }
    }
}