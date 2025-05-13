using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static GameManager;

public class MainMenuEvents : MonoBehaviour
{
    public static MainMenuEvents instance;

    [SerializeField] Texture2D controllerImage;
    [SerializeField] Texture2D keyboardImage;

    private UIDocument uIDocument;

    private List<Button> allButtons = new List<Button>();
    private Button levelButton;
    private Button level1Button;
    private Button tutorialButton;
    private Button inputButton;
    private Button quitButton;
    private Button creditsButton;
    private List<Button> levelChildrenButtons = new List<Button>();
    private List<Button> dpadButtons = new List<Button>();
    private int dpadnum = 0;
    private List<VisualElement> allElements = new List<VisualElement>();

    private Label gameMode;
    private Label explanation;

    private VisualElement fadeIn;
    private VisualElement squareIn;
    private VisualElement trasitionTypes;
    public VisualElement transitionName;
    private VisualElement inputPicture;
    private VisualElement mainMenu;
    private string transitionDescription;
    public bool isTrasitioning = true;

    private PS5Input GetInputs;
    private enum ChooseTransition
    {
        FadeIn,
        SqaureIn
    }
    [SerializeField] ChooseTransition chooseTransition;

    private AudioSource buttonSound;

    [HideInInspector] public bool focusMenu = true;

    private void Awake()
    {
        GetInputs = new PS5Input();
        //Singleton
        if (instance != null && instance != this) { Destroy(instance); }
        else { instance = this; }

        //Refernce UI 
        uIDocument = GetComponent<UIDocument>();
        allElements = uIDocument.rootVisualElement.Query<VisualElement>().ToList();

        //Reference Buttons
        levelButton = uIDocument.rootVisualElement.Q("LevelButton") as Button;
        level1Button = uIDocument.rootVisualElement.Q("Level1Button") as Button;
        tutorialButton = uIDocument.rootVisualElement.Q("Tutorial") as Button;
        //inputButton = uIDocument.rootVisualElement.Q("Input") as Button;
        quitButton = uIDocument.rootVisualElement.Q("Quit") as Button;
        creditsButton = uIDocument.rootVisualElement.Q("Credits") as Button;
        allButtons = uIDocument.rootVisualElement.Query<Button>().ToList();
        levelChildrenButtons = uIDocument.rootVisualElement.Query<Button>(null, "levelChildren").ToList();
        dpadButtons = uIDocument.rootVisualElement.Query<Button>(null, "dpadbuttons").ToList();

        //Reference Visual Elements
        trasitionTypes = uIDocument.rootVisualElement.Q("TransitionTypes") as VisualElement;
        fadeIn = uIDocument.rootVisualElement.Q("FadeIn") as VisualElement;
        squareIn = uIDocument.rootVisualElement.Q("SquareIn") as VisualElement;
        mainMenu = uIDocument.rootVisualElement.Q("MainButtons") as VisualElement;
        inputPicture = uIDocument.rootVisualElement.Q("InputPicture") as VisualElement;

        //Reference Labels
        gameMode = uIDocument.rootVisualElement.Q("gamemode") as Label;
        explanation = uIDocument.rootVisualElement.Q("explanation") as Label;

        //Register
        levelButton.RegisterCallback<ClickEvent>(onPlayButton);
        level1Button.RegisterCallback<ClickEvent>(onPlayParentButtons);
        // IDK WHY but registering these buttons is breaking the game start
        // FIX ^^^ it was because the reference to the button was wrong lol
        tutorialButton.RegisterCallback<ClickEvent>(onTutorialButton);
        //inputButton.RegisterCallback<ClickEvent>(onInputButton);
        quitButton.RegisterCallback<ClickEvent>(onQuitButton);
        creditsButton.RegisterCallback<ClickEvent>(onCreditsButton);
        foreach (Button button in allButtons) { button.RegisterCallback<ClickEvent>(onAllButtons); }
        //Miscelleaneous Things
        //Make Inactive Buttons Dissapear
        foreach (Button button in levelChildrenButtons) { if (button.ClassListContains("levelChildrenActive")) { button.AddToClassList("levelChildrenInactive"); } }

        //Add the Transition
        if (fadeIn.ClassListContains("fadeOut") && chooseTransition == ChooseTransition.FadeIn) { fadeIn.RemoveFromClassList("fadeOut"); transitionName = fadeIn; transitionDescription = "fadeOut"; }
        if (squareIn.ClassListContains("squareOut") && chooseTransition == ChooseTransition.SqaureIn) { squareIn.RemoveFromClassList("squareOut"); transitionName = squareIn; transitionDescription = "squareOut"; }

        //Get AudioSource
        buttonSound = GetComponent<AudioSource>();
        dpadButtons[dpadnum].AddToClassList("buttonhover");
    }

    private void OnEnable()
    {
        GetInputs.Enable();
    }

    private void OnDisable()
    {
        //Deregister
        GetInputs.Disable();
        levelButton.UnregisterCallback<ClickEvent>(onPlayButton);
        level1Button.UnregisterCallback<ClickEvent>(onPlayParentButtons);
        tutorialButton.UnregisterCallback<ClickEvent>(onTutorialButton);
        //inputButton.UnregisterCallback<ClickEvent>(onInputButton);
        quitButton.UnregisterCallback<ClickEvent>(onQuitButton);
        creditsButton.UnregisterCallback<ClickEvent>(onCreditsButton);
        foreach (Button button in allButtons) { button.UnregisterCallback<ClickEvent>(onAllButtons); }
    }

