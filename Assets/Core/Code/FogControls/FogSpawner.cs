using UnityEngine;
using AtmosphericHeightFog;
using DayNightSystem.Scripts;

namespace FogControls.Scripts
{
    public class FogSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject fogPrefab;
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private Material fogMaterialCurrent;
        [SerializeField] private Material fogMaterialDaytime;
        [SerializeField] private Material fogMaterialNightTime;
        private HeightFogGlobal fog;

        void Update()
        {
            if (timeManager == null && TimeManager.Instance != null) timeManager = TimeManager.Instance;
            if (Camera.main == null || timeManager == null) return;

            GameObject fogObject = Instantiate(fogPrefab, transform);
            if(fogObject.TryGetComponent(out fog))
            {
                FogController fogController = gameObject.AddComponent<FogController>();
                fogController.Fog = fog;
                fogController.TimeManager = TimeManager.Instance;
                fogController.FogMaterialCurrent = fogMaterialCurrent;
                fogController.FogMaterialDaytime = fogMaterialDaytime;
                fogController.FogMaterialNightTime = fogMaterialNightTime;
                fogController.LightingPreset = TimeManager.Instance.LightingSettings;
            }
            Destroy(this);
        }
    }
}
