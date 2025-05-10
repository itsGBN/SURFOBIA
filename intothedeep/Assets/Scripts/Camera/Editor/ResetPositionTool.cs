using UnityEditor;
using UnityEngine;

public class ResetPositionTool : EditorWindow
{
    private GameObject player;
    private Vector3   targetPosition;
    private float     idleFloatValue;

    [MenuItem("Tools/Reset Player Position & IdleFloat")]
    public static void ShowWindow()
    {
        GetWindow<ResetPositionTool>("Reset Player");
    }

    void OnGUI()
    {
        GUILayout.Label("Player Reset Tool", EditorStyles.boldLabel);
        
        // 选择玩家对象
        player = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Player GameObject", "Drag the player object here"),
            player,
            typeof(GameObject),
            true
        );

        GUILayout.Space(5);
        // 目标位置字段
        targetPosition = EditorGUILayout.Vector3Field(
            new GUIContent("Target Position", "Position to move the player to"),
            targetPosition
        );

        GUILayout.Space(5);
        // Idle Float 字段
        idleFloatValue = EditorGUILayout.FloatField(
            new GUIContent("Idle Float", "Value to assign to PlayerController.idleFloat"),
            idleFloatValue
        );

        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(player == null);
        // 重置位置按钮
        if (GUILayout.Button("Reset Position"))
        {
            Undo.RecordObject(player.transform, "Reset Player Position");
            player.transform.position = targetPosition;
        }

        GUILayout.Space(5);
        // 重置 idleFloat 按钮
        if (GUILayout.Button("Reset Idle Float"))
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                Undo.RecordObject(pc, "Reset idleFloat");
                pc.idleFloat = idleFloatValue;
                EditorUtility.SetDirty(pc);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Selected object has no PlayerController component.", "OK");
            }
        }
        EditorGUI.EndDisabledGroup();
    }
}
