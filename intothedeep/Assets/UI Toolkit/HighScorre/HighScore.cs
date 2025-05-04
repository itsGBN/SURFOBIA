using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static GameManager;

public class HighScore : MonoBehaviour
{
    //Singleton
    public static HighScore instance;
    
    //Refernces
    private UIDocument uIDocument;
    private VisualElement scoreHUD;
    private List<Label> leaderBoard = new List<Label>();
    private TextField holder;
    private Label winText;
    private Button home;
    
    //Public Variables
    public ScoreSO[] tenScores;
    public bool scoreActive;

    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this) { Destroy(instance); }
        else { instance = this; }

        //Refernces
        uIDocument = GetComponent<UIDocument>();
        leaderBoard = uIDocument.rootVisualElement.Query<Label>(null, "Leaderboard").ToList();
        scoreHUD = uIDocument.rootVisualElement.Q<VisualElement>("ScoreHUD");
        holder = uIDocument.rootVisualElement.Q<TextField>("Name");
        winText = uIDocument.rootVisualElement.Q<Label>("madeit");
        home = uIDocument.rootVisualElement.Q<Button>("Home");
        
        //Register event
        holder.RegisterCallback<KeyDownEvent>(evt =>
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                OnSubmitName(); // Your function to insert the high score
                evt.StopPropagation(); // prevent bubbling
            }
        });
        home.RegisterCallback<ClickEvent>(onHomeButton);
    }

    private void Start()
    {
        //Set the Scores of the Leaderboard
        for(int i = 0; i < tenScores.Length; i++)
            leaderBoard[i].text = (i+1).ToString() + "     " + tenScores[i].score + "   :   " + tenScores[i].Name;

        //Turn the Visibility off
        scoreHUD.style.visibility = Visibility.Hidden;
    }


    private void Update()
    {
        //Update Methods
        onActive();
    }

    #region Activating HUD
    private void onActive()
    {
        //Activate the HUD
        if(scoreActive)
        {
            scoreHUD.style.visibility = Visibility.Visible;
            StartCoroutine(onActiveCouroutine());
        }
    }

    IEnumerator onActiveCouroutine()
    {
        //Bring in the Scores 1 by 1
        for (int i = 0; i < tenScores.Length; i++)
        {
            leaderBoard[i].AddToClassList("Leaderboard-Active");
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    #region Insert Name
    public int TryInsertNewHighScore(string playerName, float newScore)
    {
        int insertIndex = -1;

        for (int i = 0; i < tenScores.Length; i++)
        {
            if (newScore > tenScores[i].score)
            {
                insertIndex = i;
                break;
            }
        }

        if (insertIndex == -1)
        {
            winText.text = "YOU DID NOT MAKE IT\n\nYour Score Was Not In The Top 10";
            return -1;
        }
        else
        {
            winText.text = "YOU MADE IT\n\nYour Score Was In The Top 10";
        }

        for (int i = tenScores.Length - 1; i > insertIndex; i--)
        {
            tenScores[i].score = tenScores[i - 1].score;
            tenScores[i].Name = tenScores[i - 1].Name;
        }

        tenScores[insertIndex].score = newScore;
        tenScores[insertIndex].Name = playerName;

        for (int i = 0; i < tenScores.Length; i++)
        {
            leaderBoard[i].text = tenScores[i].Name + " : " + tenScores[i].score.ToString("F0");
        }

        return insertIndex;
    }



    private void OnSubmitName()
    {
        string playerName = holder.value.Trim();
        float finalScore = HUD.instance.GetScore();

        if (!string.IsNullOrEmpty(playerName))
        {
            int insertedIndex = TryInsertNewHighScore(playerName, finalScore);
            holder.style.display = DisplayStyle.None;

            if (insertedIndex != -1)
            {
                leaderBoard[insertedIndex].AddToClassList("Leaderboard-Submit");
            }
        }
    }
    #endregion

    #region Buttons
    private void onHomeButton(ClickEvent e)
    {
        GameManager.instance.UpdateState(GameState.READY);
        StartCoroutine(MainMenuEvents.instance.onTransition(SceneManager.GetActiveScene().name, MainMenuEvents.instance.transitionName, 1f));
    }
    #endregion
}
