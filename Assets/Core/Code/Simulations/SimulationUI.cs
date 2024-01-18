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
        [SerializeField] private GameObject aim;
        private MissileLauncherController missileLauncherController;
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
            missileLauncherController = start ? FindObjectOfType<MissileLauncherController>() : null;
            StopAllCoroutines();
            StartCoroutine(FadeOutText(3f));
        }

        private void Start()
        {
            notification.gameObject.SetActive(false);
            aim.SetActive(false);
        }

        private void Update()
        {
            if (missileLauncherController != null) aim.SetActive(missileLauncherController.IsManuallyControlled && 
                !missileLauncherController.ActiveMissile.IsActivated);
            GenerateInputInstructions();
        }

        private void GenerateInputInstructions()
        {
            if (simulationController.SimulationRunning)
            {
                inputInstructions.text = $"End simulation: {simulationController.EndSimulationButton}.\n" +
                    $"\nChange camera mode: {simulationController.CameraButton}.\n";
                if (missileLauncherController != null)
                {
                    inputInstructions.text += $"\nMissle manually controlled: {missileLauncherController.IsManuallyControlled}.\n " +
                        $"Control with: {missileLauncherController.ActivateManualControl}.";
                    if (missileLauncherController.IsManuallyControlled) inputInstructions.text += "\nControl with arrows.";
                }

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