using UnityEngine;

namespace DayNightSystem.Scripts
{
    [CreateAssetMenu(fileName = "Lighting Preset", menuName = "Lighting Preset")]
    public class LightingPreset : ScriptableObject
    {
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public Gradient FogColor;
        public AnimationCurve AmbientIntensityMultiplierDay;
        public AnimationCurve ReflectionIntensityMultiplierDay;
        public AnimationCurve SunIntensity;
        public AnimationCurve FogNightTransition;
    }
}