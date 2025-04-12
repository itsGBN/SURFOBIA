using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] Animator mantaAnim;
    [SerializeField] Animator skeletonAnim;


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
        float moveInput = GetInputs.PS5Map.Move.ReadValue<Vector2>().y;

        skeletonAnim.SetFloat("Joystick", moveInput);
        mantaAnim.SetFloat("Joystick", moveInput);

    }

    public void DoTrick(string trickName)
    {
        skeletonAnim.SetTrigger(trickName);
        Debug.Log(trickName + "inanimanager");
    }
}
