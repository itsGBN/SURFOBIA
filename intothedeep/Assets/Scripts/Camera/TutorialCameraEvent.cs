using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TutorialCameraEvent : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void reducePriority()
    {
        vcam.Priority = 9;
    }

    public void risePriority()
    {
        vcam.Priority = 20;
    }
}
