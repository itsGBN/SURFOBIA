using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using System.Collections;
using Cinemachine.PostFX;

public class ClipRecordHelper : MonoBehaviour
{
    [Tooltip("拖你的 Cinemachine Virtual Camera 上的 CinemachineVolumeSettings 组件")]
    public CinemachineVolumeSettings cineVolume;

    [Tooltip("信号触发时要切到的对焦距离")]
    public float targetFocusDistance = 1f;

    [Tooltip("恢复原始焦距时的时长")]
    public float resetDuration = 0.5f;

    private DepthOfField _dof;
    private float         _originalFocusDistance;
    private Coroutine     _running;

    public PlayerController player;

    void Awake()
    {
        if (cineVolume == null)
            cineVolume = GetComponent<CinemachineVolumeSettings>();
    }

    void Start()
    {
        // 克隆 Profile
        if (cineVolume.m_Profile != null)
            cineVolume.m_Profile = Instantiate(cineVolume.m_Profile);

        if (cineVolume.m_Profile.TryGet<DepthOfField>(out _dof))
            _originalFocusDistance = _dof.focusDistance.value;
        else
            Debug.LogWarning("没找到 DepthOfField override");
    }

    public void resumeTime()
    {
        Time.timeScale = 1f;
    }

    // Signal Emitter 触发
    public void ReduceFocus()
    {
        
        Time.timeScale = 0.02f;
        player.idleFloat = 20f;

    }



}
