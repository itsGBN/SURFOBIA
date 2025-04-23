using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FreeRoamOnly : MonoBehaviour
{
    public PlayerController player;
    public CinemachineVirtualCamera vcam;
    public float speedOffset;
    public float fastFrequency;
    public float slowFrequency;
    public float smoothTime = 0.3f;      // “追踪”所需的时间，越小越快
    private float freqVelocity = 0f;     // 内部用的速度缓存

    private CinemachineBasicMultiChannelPerlin noise;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        noise=vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || noise == null) return;

        float targetFreq = (player.GetCurrentSpeed() + speedOffset >= player.moveSpeed)
            ? fastFrequency
            : slowFrequency;

        // 三参数平滑阻尼（带缓冲效果）
        noise.m_FrequencyGain = Mathf.SmoothDamp(
            noise.m_FrequencyGain, 
            targetFreq, 
            ref freqVelocity, 
            smoothTime
        );
    }
}
