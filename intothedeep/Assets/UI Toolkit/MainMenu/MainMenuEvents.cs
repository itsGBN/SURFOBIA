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
    private List<Button> levelChildrenButtons = new List<Button>();
    private List<VisualElement> allElements = new List<VisualElement>();

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
        tutorialButton = uIDocument.rootVisualElement.Q("Turtorial") as Button;
        inputButton = uIDocument.rootVisualElement.Q("Input") as Button;
        quitButton = uIDocument.rootVisualElement.Q("Quit") as Button;
        allButtons = uIDocument.rootVisualElement.Query<Button>().ToList();
        levelChildrenButtons = uIDocument.rootVisualElement.Query<Button>(null, "levelChildren").ToList();

        //Reference Visual Elements
        trasitionTypes = uIDocument.rootVisualElement.Q("TransitionTypes") as VisualElement;
        fadeIn = uIDocument.rootVisualElement.Q("FadeIn") as VisualElement;
        squareIn = uIDocument.rootVisualElement.Q("SquareIn") as VisualElement;
        mainMenu = uIDocument.rootVisualElement.Q("MainButtons") as VisualElement;
        inputPicture = uIDocument.rootVisualElement.Q("InputPicture") as VisualElement;

        //Register
        levelButton.RegisterCallback<ClickEvent>(onPlayButton);
        level1Button.RegisterCallback<ClickEvent>(onPlayParentButtons);
        // IDK WHY but registering these buttons is breaking the game start
        //tutorialButton.RegisterCallback<ClickEvent>(onTutorialButton);
        //inputButton.RegisterCallback<ClickEvent>(onInputButton);
        quitButton.RegisterCallback<ClickEvent>(onQuitButton);
        foreach (Button button in allButtons) { button.RegisterCallback<ClickEvent>(onAllButtons); }
        //Miscelleaneous Things
        //Make Inactive Buttons Dissapear
        foreach (Button button in levelChildrenButtons) { if (button.ClassListContains("levelChildrenActive")) { button.AddToClassList("levelChildrenInactive"); } }

        //Add the Transition
        if (fadeIn.ClassListContains("fadeOut") && chooseTransition == ChooseTransition.FadeIn) { fadeIn.RemoveFromClassList("fadeOut"); transitionName = fadeIn; transitionDescription = "fadeOut"; }
        if (squareIn.ClassListContains("squareOut") && chooseTransition == ChooseTransition.SqaureIn) { squareIn.RemoveFromClassList("squareOut"); transitionName = squareIn; transitionDescription = "squareOut"; }

        //Get AudioSource
        buttonSound = GetComponent<AudioSource>();
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
        inputButton.UnregisterCallback<ClickEvent>(onInputButton);
        quitButton.UnregisterCallback<ClickEvent>(onQuitButton);
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
            //Debug.Log("check"); BUG THIS IF STATEMENT ISNT RUNNING WHEN RELOADING FROM ENTIRE GAME LOOP (MAIN MENU)
            if (mainMenu.ClassListContains("menuInactive")) { mainMenu.RemoveFromClassList("menuInactive"); focusMenu = true; GameManager.instance.PauseGame(); }
            else { mainMenu.AddToClassList("menuInactive"); focusMenu = false; GameManager.instance.UnpauseGame(); }
        }
    }

    private void onAllButtons(ClickEvent e)
    {
        //buttonSound.Play();
    }

    //Play Button
    private void onPlayButton(ClickEvent e)
    {
        /**
        foreach (Button button in levelChildrenButtons)
        {
            if (button.ClassListContains("levelChildrenInactive")) { button.RemoveFromClassList("levelChildrenInactive"); }
            else { button.AddToClassList("levelChildrenInactive"); }
        }
        */
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
