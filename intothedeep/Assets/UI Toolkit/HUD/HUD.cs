using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    private UIDocument uIDocument;
    private Label distanceLabel;
    private VisualElement hudVisualElement;
    private Dictionary<Label, (int, string)> enemyScores = new Dictionary<Label, (int, string)>();
    private Label trickLabel;
    private int trickNum;
    private Queue<Label> playerTricks = new Queue<Label>();

    [SerializeField] [Range(1, 1000)] int distannceRange;


    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this) { Destroy(instance); }
        else { instance = this; }

        //Refernce UI
        uIDocument = GetComponent<UIDocument>();

        //Reference Labels
        hudVisualElement = uIDocument.rootVisualElement.Q("HUD") as VisualElement;
        distanceLabel = uIDocument.rootVisualElement.Q("Distance") as Label;
        List<Label> labels = uIDocument.rootVisualElement.Query<Label>(null, "scores").ToList();
        enemyScores = labels.ToDictionary(label => label, label => (0, "Name"));
  

        //Miscellaneous Things
        //Naming the Scorer
        foreach (var entry in enemyScores.ToList())
        {
            Label label = entry.Key;
            int score = entry.Value.Item1;
            string text = entry.Value.Item2;

            var list = enemyScores.ToList();
            int index = list.FindIndex(entry => entry.Key == label);

            if(index == 0) { enemyScores[label] = (score, "Mia"); }
            if(index == 1) { enemyScores[label] = (score, "Maryam"); }
            if(index == 2) { enemyScores[label] = (score, "Zoey"); }
            if(index == 3) { enemyScores[label] = (score, "Victor"); }
        }

    }

    private void Start()
    {
        InvokeRepeating("OnEnemyScore", 1, 1);
    }

    private void Update()
    {
        onDistance();
        onPlayerTrick();
        onVisibility();
    }

    private void onVisibility()
    {
        if (MainMenuEvents.instance.focusMenu)
        {
            hudVisualElement.style.opacity = 0;
        }
        else
        {
            hudVisualElement.style.opacity = 100;
        }
    }

    private void onDistance()
    {
        distanceLabel.text = distannceRange.ToString();
    }

    private void OnEnemyScore()
    {
        foreach (var entry in enemyScores.ToList()) 
        {
            Label label = entry.Key;
            int score = entry.Value.Item1; 
            string text = entry.Value.Item2; 
            int points = UnityEngine.Random.Range(1,4);

            enemyScores[label] = (score + points, text);
            label.text = $"{text}: {score}";
        }

        enemyScores = enemyScores.OrderByDescending(entry => entry.Value.Item1).ToDictionary(entry => entry.Key, entry => entry.Value);

        foreach (var entry in enemyScores.ToList())
        {
            Label label = entry.Key;
            int score = entry.Value.Item1;
            string text = entry.Value.Item2;

            if (label.ClassListContains("one")) { label.RemoveFromClassList("one"); }
            if (label.ClassListContains("two")) { label.RemoveFromClassList("two"); }
            if (label.ClassListContains("three")) { label.RemoveFromClassList("three"); }
            if (label.ClassListContains("four")) { label.RemoveFromClassList("four"); }

            var list = enemyScores.ToList();
            int index = list.FindIndex(entry => entry.Key == label);

            switch (index+1)
            {
                case 1:
                    label.AddToClassList(("one").ToString()); break;
                case 2:
                    label.AddToClassList(("two").ToString()); break;
                case 3:
                    label.AddToClassList(("three").ToString()); break;
                case 4:
                    label.AddToClassList(("four").ToString()); break;

            }

        }
    }

    private void onPlayerTrick()
    {
        string trickName = null;

        if (Input.GetKeyDown(KeyCode.Alpha1)) { trickName = "FLIP"; onPlayerTrickHud(); }
        if(Input.GetKeyDown(KeyCode.Alpha2)) { trickName = "GRIND"; onPlayerTrickHud(); }
        if(Input.GetKeyDown(KeyCode.Alpha3)) { trickName = "JUMP"; onPlayerTrickHud(); }
    }

    private void onPlayerTrickHud()
    {
        trickNum++;
        trickLabel = new Label("TrickName" + trickNum.ToString());
        trickLabel.AddToClassList("label");
        trickLabel.AddToClassList("tricks");
        trickLabel.AddToClassList("tricker");

        uIDocument.rootVisualElement.Add(trickLabel);

        playerTricks.Enqueue(trickLabel);
        Label[] queueArray = playerTricks.ToArray();

        for (int i = 0; i < queueArray.Length; i++)
        {
            if (queueArray[i].ClassListContains("tricker")) { queueArray[i].RemoveFromClassList("tricker"); queueArray[i].AddToClassList(("tzero")); continue; }
            if (queueArray[i].ClassListContains("tzero")) { queueArray[i].RemoveFromClassList("tzero"); queueArray[i].AddToClassList(("tone")); continue; }
            if (queueArray[i].ClassListContains("tone")) { queueArray[i].RemoveFromClassList("tone"); queueArray[i].AddToClassList(("ttwo")); continue;  }
            if (queueArray[i].ClassListContains("ttwo")) { queueArray[i].RemoveFromClassList("ttwo"); queueArray[i].AddToClassList(("tthree")); continue; }
            if (queueArray[i].ClassListContains("tthree")) { queueArray[i].RemoveFromClassList("tthree"); queueArray[i].AddToClassList(("tfour")); continue; }
            if (queueArray[i].ClassListContains("tfour")) { Label toDelete = playerTricks.Dequeue(); uIDocument.rootVisualElement.Remove(toDelete); toDelete = null; continue; }
        }
    }
}

 class Trick
{
    public Label trickLabel;
    public int trickInt;
}
