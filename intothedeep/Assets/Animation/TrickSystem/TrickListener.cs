using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TrickListener : MonoBehaviour
{
    public TestTrick01 testTrick01;
    public List<TricksListScriptableObject> predefinedTricks = new List<TricksListScriptableObject>();

    [Header("Changeable Variables")]
    [SerializeField] float maxTrickTime;
    [SerializeField] float maxComboTime; //Tracks maximum time next trick needs to be completed

    [Header("GameObjects")]
    [SerializeField] AnimationManager AnimationManager;

    string lastTrick;
    public bool isDoingTrick;
    bool isDoingFall;
    float trickTimer;
    string lastTrickInput;

    //COMBO VARIABLES
    float comboTimer; //Tracks maximum time next trick needs to be completed
    int comboMultiplier = 1;
    float lastPointValue; //Point value gained by most recent trip
    float totalPointValue; //Total points gained
    float totalComboPoints; //Counts points towards Combo
    bool inComboMode; //Whether or not counting Combo

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

    }
}
