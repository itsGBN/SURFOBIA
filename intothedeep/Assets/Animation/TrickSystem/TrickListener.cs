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
    bool isDoingTrick;
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

        if (testTrick01.playerInputList.Count == 0)
        {
            // text.text = "No Trick";
            // Debug.Log("Trick: " + testTrick01.playerInputList[testTrick01.playerInputList.Count - 1]);
        }


        testTrick01.isChecking = !isDoingTrick;


        if (isDoingTrick)
        {
            trickTimer += Time.deltaTime;

            /*
            if (!isDoingFall)
            { //LAST BUTTON MUST BE HELD TO COMPLETE TRICK

                Vector2 trickValue = GetInputs.PS5Map.TrickStick.ReadValue<Vector2>();

                if ((lastTrickInput == "up" && trickValue.y > 0.5f) || (lastTrickInput == "down" && trickValue.y < -0.5f)
                || (lastTrickInput == "right" && trickValue.x > 0.5f) || (lastTrickInput == "left" && trickValue.x < -0.5f)
                || (lastTrickInput == "rightTrigger" && GetInputs.PS5Map.RightTrigger.WasPressedThisFrame())
                || (lastTrickInput == "rightTrigger" && GetInputs.PS5Map.RightTrigger.WasPressedThisFrame()))
                {

                }
                else
                {
                    //TRICK FAILED

                    //isDoingTrick = false;
                    trickTimer = 0;
                    isDoingFall = true;
                    HUD.instance.onPlayerTrickHud(lastTrick + " FAILED");
                    HUD.instance.onPlayerTrickHud("COMBO LOST");

                    AnimationManager.DoTrick("Fall");

                    inComboMode = false;
                    totalComboPoints = 0;
                    comboMultiplier = 1;
                }
            }
            */
            

            if (trickTimer >= maxTrickTime)
            {
                //text.text = "Waiting for Next Trick";
                //if (!isDoingFall) {

                    HUD.instance.onPlayerTrickHud(lastTrick + " +" + lastPointValue + " x " + comboMultiplier); //publishes trick to be viewed by player

                    totalPointValue += lastPointValue * comboMultiplier;
                    totalComboPoints += lastPointValue;
                    comboTimer = 0;
                    inComboMode = true;

                    if (totalComboPoints > 50 * (1.5f * (comboMultiplier)))
                    {
                        comboMultiplier += 1;
                        HUD.instance.onPlayerTrickHud("COMBO MULTIPLER: x" + comboMultiplier);
                    }
                //}
               
                trickTimer = 0;
                isDoingTrick = false;
                isDoingFall = false;
                
            }
        }

        if (inComboMode) {
            comboTimer += Time.deltaTime;

            if(comboTimer >= maxComboTime)
            {
                inComboMode = false;
                totalComboPoints = 0;
                comboMultiplier = 1;

                HUD.instance.onPlayerTrickHud("COMBO LOST");
            }
        }


        if (!isDoingTrick && AnyListMatches(testTrick01.playerInputList, predefinedTricks) && testTrick01.timerOn)
        {

            Debug.Log(lastTrickInput);
            
            
 
            //Debug.Log(lastTrick);
            AnimationManager.DoTrick(lastTrick);
            isDoingTrick = true;
        }


    }

    private bool ListsMatch(List<string> list1, List<string> list2)
    {

        if (list1.Count != list2.Count)
        {

            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {

            if (list1[i] != list2[i])
            {

                return false;
            }

        }


        lastTrickInput = list2[list2.Count - 1];

        return true;
    }

    //so this checks all the possible tricks, and if they match, it will return true
    private bool AnyListMatches(List<string> playerInputList, List<TricksListScriptableObject> predefinedTricks)
    {
        for (int i = 0; i < predefinedTricks.Count; i++)
        {
            predefinedTricks[i].ConvertTestListToPlayerInputList();


            if (playerInputList.Count >= predefinedTricks[i].playerInputList.Count)
            {

                List<string> recentInputs = new List<string>();

                for (int j = predefinedTricks[i].playerInputList.Count; j > 0; j--)
                {
                    recentInputs.Add(playerInputList[playerInputList.Count - j].ToString());
                }

                if (ListsMatch(recentInputs, predefinedTricks[i].playerInputList))
                {
                    lastTrick = predefinedTricks.Find(x => ListsMatch(recentInputs, x.playerInputList)).trickName;
                    lastPointValue = predefinedTricks.Find(x => ListsMatch(recentInputs, x.playerInputList)).pointsAmount;
                    testTrick01.playerInputList.Clear();
                    return true;
                }

                recentInputs.Clear();

            }

        }

        return false;
    }
}
