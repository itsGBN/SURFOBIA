using UnityEngine;
using UnityEditor;

/// <summary>
/// A simple Unity Editor window to control Time.timeScale during Play Mode.
/// Place this script in an "Editor" folder in your project.
/// </summary>
public class TimeScaleController : EditorWindow
{
    private float timeScale = 1f;

    [MenuItem("Tools/Time Scale Controller")]
    public static void ShowWindow()
    {
        GetWindow<TimeScaleController>("Time Scale");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Time Scale Controller", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Adjust the slider or presets during Play Mode to control Time.timeScale", MessageType.Info);

        // Slider for custom time scale (0 = paused, 1 = normal speed)
        timeScale = EditorGUILayout.Slider("Time Scale", timeScale, 0f, 2f);

        // Preset buttons for common slow-motion speeds
        GUILayout.Label("Presets:", EditorStyles.label);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("0.5x")) SetTimeScale(0.5f);
        if (GUILayout.Button("0.25x")) SetTimeScale(0.25f);
        if (GUILayout.Button("0.1x")) SetTimeScale(0.1f);
        if (GUILayout.Button("Pause")) SetTimeScale(0f);
        GUILayout.EndHorizontal();

        // Apply and reset buttons
        GUILayout.Space(5);
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Apply"))
            {
                SetTimeScale(timeScale);
            }
            if (GUILayout.Button("Reset to 1x"))
            {
                SetTimeScale(1f);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play Mode to adjust time scale", MessageType.Warning);
        }

        // Display current Time.timeScale
        GUILayout.Space(10);
        GUILayout.Label($"Current Time.timeScale: {(Application.isPlaying ? Time.timeScale.ToString("F2") : "N/A")}");
    }

    private void OnInspectorUpdate()
    {
        // Repaint to update current timeScale display in real-time
        Repaint();
    }

    /// <summary>
    /// Sets both the local slider value and Unity's Time.timeScale.
    /// </summary>
    private void SetTimeScale(float scale)
    {
        timeScale = scale;
        Time.timeScale = scale;
    }
}
