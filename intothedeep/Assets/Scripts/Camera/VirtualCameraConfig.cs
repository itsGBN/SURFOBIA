using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CameraConfig/VirtualCameraConfiguration")]
public class VirtualCameraConfig : ScriptableObject
{
    [Header("Lens Settings")]
    public float fov=65;
    [Header("Body Settings")]
    public Vector3 followOffset = new Vector3(0.0f,4.0f,-15.0f);
    public float xDamping = 1.5f;
    public float yDamping = 2.0f;
    public float zDamping = 1.0f;
    public float pitchDamping = 2f;
    public float yawDamping = 1f;
    public float rollDamping = 1f;
    [Header("Aim Settings")] 
    public Vector3 trackedObjectOffset = new Vector3(0, 0, 0);
    public float lookaheadTime = 0f;
    public float lookaheadSmoothing = 0f;
    public bool lookaheadIgnoreY = true;
    public float horizontalDamping = 0.5f;
    public float verticalDamping = 0.2f;
    public float screenX = 0.5f;
    public float screenY = 0.8f;
    public float deadZoneWidth = 0.164f;
    public float deadZoneHeight = 0.09f;
    public float softZoneWidth = 0.8f;
    public float softZoneHeight = 1.211f;
    public float biasX = 0f;
    public float biasY = 0f;
    public bool centerOnActive = true;
    [Header("Noise Settings")]
    public Vector3 pivotOffset = new Vector3(0, 0, 0);
    public float amplitudeGain = 0.5f;
    public float frequencyGain = 0.5f;

}
