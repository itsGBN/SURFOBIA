using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class ControllerDialogue : MonoBehaviour
{
    //[SerializeField] DialogInput dialogInput;

    bool clickedNext;
    float clickDelay;

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
        if (GetInputs.PS5Map.Jump.WasReleasedThisFrame() && !clickedNext) {
            this.GetComponent<DialogInput>().SetClickAnywhereClickedFlag();
            clickedNext = true;
        } 

        if(clickedNext)
        {
            clickDelay += Time.deltaTime;

            if(clickDelay > 2)
            {
                clickDelay = 0;
                clickedNext = false;
            }
        }

        
    }
}
