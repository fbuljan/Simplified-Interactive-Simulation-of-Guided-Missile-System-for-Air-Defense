using TMPro;
using UnityEngine;

namespace DayNightSystem.Scripts
{
    public class TimeDisplay : MonoBehaviour
    {
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private TMP_Text timeUI;

        private void OnEnable()
        {
            timeManager.OnTimeChanged += Display;
        }

        private void OnDisable()
        {
            timeManager.OnTimeChanged -= Display;
        }

        private void Display(int currentHours, int currentMinutes)
        {
            string zero = "";
            if (currentMinutes < 10) zero = "0";
            timeUI.text = $"{currentHours}:{zero}{currentMinutes}";
        }
    }
}