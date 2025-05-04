using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    private VisualElement redVisualElement;
    private VisualElement endscreenVisualElement;
    private Label countdownLabel;
    private Label scoreLabel;
    private Label comboLabel;
    private Dictionary<Label, (int, string)> enemyScores = new Dictionary<Label, (int, string)>();
    private Label trickLabel;
    private int trickNum;
    private int scoreNum;
    private float comboNum;
    private int enemyNum;
    private Queue<Label> playerTricks = new Queue<Label>();
    private ProgressBar progressBar;
    private float elapsedTime = 0f;

    [SerializeField] [Range(1, 1000)] int distannceRange;

    private bool focusEndscreen = false;


    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this) { Destroy(instance); }
        else { instance = this; }

        //Refernce UI
        uIDocument = GetComponent<UIDocument>();

        //Reference Labels
        hudVisualElement = uIDocument.rootVisualElement.Q("HUD") as VisualElement;
        redVisualElement = uIDocument.rootVisualElement.Q("Red") as VisualElement;
        endscreenVisualElement = uIDocument.rootVisualElement.Q("Endscreen") as VisualElement;
        distanceLabel = uIDocument.rootVisualElement.Q("Distance") as Label;
        countdownLabel = uIDocument.rootVisualElement.Q("Countdown") as Label;
        scoreLabel = uIDocument.rootVisualElement.Q("Score") as Label;
        comboLabel = uIDocument.rootVisualElement.Q("Mult") as Label;
        comboLabel.text = "x"+comboNum.ToString();
        List<Label> labels = uIDocument.rootVisualElement.Query<Label>(null, "scores").ToList();
        enemyScores = labels.ToDictionary(label => label, label => (0, "Name"));
        progressBar = uIDocument.rootVisualElement.Q("Combo") as ProgressBar;


        //Miscellaneous Things
        //Naming the Scorer
        foreach (var entry in enemyScores.ToList())
        {
            Label label = entry.Key;
            int score = entry.Value.Item1;
            string text = entry.Value.Item2;

            var list = enemyScores.ToList();
            int index = list.FindIndex(entry => entry.Key == label);

            if(index == 0) { enemyScores[label] = (score, "Tibia H"); }
            if(index == 1) { enemyScores[label] = (score, "Ribcage"); }
            if(index == 2) { enemyScores[label] = (score, "Boney S"); }
            if(index == 3) { enemyScores[label] = (score, "Femur S"); }
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

    public IEnumerator onRed()
    {
        redVisualElement.style.visibility = Visibility.Visible;
        redVisualElement.style.opacity = 0;
        yield return new WaitForSeconds(0.6f);
        redVisualElement.style.visibility = Visibility.Hidden;
        redVisualElement.style.opacity = 0.5f;
    }

    public void SetColor()
    {

    }

    private void onVisibility()
    {
        if (MainMenuEvents.instance.focusMenu || focusEndscreen || HighScore.instance.scoreActive)
        {
            hudVisualElement.style.visibility = Visibility.Hidden;
        }
        else
        {
            hudVisualElement.style.visibility = Visibility.Visible;
        }
    }

    private void onDistance()
    {
        if(GameManager.instance.gameState == GameManager.GameState.RACING)
        {
            elapsedTime += Time.deltaTime;

            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            int milliseconds = (int)((elapsedTime * 1000) % 1000);

            distanceLabel.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    private void OnEnemyScore()
    {
        if (!MainMenuEvents.instance.focusMenu)
        {
            foreach (var entry in enemyScores.ToList())
            {
                Label label = entry.Key;
                int score = entry.Value.Item1;
                string text = entry.Value.Item2;
                int points = UnityEngine.Random.Range(1, 4);

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

                switch (index + 1)
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
    }

    private void onPlayerTrick()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { onPlayerTrickHud("FLIP", 10); }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {  onPlayerTrickHud("GRIND", 20); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { onPlayerTrickHud("JUMP", 30); }
    }

    public void onPlayerTrickHud(string trickname, int points)
    {
        trickNum++;
        trickLabel = new Label(trickname);
        trickLabel.AddToClassList("label");
        trickLabel.AddToClassList("tricks");
        trickLabel.AddToClassList("tricker");

        uIDocument.rootVisualElement.Add(trickLabel);

        playerTricks.Enqueue(trickLabel);
        Label[] queueArray = playerTricks.ToArray();

        StartCoroutine(MoveTrick(queueArray));

        scoreNum+= points;
        scoreLabel.text = scoreNum.ToString(); 

        progressBar.value += points;
        if(progressBar.value > 100) { progressBar.value = 0; comboNum += .5f; comboLabel.text = "x"+comboNum.ToString(); }

    }

    IEnumerator MoveTrick(Label[] queueArray)
    {
        for (int i = 0; i < queueArray.Length; i++)
        {
            yield return new WaitForSeconds(0f);
            if (queueArray[i].ClassListContains("tricker")) { queueArray[i].RemoveFromClassList("tricker"); queueArray[i].AddToClassList(("tzero")); continue; }
            if (queueArray[i].ClassListContains("tzero")) { queueArray[i].RemoveFromClassList("tzero"); queueArray[i].AddToClassList(("tone")); continue; }
            if (queueArray[i].ClassListContains("tone")) { queueArray[i].RemoveFromClassList("tone"); queueArray[i].AddToClassList(("ttwo")); continue; }
            if (queueArray[i].ClassListContains("ttwo")) { queueArray[i].RemoveFromClassList("ttwo"); queueArray[i].AddToClassList(("tthree")); continue; }
            if (queueArray[i].ClassListContains("tthree")) { queueArray[i].RemoveFromClassList("tthree"); queueArray[i].AddToClassList(("tfour")); continue; }
            if (queueArray[i].ClassListContains("tfour")) { Label toDelete = playerTricks.Dequeue(); uIDocument.rootVisualElement.Remove(toDelete); toDelete = null; continue; }
        }

    }

    public void UpdateCountdown(float value)
    {
        countdownLabel.style.visibility = Visibility.Visible;
        countdownLabel.text = value.ToString();
        if (value <= 0) { countdownLabel.style.visibility = Visibility.Hidden; }
    }


    public void Endscreen()
    {
        /*
        focusEndscreen = true;
        endscreenVisualElement.style.opacity = 1;
        endscreenVisualElement.style.visibility = Visibility.Visible;
        // update stats
        */
    }

    public int GetScore()
    {
        return scoreNum;
    }
}

class Trick
{
    public Label trickLabel;
    public int trickInt;
}
