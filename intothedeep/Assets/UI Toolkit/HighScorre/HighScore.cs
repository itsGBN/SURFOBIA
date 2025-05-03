using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HighScore : MonoBehaviour
{
    public static HighScore instance;
    private UIDocument uIDocument;
    public ScoreSO[] tenScores;

    private List<Label> leaderBoard = new List<Label>();

    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this) { Destroy(instance); }
        else { instance = this; }

        //Refernce UI
        uIDocument = GetComponent<UIDocument>();
        leaderBoard = uIDocument.rootVisualElement.Query<Label>(null, "Leaderboard").ToList();
    }

    private void Start()
    {
        for(int i = 0; i < tenScores.Length; i++)
        {
            leaderBoard[i].text = tenScores[i].Name + " : " + tenScores[i].score;
        }
    }
}
