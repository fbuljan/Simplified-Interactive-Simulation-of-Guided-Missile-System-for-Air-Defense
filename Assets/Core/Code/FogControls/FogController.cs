using UnityEngine;
using AtmosphericHeightFog;
using DayNightSystem.Scripts;

namespace FogControls.Scripts
{
    public class FogController : MonoBehaviour
    {
        public TimeManager TimeManager { get; set; }
        public HeightFogGlobal Fog { get; set; }
        public Material FogMaterialCurrent { get; set; }
        public Material FogMaterialDaytime { get; set; }
        public Material FogMaterialNightTime { get; set; }
        public LightingPreset LightingPreset { get; set; }

        private void Start()
        {
            Fog.presetMaterial = FogMaterialCurrent;
        }

        void Update()
        {
            SetupFog();
        }

        private void SetupFog()
        {
            FogMaterialCurrent.Lerp(FogMaterialDaytime, FogMaterialNightTime, LightingPreset.FogNightTransition.Evaluate(TimeManager.GetDayProgress()*0.5f));
        }
    }
}
