using UnityEngine;
using System.Collections;
using UnityEditor;


namespace Expect.Vibration
{
    [CustomEditor(typeof(VibrationSampleCode))]
    public class VibrationSampleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            VibrationSampleCode myScript = (VibrationSampleCode)target;
            if (GUILayout.Button("Test Full Hand Vibrate"))
            {
                myScript.TestFullVibration();
            }

            if (GUILayout.Button("Test Finger Vibrate"))
            {
                myScript.TestFingerVibration();
            }

        }
    }
}