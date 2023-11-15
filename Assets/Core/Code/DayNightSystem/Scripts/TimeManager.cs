using System;
using System.Linq;
using UnityEngine;

namespace DayNightSystem.Scripts
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;

        [SerializeField] private Light directionalLight;
        [SerializeField] private LightingPreset lightingPreset;
        [SerializeField] private float dayLength = 60f;
        [SerializeField] private float nightLength = 20f;
        [SerializeField] private int currentHours = 0;
        [SerializeField] private int currentMinutes = -1;
        [SerializeField] private PartOfDay startDayPeriod = PartOfDay.Dawn;
        private PartOfDay previousPartOfDay;
        private PartOfDay partOfDay;
        private float currentRawTime = 0f;
        private float dayTime = 0f;
        private float nightTime = 0f;
        private int lastFrameMinutes;

        public bool IsDay => currentRawTime >= (int)PartOfDay.Dawn && currentRawTime < (int)PartOfDay.Night;
        public bool IsNight => !IsDay;
        public PartOfDay CurrentPartOfDay => partOfDay;
        public float CurrentHours => currentHours;
        public float CurrentMinutes => currentMinutes;
        public LightingPreset LightingSettings => lightingPreset;

        public event Action<int, int> OnTimeChanged;
        public event Action<PartOfDay> OnPartOfDayChanged;
        public event Action OnDayChanged;

        private void Awake()
        {
            Instance = this;

            if (!directionalLight)
            {
                directionalLight = GameObject.Find("Directional Light")?.GetComponent<Light>();
                if (!directionalLight) Debug.LogWarning("No Directional Light found in scene.");
            }
        }

        private void Start()
        {
            SetTimeOfDay(startDayPeriod);
        }

        private void Update()
        {
            HandleTime();
        }

        private void HandleTime()
        {
            if (lightingPreset == null) return;

            FindTimeOfDay();
            CalculateTimestamp();
            FindPartOfDay();
            UpdateLighting();
        }

        private void FindTimeOfDay()
        {
            if (currentRawTime < (int)PartOfDay.Dawn || currentRawTime > (int)PartOfDay.Night)
            {
                nightTime += Time.deltaTime;
                currentRawTime = (int)PartOfDay.Night + nightTime / nightLength * (24 - (int)PartOfDay.Night + (int)PartOfDay.Dawn);
                currentRawTime %= 24;
                return;
            }

            dayTime += Time.deltaTime;
            currentRawTime = (int)PartOfDay.Dawn + dayTime / dayLength * ((int)PartOfDay.Night - (int)PartOfDay.Dawn);
            currentRawTime %= 24;

            if (dayTime > dayLength || nightTime > nightLength)
                dayTime = nightTime = 0f;
        }

        private void UpdateLighting()
        {
            float dayProgressFullDay = GetDayProgress();
            float dayProgressDaytimeOnly = Mathf.Clamp(dayProgressFullDay, 0, 1);
            RenderSettings.ambientLight = lightingPreset.AmbientColor.Evaluate(dayProgressDaytimeOnly);
            RenderSettings.fogColor = lightingPreset.FogColor.Evaluate(dayProgressDaytimeOnly);
            if (directionalLight == null) return;

            directionalLight.color = lightingPreset.DirectionalColor.Evaluate(dayProgressDaytimeOnly);
            directionalLight.intensity = lightingPreset.SunIntensity.Evaluate(dayProgressDaytimeOnly);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((dayProgressFullDay * 180), 170f, 0f));
            RenderSettings.ambientIntensity = lightingPreset.AmbientIntensityMultiplierDay.Evaluate(dayProgressDaytimeOnly);
            RenderSettings.reflectionIntensity = lightingPreset.ReflectionIntensityMultiplierDay.Evaluate(dayProgressDaytimeOnly);
        }

        public float GetDayProgress()
        {
            float adjustedDawn = Mathf.Clamp((int)PartOfDay.Dawn, 0, 24);
            float adjustedNight = Mathf.Clamp((int)PartOfDay.Night, 0, 24);

            float dayDuration = adjustedNight - adjustedDawn;
            float nightDuration = 24 - dayDuration;

            if (IsDay)
            {
                return (currentRawTime - adjustedDawn) / dayDuration;
            }

            float nightTime = currentRawTime < adjustedDawn ? currentRawTime + 24 - adjustedNight : currentRawTime - adjustedNight;
            return 1 + nightTime / nightDuration;
        }

        private void CalculateTimestamp()
        {
            lastFrameMinutes = currentMinutes;
            currentHours = (int)currentRawTime;
            currentMinutes = (int)((currentRawTime - currentHours) * 60);

            if (lastFrameMinutes != currentMinutes) OnTimeChanged?.Invoke(currentHours, currentMinutes);
        }

        private void FindPartOfDay()
        {
            partOfDay = default;
            // Find the first PartOfDay value with a start time less than or equal to the current hour
            foreach (var part in Enum.GetValues(typeof(PartOfDay)).Cast<PartOfDay>())
            {
                if (currentHours >= (int)part) partOfDay = part;
            }

            // If no matching PartOfDay value is found, default to Night
            if (partOfDay == default)
                partOfDay = PartOfDay.Night;

            if (partOfDay != previousPartOfDay)
                OnPartOfDayChanged?.Invoke(partOfDay);

            previousPartOfDay = partOfDay;
        }

        public void SetTimeOfDay(PartOfDay partOfDay)
        {
            SetTimeOfDay((int)partOfDay);
        }

        public void SetTimeOfDay(float hours = 0, float minutes = 0)
        {
            NormalizeTime(ref hours, ref minutes);
            dayTime = nightTime = 0f;
            float timeToSet = (hours + minutes / 60f) % 24;

            if (timeToSet < (int)PartOfDay.Dawn || timeToSet > (int)PartOfDay.Night)
            {
                if (timeToSet > (int)PartOfDay.Night)
                {
                    nightTime = nightLength * (timeToSet - (int)PartOfDay.Night) / (24 - (int)PartOfDay.Night + (int)PartOfDay.Dawn);
                }
                else
                {
                    nightTime = nightLength * (timeToSet + (24 - (int)PartOfDay.Night)) / (24 - (int)PartOfDay.Night + (int)PartOfDay.Dawn);
                }
                currentRawTime = timeToSet;
                return;
            }

            dayTime = dayLength * (timeToSet - (int)PartOfDay.Dawn) / ((int)PartOfDay.Night - (int)PartOfDay.Dawn);
            currentRawTime = timeToSet;
        }

        private void NormalizeTime(ref float hours, ref float minutes)
        {
            hours = (hours + Mathf.Floor(minutes / 60f)) % 24f;
            minutes %= 60f;

            if (hours < 0f) hours += 24f;
            if (minutes < 0f) minutes += 60f;
        }
    }
}