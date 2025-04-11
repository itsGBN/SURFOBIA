using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrickListener : MonoBehaviour
{
    public TestTrick01 testTrick01;
    public List<TricksListScriptableObject> predefinedTricks = new List<TricksListScriptableObject>();

    [Header("Changeable Variables")]
    [SerializeField] float maxTrickTime;

    [Header("GameObjects")]
    [SerializeField] AnimationManager AnimationManager;

    string lastTrick;
    bool isDoingTrick;
    float trickTimer;

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


        if (!isDoingTrick && AnyListMatches(testTrick01.playerInputList, predefinedTricks) && testTrick01.timerOn)
        {
            HUD.instance.onPlayerTrickHud(lastTrick); //publishes trick to be viewed by player
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
                    testTrick01.playerInputList.Clear();
                    return true;
                }

                recentInputs.Clear();

            }

        }

        return false;
    }
}
