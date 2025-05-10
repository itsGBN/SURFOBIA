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
        
        Time.timeScale = 0.2f;
        player.idleFloat = 10f;
        // 停掉上一次、再来一次
        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(LerpFocus(_dof.focusDistance.value, targetFocusDistance, resetDuration));
    }

    // 可选重置
    public void ResetFocus()
    {
        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(LerpFocus(_dof.focusDistance.value, _originalFocusDistance, resetDuration));
    }

    // 核心：在 duration 秒内插值 from→to
    private IEnumerator LerpFocus(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // 你可以改 Ease（比如 t = t*t）
            float cur = Mathf.Lerp(from, to, t);
            _dof.focusDistance.value = cur;
            yield return null;
        }
        _dof.focusDistance.value = to;
        _running = null;
    }
}
