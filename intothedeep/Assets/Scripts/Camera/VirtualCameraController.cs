using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine.Serialization;

//serializable class used to store the corresponding game state and camera configuration
//each GameStateVirtualCameraConfig class stores one group of the data
//the GameStateVirtualCameraConfig is stored in VirtualCameraMode as lists
//ex: gameState = sprint, cameraConfig = glidingSprint
[Serializable]
public class GameStateVirtualCameraConfig
{
    //waiting for change on GameState because we currently do not have it
    public VelocityState velocityState;
    public VirtualCameraConfig cameraConfig;
}
public class VirtualCameraController : MonoBehaviour, ICameraController
{
    //The camera that is controlled by this specific controller,
    //typically just drag the camera that this script attached on to it
    [Header("Connected Camera")]
    public CinemachineVirtualCamera vcam;

    private CinemachineTransposer _transposer;
    private CinemachineComposer _composer;
    private CinemachineBasicMultiChannelPerlin _perlin;
    
    //the configurations for this camera
    //ex: how it behaves when sprint, how it behaves when jump...
    [Header("Camera Configs")]
    public List<GameStateVirtualCameraConfig> configurations;
    
    //how fast the transition of camera values between each movement is
    [Header("Transition Speed")]
    public float transitionSpeed;

    public float FOVtransitionSpeed;
    public float followOffsetTransitionSpeed;
    
    //This is to save every data in the GameStateVirtualCameraConfig list
    //into a private dictionary at the awakening of the script so that
    //it responds much faster
    public Dictionary<VelocityState, VirtualCameraConfig> stateConfigs;

    private void Awake()
    {
        stateConfigs = new Dictionary<VelocityState, VirtualCameraConfig>();
        foreach (var entry in configurations)
        {
            if (!stateConfigs.ContainsKey(entry.velocityState) && entry.cameraConfig != null)
                stateConfigs.Add(entry.velocityState, entry.cameraConfig);
        }

        if (vcam != null)
        {
            _composer = vcam.GetCinemachineComponent<CinemachineComposer>();
            _transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            _perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public bool ShouldApplyState(int gameState)
    {
        throw new System.NotImplementedException();
    }

    public void ApplySettings(VelocityState velocityState, float deltaTime)
    {
        if (stateConfigs.ContainsKey(velocityState))
        {
            VirtualCameraConfig config = stateConfigs[velocityState];

            // update lens
            float t1 = Mathf.SmoothStep(0, 1, FOVtransitionSpeed * Time.deltaTime);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, config.fov, t1);

            // 更新 Body 设置（Transposer）
            
            if (_transposer != null)
            {
                float t2 = Mathf.SmoothStep(0, 1, followOffsetTransitionSpeed * deltaTime);
                _transposer.m_FollowOffset = Vector3.Lerp(_transposer.m_FollowOffset, config.followOffset, t2);
                _transposer.m_XDamping = Mathf.Lerp(_transposer.m_XDamping, config.xDamping, transitionSpeed * deltaTime);
                _transposer.m_YDamping = Mathf.Lerp(_transposer.m_YDamping, config.yDamping, transitionSpeed * deltaTime);
                _transposer.m_ZDamping = Mathf.Lerp(_transposer.m_ZDamping, config.zDamping, transitionSpeed * deltaTime);
                //_transposer.m_YawDamping = Mathf.Lerp(_transposer.m_YawDamping, config.yawDamping, transitionSpeed * deltaTime);
                //_transposer.m_RollDamping = Mathf.Lerp(_transposer.m_RollDamping, config.rollDamping, transitionSpeed * deltaTime);
                //_transposer.m_PitchDamping = Mathf.Lerp(_transposer.m_PitchDamping, config.pitchDamping, transitionSpeed * deltaTime);
            }

            // 更新 Aim 设置（Composer
            if (_composer != null)
            {
                _composer.m_TrackedObjectOffset = Vector3.Lerp(_composer.m_TrackedObjectOffset, config.trackedObjectOffset, transitionSpeed * deltaTime);
                _composer.m_LookaheadTime = Mathf.Lerp(_composer.m_LookaheadTime, config.lookaheadTime, transitionSpeed * deltaTime);
                _composer.m_LookaheadSmoothing = Mathf.Lerp(_composer.m_LookaheadSmoothing, config.lookaheadSmoothing, transitionSpeed * deltaTime);
                _composer.m_HorizontalDamping = Mathf.Lerp(_composer.m_HorizontalDamping, config.horizontalDamping, transitionSpeed * deltaTime);
                _composer.m_VerticalDamping = Mathf.Lerp(_composer.m_VerticalDamping, config.verticalDamping, transitionSpeed * deltaTime);
                //_composer.m_ScreenX = Mathf.Lerp(_composer.m_ScreenX, config.screenX, transitionSpeed * deltaTime);
                _composer.m_ScreenY = Mathf.Lerp(_composer.m_ScreenY, config.screenY, transitionSpeed * deltaTime);
                _composer.m_DeadZoneWidth = Mathf.Lerp(_composer.m_DeadZoneWidth, config.deadZoneWidth, transitionSpeed * deltaTime);
                _composer.m_DeadZoneHeight = Mathf.Lerp(_composer.m_DeadZoneHeight, config.deadZoneHeight, transitionSpeed * deltaTime);
                _composer.m_SoftZoneWidth = Mathf.Lerp(_composer.m_SoftZoneWidth, config.softZoneWidth, transitionSpeed * deltaTime);
                _composer.m_SoftZoneHeight = Mathf.Lerp(_composer.m_SoftZoneHeight, config.softZoneHeight, transitionSpeed * deltaTime);
                _composer.m_BiasX = Mathf.Lerp(_composer.m_BiasX, config.biasX, transitionSpeed * deltaTime);
                _composer.m_BiasY = Mathf.Lerp(_composer.m_BiasY, config.biasY, transitionSpeed * deltaTime);
                // 布尔值直接赋值
                _composer.m_LookaheadIgnoreY = config.lookaheadIgnoreY;
                _composer.m_CenterOnActivate = config.centerOnActive;
            }

            if (_perlin != null)
            {
                _perlin.m_AmplitudeGain = Mathf.Lerp(_perlin.m_AmplitudeGain, config.amplitudeGain, transitionSpeed * deltaTime);
                _perlin.m_FrequencyGain = Mathf.Lerp(_perlin.m_FrequencyGain, config.frequencyGain, transitionSpeed * deltaTime);
            }
        }
    }

    private void changeXRotationAndPYRDampingWhenRotate()
    {
        
    }
    
    public void SetActive(bool isActive)
    {
        vcam.Priority = isActive ? 20 : 10;
    }
}
