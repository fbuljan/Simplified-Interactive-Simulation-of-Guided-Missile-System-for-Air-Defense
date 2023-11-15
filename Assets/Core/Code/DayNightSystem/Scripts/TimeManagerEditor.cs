#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DayNightSystem.Scripts
{
    [CustomEditor(typeof(TimeManager))]
    public class TimeManagerEditor : Editor
    {
        float hours = 0f;
        float minutes = 0f;
        PartOfDay partOfDay = PartOfDay.Dawn;
        TimeOfDaySetup timeOfDaySetup = TimeOfDaySetup.HoursAndMinutes;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space(20);
            timeOfDaySetup = (TimeOfDaySetup)EditorGUILayout.EnumPopup("Time of day setup", timeOfDaySetup);

            if (timeOfDaySetup == TimeOfDaySetup.PartOfDay)
            {
                partOfDay = (PartOfDay)EditorGUILayout.EnumPopup("Part of day", partOfDay);
            }
            else
            {
                hours = EditorGUILayout.FloatField(hours);
                minutes = EditorGUILayout.FloatField(minutes);
            }

            if (GUILayout.Button("Set Time"))
            {
                TimeManager myTarget = (TimeManager)target;
                if (timeOfDaySetup == TimeOfDaySetup.PartOfDay)
                    myTarget.SetTimeOfDay(partOfDay);
                else
                    myTarget.SetTimeOfDay(hours, minutes);
            }
        }

        private enum TimeOfDaySetup
        {
            PartOfDay, HoursAndMinutes
        }
    }
}
#endif