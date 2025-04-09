using UnityEngine;
using UnityEditor;
using Cinemachine;

public class SaveVCamConfig
{
    [MenuItem("Tools/CameraConfig/Save Virtual Camera Config")]
    public static void SaveConfig()
    {
        // 检查当前选中的对象
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择一个包含 CinemachineVirtualCamera 组件的 GameObject", "确定");
            return;
        }

        // 尝试获取 CinemachineVirtualCamera 组件
        CinemachineVirtualCamera vcam = Selection.activeGameObject.GetComponent<CinemachineVirtualCamera>();
        if (vcam == null)
        {
            EditorUtility.DisplayDialog("错误", "选中的对象没有 CinemachineVirtualCamera 组件", "确定");
            return;
        }

        // 创建一个新的 VirtualCameraConfig 实例
        VirtualCameraConfig config = ScriptableObject.CreateInstance<VirtualCameraConfig>();

        // 从虚拟摄像机读取 Lens 参数（FOV）
        config.fov = vcam.m_Lens.FieldOfView;

        // 获取 Transposer 组件，读取跟随偏移和 Damping
        CinemachineTransposer transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            config.followOffset = transposer.m_FollowOffset;
            config.xDamping = transposer.m_XDamping;
            config.yDamping = transposer.m_YDamping;
            config.zDamping = transposer.m_ZDamping;
            config.yawDamping = transposer.m_YawDamping;
            config.pitchDamping = transposer.m_PitchDamping;
            config.rollDamping = transposer.m_RollDamping;
        }

        // 获取 Composer 组件，读取 Aim 相关参数
        CinemachineComposer composer = vcam.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            config.trackedObjectOffset = composer.m_TrackedObjectOffset;
            config.lookaheadTime = composer.m_LookaheadTime;
            config.lookaheadSmoothing = composer.m_LookaheadSmoothing;
            config.lookaheadIgnoreY = composer.m_LookaheadIgnoreY;
            config.horizontalDamping = composer.m_HorizontalDamping;
            config.verticalDamping = composer.m_VerticalDamping;
            config.screenX = composer.m_ScreenX;
            config.screenY = composer.m_ScreenY;
            config.deadZoneWidth = composer.m_DeadZoneWidth;
            config.deadZoneHeight = composer.m_DeadZoneHeight;
            config.softZoneWidth = composer.m_SoftZoneWidth;
            config.softZoneHeight = composer.m_SoftZoneHeight;
            config.biasX = composer.m_BiasX;
            config.biasY = composer.m_BiasY;
            config.centerOnActive = composer.m_CenterOnActivate;
        }

        // 获取 Noise 组件（如果存在）
        CinemachineBasicMultiChannelPerlin noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise != null)
        {
            // pivotOffset 可能需要你自行决定如何读取，这里简单示例设置为 Vector3.zero
            config.pivotOffset = Vector3.zero;
            config.amplitudeGain = noise.m_AmplitudeGain;
            config.frequencyGain = noise.m_FrequencyGain;
        }

        // 弹出保存对话框，让用户指定保存路径和文件名
        string path = EditorUtility.SaveFilePanelInProject("Save virtual camera configuration", vcam.name + "_Config.asset", "asset", "Please enter file save address");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Success", "Virtual Camera configuration has been saved！", "ok");
        }
    }
}
