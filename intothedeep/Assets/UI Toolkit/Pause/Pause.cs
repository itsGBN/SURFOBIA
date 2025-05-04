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

    //visualelement
    private VisualElement puase;

    //Action Map
    private PS5Input GetInputs;

    //Audio
    private AudioSource buttonSound;

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
        allButtons = uIDocument.rootVisualElement.Query<Button>().ToList();

        //Refernce Visual Elements
        puase = uIDocument.rootVisualElement.Q("Background") as VisualElement;

        //Register
        mainMenuButton.RegisterCallback<ClickEvent>(onMainMenuButton);
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
        if (GetInputs.PS5Map.Menu.WasPressedThisFrame() && !MainMenuEvents.instance.isTrasitioning && (GameManager.instance.gameState == GameManager.GameState.RACING))
        {
            if (puase.ClassListContains("inactive")) { puase.RemoveFromClassList("inactive"); MainMenuEvents.instance.focusMenu = true; Time.timeScale = 0; }
            else { puase.AddToClassList("inactive"); MainMenuEvents.instance.focusMenu = false; Time.timeScale = 1; }
        }
    }
}
