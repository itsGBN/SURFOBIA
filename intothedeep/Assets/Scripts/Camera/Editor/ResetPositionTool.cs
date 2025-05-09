using UnityEditor;
using UnityEngine;

public class ResetPositionTool : EditorWindow
{
    private GameObject player;
    private Vector3 targetPosition;

    [MenuItem("Tools/Reset Player Position")]
    public static void ShowWindow()
    {
        GetWindow<ResetPositionTool>("Reset Position");
    }

    void OnGUI()
    {
        GUILayout.Label("Player Teleport Tool", EditorStyles.boldLabel);
        
        // 选择玩家对象
        player = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Player GameObject", "drag the player object to here"),
            player,
            typeof(GameObject),
            true
        );

        // 设置目标位置
        targetPosition = EditorGUILayout.Vector3Field(
            new GUIContent("Target Position", "player will be moved to here"),
            targetPosition
        );

        GUILayout.Space(10);

        // 重置按钮
        if (GUILayout.Button("Reset Position"))
        {
            if (player != null)
            {
                Undo.RecordObject(player.transform, "Reset Player Position");
                player.transform.position = targetPosition;
                // 如果你想也重置旋转，可加：
                // player.transform.rotation = Quaternion.identity;
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    "Please give me a player object",
                    "OK"
                );
            }
        }
    }
}