using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class IntroSignal : MonoBehaviour
{
    public GameObject imageObject;

    public CinemachineStateDrivenCamera cam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeimage()
    {
        imageObject.SetActive(false);
        cam.Priority = 11;
    }
}