    private void Start()
    {
        StartCoroutine(onTransition(transitionName));
    }


    private void Update()
    {
        if (GetInputs.PS5Map.Menu.WasPressedThisFrame() && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.READY))
        {
            switch (dpadnum) 
            {
                case 0:
                    if (mainMenu.ClassListContains("menuInactive")) { mainMenu.RemoveFromClassList("menuInactive"); focusMenu = true; GameManager.instance.PauseGame(); }
                    else { mainMenu.AddToClassList("menuInactive"); focusMenu = false; GameManager.instance.UnpauseGame(); }
                    break;
                case 1:
                    SceneManager.LoadScene("Tutorial");
                    break;
                case 2:
                    SceneManager.LoadScene("Credits");
                    break;
                case 3:
                    Debug.Log("Application Quit");
                    Application.Quit();
                    break;
            }
        }

        if (GetInputs.PS5Map.MenuRight.WasPressedThisFrame() && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.READY))
        {
            dpadButtons[dpadnum].RemoveFromClassList("buttonhover");
            dpadnum += 1;
            if(dpadnum > 3) { dpadnum = 0; }
            dpadButtons[dpadnum].AddToClassList("buttonhover");
            print(dpadnum);

        }
        if (GetInputs.PS5Map.MenuLeft.WasPressedThisFrame() && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.READY))
        {
            dpadButtons[dpadnum].RemoveFromClassList("buttonhover");
            dpadnum -= 1;
            if (dpadnum < 0) { dpadnum = 3; }
            dpadButtons[dpadnum].AddToClassList("buttonhover");
            print(dpadnum);

        }

        switch(dpadnum)
        {
            case 0:
                gameMode.text = "Poseidon's Time Trail";
                explanation.text = "Do gnarly tricks and race to the finish line before time runs out.";
                break;
            case 1:
                gameMode.text = "Surf School";
                explanation.text = "Learn how to surf, flip, and spin like a champ from a reknowned expert.";
                break;
            case 2:
                gameMode.text = "Meet the Team";
                explanation.text = "The team behind Surfobia: Into the Deep";
                break;
            case 3:
                gameMode.text = "Quit";
                explanation.text = "Leave behind your ray and give up on becoming Poseidon's next champion.";
                break;
        }
    }

    private void onAllButtons(ClickEvent e)
    {
        //buttonSound.Play();
    }

    //Play Button
    private void onPlayButton(ClickEvent e)
    {
        //if (mainMenu.ClassListContains("menuInactive")) { mainMenu.RemoveFromClassList("menuInactive"); focusMenu = true; GameManager.instance.PauseGame(); }
        //else { mainMenu.AddToClassList("menuInactive"); focusMenu = false; GameManager.instance.UnpauseGame(); }
    }

    private void onPlayParentButtons(ClickEvent e)
    {
        GameManager.instance.UpdateState(GameState.READY);
        StartCoroutine(MainMenuEvents.instance.onTransition(SceneManager.GetActiveScene().name, MainMenuEvents.instance.transitionName, 1f));
    }

    private void onTutorialButton(ClickEvent e)
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void onCreditsButton(ClickEvent e)
    {
        SceneManager.LoadScene("Credits");
    }

    private void onInputButton(ClickEvent e)
    {
        GameManager.SetInputType(!GameManager.INPUT_CONTROLLER);
        inputPicture.style.backgroundImage = GameManager.INPUT_CONTROLLER ? controllerImage : keyboardImage;
    }

    private void onQuitButton(ClickEvent e)
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    public IEnumerator onTransition(string sceneName, VisualElement transitionName, float waitTime)
    {
        transitionName.style.display = DisplayStyle.Flex;
        trasitionTypes.style.display = DisplayStyle.Flex;
        transitionName.RemoveFromClassList(transitionDescription);
        yield return new WaitForSeconds(waitTime);

        //StartCoroutine(onTransition(transitionName));
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator onCheckTransition(string sceneName, VisualElement transitionName, float waitTime)
    {
        transitionName.style.display = DisplayStyle.Flex;
        trasitionTypes.style.display = DisplayStyle.Flex;
        transitionName.RemoveFromClassList(transitionDescription);
        yield return new WaitForSeconds(waitTime);

        StartCoroutine(onCheckTransition(transitionName));
        //SceneManager.LoadScene(sceneName);
    }

    IEnumerator onTransition(VisualElement transitionName)
    {
        yield return new WaitForSeconds(0.5f);
        transitionName.AddToClassList(transitionDescription);
        yield return new WaitForSeconds(1);
        transitionName.style.display = DisplayStyle.None;
        trasitionTypes.style.display = DisplayStyle.None;
        isTrasitioning = false;
    }

    IEnumerator onCheckTransition(VisualElement transitionName)
    {
        yield return new WaitForSeconds(0.5f);
        transitionName.AddToClassList(transitionDescription);
        PlayerController tempPlayer = FindObjectOfType<PlayerController>();
        if (CheckPointScript.checkpointPosition != Vector3.zero)
        {
            tempPlayer.transform.position = CheckPointScript.checkpointPosition;
            GameManager.instance.UpdateState(GameState.RACING);
            // Debug.Log("Player position set to checkpoint: " + CheckPointScript.checkpointPosition);
        }
        yield return new WaitForSeconds(1);
        transitionName.style.display = DisplayStyle.None;
        trasitionTypes.style.display = DisplayStyle.None;

        isTrasitioning = false;
    }
}
