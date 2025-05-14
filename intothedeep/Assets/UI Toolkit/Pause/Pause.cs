using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UIElements;
using static GameManager;
using static System.TimeZoneInfo;

public class Pause : MonoBehaviour
{
    //singleton
    public static Pause instance;

    //uiDocument and all elements
    private UIDocument uIDocument;
    private List<VisualElement> allElements = new List<VisualElement>();

    //buttons
    private List<Button> allButtons = new List<Button>();
    private Button mainMenuButton;
    private Button quitButton;
    private Button resumeButton;
    private List<Button> dpadButtons = new List<Button>();
    private int dpadnum = 0;


    //visualelement
    private VisualElement puase;

    //Action Map
    private PS5Input GetInputs;

    //Audio
    private AudioSource buttonSound;

    bool isResume;

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
        mainMenuButton = uIDocument.rootVisualElement.Q("MainMenu") as Button;
        quitButton = uIDocument.rootVisualElement.Q("Quit") as Button;
        resumeButton = uIDocument.rootVisualElement.Q("Resume") as Button;
        allButtons = uIDocument.rootVisualElement.Query<Button>().ToList();
        dpadButtons = uIDocument.rootVisualElement.Query<Button>(null, "dpadbuttons").ToList();

        //Refernce Visual Elements
        puase = uIDocument.rootVisualElement.Q("Background") as VisualElement;

        //Register
        mainMenuButton.RegisterCallback<ClickEvent>(onMainMenuButton);
        quitButton.RegisterCallback<ClickEvent>(onQuitButton);
        resumeButton.RegisterCallback<ClickEvent>(onResumeButton);
        foreach (Button button in allButtons) { button.RegisterCallback<ClickEvent>(onAllButtons); }

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
        mainMenuButton.UnregisterCallback<ClickEvent>(onMainMenuButton);
        quitButton.UnregisterCallback<ClickEvent>(onQuitButton);
        resumeButton.UnregisterCallback<ClickEvent>(onResumeButton);
        foreach (Button button in allButtons) { button.UnregisterCallback<ClickEvent>(onAllButtons); }
    }

    private void onMainMenuButton(ClickEvent e)
    {
        GameManager.instance.UpdateState(GameState.READY);
        StartCoroutine(MainMenuEvents.instance.onTransition(SceneManager.GetActiveScene().name, MainMenuEvents.instance.transitionName, 1f));
    }

    private void onAllButtons(ClickEvent e)
    {
        buttonSound.Play();
    }

    private void Update()
    {
        if ((GetInputs.PS5Map.Menu.WasPressedThisFrame() || isResume) && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.RACING))
        {
            if (puase.ClassListContains("inactive"))
            {
                puase.RemoveFromClassList("inactive"); MainMenuEvents.instance.focusMenu = true; Time.timeScale = 0;
            }
            else
            {
                switch (dpadnum)
                {
                    case 0:
                        puase.AddToClassList("inactive"); MainMenuEvents.instance.focusMenu = false; Time.timeScale = 1; 
                        break;
                    case 1:
                        GameManager.instance.gameState = GameManager.GameState.READY;
                        SceneManager.LoadScene("Leve1");
                        break;
                    case 2:
                        SceneManager.LoadScene("Credits");
                        Time.timeScale = 1;
                        break;
                    case 3:
                        Debug.Log("Application Quit");
                        Application.Quit();
                        break;
                }
            }
        }

        if (GetInputs.PS5Map.MenuRight.WasPressedThisFrame() && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.RACING))
        {
            dpadButtons[dpadnum].RemoveFromClassList("buttonhover");
            dpadnum += 1;
            if (dpadnum > 3) { dpadnum = 0; }
            dpadButtons[dpadnum].AddToClassList("buttonhover");
            //print(dpadnum);

        }
        if (GetInputs.PS5Map.MenuLeft.WasPressedThisFrame() && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.RACING))
        {
            dpadButtons[dpadnum].RemoveFromClassList("buttonhover");
            dpadnum -= 1;
            if (dpadnum < 0) { dpadnum = 3; }
            dpadButtons[dpadnum].AddToClassList("buttonhover");
            //print(dpadnum);

        }
    }

    private void onQuitButton(ClickEvent e)
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    private void onResumeButton(ClickEvent e)
    {
        Debug.Log("Resume");
        puase.AddToClassList("inactive"); MainMenuEvents.instance.focusMenu = false; Time.timeScale = 1;

        //isResume = true;
    }
}
