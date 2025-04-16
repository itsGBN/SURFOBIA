using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TutorialZone : MonoBehaviour
{
    [SerializeField] string fungusSignal;
    [SerializeField] CinemachineVirtualCamera cam;

    bool passed = false;

    public CinemachineVirtualCamera GetCam() { return cam; }
    public string GetSignal() { return fungusSignal; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { FindObjectOfType<TutorialManager>().EnterZone(this); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            FindObjectOfType<TutorialManager>().EnterZone(this);
        }
    }
}
