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
    float trickTimer;

    //COMBO VARIABLES
    float comboTimer; //Tracks maximum time next trick needs to be completed
    int comboMultiplier = 1;
    float lastPointValue; //Point value gained by most recent trip
    float totalPointValue; //Total points gained
    float totalComboPoints; //Counts points towards Combo
    bool inComboMode; //Whether or not counting Combo

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

        if (isDoingTrick)
        {
            trickTimer += Time.deltaTime;

            if (trickTimer >= maxTrickTime)
            {
                //text.text = "Waiting for Next Trick";
               
                trickTimer = 0;
                isDoingTrick = false;
            }
        }

        if (inComboMode) {
            comboTimer += Time.deltaTime;

            if(comboTimer >= maxComboTime)
            {
                inComboMode = false;
                totalComboPoints = 0;
                comboMultiplier = 1;
            }
        }


        if (!isDoingTrick && AnyListMatches(testTrick01.playerInputList, predefinedTricks) && testTrick01.timerOn)
        {
            HUD.instance.onPlayerTrickHud(lastTrick + " +" + lastPointValue + " x " + comboMultiplier); //publishes trick to be viewed by player
            totalPointValue += lastPointValue * comboMultiplier;
            totalComboPoints += lastPointValue * comboMultiplier;
            comboTimer = 0;
            inComboMode = true;

            if (totalComboPoints > 50 * (1.5f * (comboMultiplier))) {
                comboMultiplier += 1;
                HUD.instance.onPlayerTrickHud("COMBO MULTIPLER: x" + comboMultiplier);
            } 
 
            Debug.Log(lastTrick);
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
