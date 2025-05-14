using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private VisualElement updown;
    private VisualElement lbeigining;
    public List<Label> leaderBoard = new List<Label>();
    //private TextField holder;
    private Label winText;
    private Label meme;
    private Button home;
    private Button up1;
    private Button down1;
    private PS5Input GetInputs;
    private Dictionary<Label, int> letter = new Dictionary<Label, int>();
    private List<Button> dpadButtons = new List<Button>();
    private int dpadnum = 0;
    private Button quitButton;
    private Button resumeButton;

    //Public Variables
    public ScoreSO[] tenScores;
    public bool scoreActive;
    public bool ayActive;

    //PrivateVraiables
    int updownIndex = 0;
    int currentCharIndex = 0; // 'A'
    const int MIN_CHAR = 0;
    const int MAX_CHAR = 25;
    bool canPress;
    bool canCheck;

    private void Awake()
    {
        GetInputs = new PS5Input();
        //Singleton
        if (instance != null && instance != this) { Destroy(instance); }
        else { instance = this; }

        //Refernces
        uIDocument = GetComponent<UIDocument>();
        leaderBoard = uIDocument.rootVisualElement.Query<Label>(null, "Leaderboard").ToList();
        scoreHUD = uIDocument.rootVisualElement.Q<VisualElement>("ScoreHUD");
        lbeigining = uIDocument.rootVisualElement.Q<VisualElement>("NameButtons");
        updown = uIDocument.rootVisualElement.Q<VisualElement>("updown");
        //holder = uIDocument.rootVisualElement.Q<TextField>("Name");
        winText = uIDocument.rootVisualElement.Q<Label>("madeit");
        meme = uIDocument.rootVisualElement.Q<Label>("meme");
        home = uIDocument.rootVisualElement.Q<Button>("Home");
        up1 = uIDocument.rootVisualElement.Q<Button>("1up");
        down1 = uIDocument.rootVisualElement.Q<Button>("1down");
        List<Label> labels = uIDocument.rootVisualElement.Query<Label>(null, "letter").ToList();
        letter = labels.ToDictionary(label => label, label => 0);
        dpadButtons = uIDocument.rootVisualElement.Query<Button>(null, "dpadbuttons").ToList();
        quitButton = uIDocument.rootVisualElement.Q("QUIT") as Button;
        resumeButton = uIDocument.rootVisualElement.Q("Credits") as Button;

        //Register event
        //home.RegisterCallback<ClickEvent>(onHomeButton);
        quitButton.RegisterCallback<ClickEvent>(onQuitButton);
        resumeButton.RegisterCallback<ClickEvent>(onResumeButton);
        home.RegisterCallback<ClickEvent>(evt => onHomeButton());
        up1.RegisterCallback<ClickEvent>(evt => UpdateLetter(-1));
        down1.RegisterCallback<ClickEvent>(evt => UpdateLetter(1));
        scoreHUD.style.visibility = Visibility.Hidden;
    }

    private void OnEnable()
    {
        GetInputs.Enable();
    }

    private void OnDisable()
    {
        //Deregister
        GetInputs.Disable();
    }

    private void Start()
    {
        //Set the Scores of the Leaderboard
        for(int i = 0; i < tenScores.Length; i++)
            leaderBoard[i].text = (i+1).ToString() + "     " + tenScores[i].score + "   :   " + tenScores[i].Name;
        foreach (var button in dpadButtons)
            button.style.visibility = Visibility.Hidden;

        //Turn the Visibility off
        LoadLeaderboardFromPrefs();
    }


    private void Update()
    {
        //Update Methods
        onActive();
        /*
        if (GetInputs.PS5Map.Menu.WasReleasedThisFrame() && scoreActive && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.ENDGAME) && updownIndex == 3) 
        {
            GameManager.instance.UpdateState(GameState.READY);
            StartCoroutine(MainMenuEvents.instance.onTransition(SceneManager.GetActiveScene().name, MainMenuEvents.instance.transitionName, 1f));
        }
        */
        if (GetInputs.PS5Map.Menu.WasReleasedThisFrame() && scoreActive && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.ENDGAME) && updownIndex == 3)
        {
            StartCoroutine(canPresse());
            StartCoroutine(onActiveCouroutine());
            lbeigining.style.visibility = Visibility.Hidden;
            OnSubmitName();
            updownIndex = 0;
        }
        if (GetInputs.PS5Map.LRight.WasReleasedThisFrame() && scoreActive && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.ENDGAME)) 
        {
            if(updownIndex < 3 && updownIndex >= 0)
            {
                updown.style.left = updown.resolvedStyle.left + 270;
                updownIndex += 1;
            }
        }
        if (GetInputs.PS5Map.LLeft.WasReleasedThisFrame() && scoreActive && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.ENDGAME))
        {
            if (updownIndex <= 3 && updownIndex > 0)
            {
                updown.style.left = updown.resolvedStyle.left - 270;
                updownIndex -= 1;
            }
        }
        if (GetInputs.PS5Map.LUp.WasReleasedThisFrame() && scoreActive && !MainMenuEvents.instance.isTrasitioning && GameManager.instance.gameState == GameManager.GameState.ENDGAME)
        {
            UpdateLetter(-1); // move up in alphabet
        }

        if (GetInputs.PS5Map.LDown.WasReleasedThisFrame() && scoreActive && !MainMenuEvents.instance.isTrasitioning && GameManager.instance.gameState == GameManager.GameState.ENDGAME)
        {
            UpdateLetter(1); // move down in alphabet
        }

        if (GetInputs.PS5Map.Menu.WasReleasedThisFrame() && scoreActive && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.ENDGAME) && canPress)
        {
            switch (dpadnum)
            {
                case 0:
                    onHomeButton();
                    break;
                case 1:
                    SceneManager.LoadScene("Credits");
                    break;
                case 2:
                    Debug.Log("Application Quit");
                    Application.Quit();
                    break;
            }
        }

        if (GetInputs.PS5Map.MenuRight.WasPressedThisFrame() && ayActive && !MainMenuEvents.instance.isTrasitioning)
        {
            dpadButtons[dpadnum].RemoveFromClassList("dpadbutton");
            dpadnum += 1;
            if (dpadnum > 2) { dpadnum = 0; }
            dpadButtons[dpadnum].AddToClassList("dpadbutton");
            print(dpadnum);

        }
        if (GetInputs.PS5Map.MenuLeft.WasPressedThisFrame() && ayActive && !MainMenuEvents.instance.isTrasitioning)
        {
            dpadButtons[dpadnum].RemoveFromClassList("dpadbutton");
            dpadnum -= 1;
            if (dpadnum < 0) { dpadnum = 3; }
            dpadButtons[dpadnum].AddToClassList("dpadbutton");
            print(dpadnum);

        }

    }

    #region Activating HUD
    private void onActive()
    {
        //Activate the HUD
        if(scoreActive)
        {
            scoreHUD.style.visibility = Visibility.Visible;
            Check();
            //StartCoroutine(onActiveCouroutine());
        }
    }

    IEnumerator canPresse()
    {
        yield return new WaitForSeconds(1f);
        canPress = true;
    }

    IEnumerator onActiveCouroutine()
    {
        //Bring in the Scores 1 by 1
        ayActive = true;
        for (int i = 0; i < 7; i++)
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
        SaveLeaderboardToPrefs();
        return insertIndex;

    }

    private void Check()
    {
        if (!canCheck && scoreActive)
        {
            int timeBonus = Mathf.FloorToInt(HUD.instance.elapsedTime / 30f) * 500;
            float finalScore = HUD.instance.GetScore() + timeBonus;
            meme.text = finalScore.ToString(); 

            int insertIndex = -1;

            for (int i = 0; i < tenScores.Length; i++)
            {
                if (finalScore > tenScores[i].score)
                {
                    insertIndex = i;
                    break;
                }
            }

            if (insertIndex == -1)
            {
                lbeigining.style.visibility = Visibility.Hidden;
                StartCoroutine(canPresse());
                StartCoroutine(onActiveCouroutine());
                OnSubmitName();
                updownIndex = 0;
            }
            canCheck = true;
        }
    }

    private void OnSubmitName()
    {
        //string playerName = holder.value.Trim();
        string playerName = letter.Keys.ElementAt(0).text + letter.Keys.ElementAt(1).text + letter.Keys.ElementAt(2).text;
        int timeBonus = Mathf.FloorToInt(HUD.instance.elapsedTime / 30f) * 500;
        float finalScore = HUD.instance.GetScore() + timeBonus;

        if (!string.IsNullOrEmpty(playerName))
        {
            int insertedIndex = TryInsertNewHighScore(playerName, finalScore);
            //holder.style.display = DisplayStyle.None;

            if (insertedIndex != -1)
            {
                leaderBoard[insertedIndex].AddToClassList("Leaderboard-Submit");
            }
        }
        foreach (var button in dpadButtons)
            button.style.visibility = Visibility.Visible;
    }
    #endregion

    #region Buttons
    private void onHomeButton()
    {
        GameManager.instance.UpdateState(GameState.READY);
        StartCoroutine(MainMenuEvents.instance.onTransition(SceneManager.GetActiveScene().name, MainMenuEvents.instance.transitionName, 1f));
    }

    private void UpdateLetter(int direction)
    {
        if (updownIndex < 0 || updownIndex >= letter.Count) return;

        // Get the current label
        Label currentLabel = letter.Keys.ElementAt(updownIndex);

        // Get current index and apply change
        int currentIndex = letter[currentLabel];
        currentIndex = Mathf.Clamp(currentIndex + direction, MIN_CHAR, MAX_CHAR);

        // Update dictionary and label text
        letter[currentLabel] = currentIndex;
        currentLabel.text = ((char)('A' + currentIndex)).ToString();
    }
        private void onQuitButton(ClickEvent e)
        {
            Debug.Log("Application Quit");
            Application.Quit();
        }

        private void onResumeButton(ClickEvent e)
        {
            SceneManager.LoadScene("Credits");
        }
    #endregion

    #region Player Prefs
    public void SaveLeaderboardToPrefs()
    {
        for (int i = 0; i < tenScores.Length; i++)
        {
            PlayerPrefs.SetString($"HighScore{i}_Name", tenScores[i].Name);
            PlayerPrefs.SetFloat($"HighScore{i}_Score", tenScores[i].score);
        }

        PlayerPrefs.Save(); // important!
        Debug.Log("Leaderboard saved to PlayerPrefs.");
    }

    public void LoadLeaderboardFromPrefs()
    {
        for (int i = 0; i < tenScores.Length; i++)
        {
            string nameKey = $"HighScore{i}_Name";
            string scoreKey = $"HighScore{i}_Score";

            if (PlayerPrefs.HasKey(nameKey) && PlayerPrefs.HasKey(scoreKey))
            {
                tenScores[i].Name = PlayerPrefs.GetString(nameKey);
                tenScores[i].score = PlayerPrefs.GetFloat(scoreKey);
            }
            else
            {
                // Fallback defaults if nothing exists
                tenScores[i].Name = "---";
                tenScores[i].score = 0;
            }

            leaderBoard[i].text = (i + 1) + "     " + tenScores[i].score.ToString("F0") + "   :   " + tenScores[i].Name;
        }

        Debug.Log("Leaderboard loaded from PlayerPrefs.");
    }

    #endregion
}
