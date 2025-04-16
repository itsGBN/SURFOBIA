using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{

    [SerializeField] GameObject animationManager;

    bool canJump;

    #region CONTROLLER
    private PS5Input GetInputs;

    private void Awake()
    {
        GetInputs = new PS5Input();
    }

    private void OnEnable()
    {
        GetInputs.Enable();
    }

    private void OnDisable()
    {
        GetInputs.Disable();
    }
    #endregion CONTROLLER

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (canJump && GetInputs.PS5Map.Jump.WasPressedThisFrame())
        {
            animationManager.GetComponent<AnimationManager>().DoJump();
            canJump = false;
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Jump")) canJump = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canJump = false;
    }
}
