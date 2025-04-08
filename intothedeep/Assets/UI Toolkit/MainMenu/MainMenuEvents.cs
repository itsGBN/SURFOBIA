using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    public static MainMenuEvents instance;

    private UIDocument uIDocument;

    private List<Button> allButtons = new List<Button>();
    private Button levelButton;
    private List<Button> levelChildrenButtons = new List<Button>();
    private List<VisualElement> allElements = new List<VisualElement>();

    private VisualElement fadeIn;
    private VisualElement squareIn;
    private VisualElement trasitionTypes;
    private VisualElement transitionName;
    private VisualElement mainMenu;
    private string transitionDescription;
    private bool isTrasitioning = true;
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
        //Singleton
        if(instance != null && instance != this) { Destroy(instance); }
        else { instance = this; }

        //Refernce UI 
        uIDocument = GetComponent<UIDocument>();
        allElements = uIDocument.rootVisualElement.Query<VisualElement>().ToList();

        //Reference Buttons
        levelButton = uIDocument.rootVisualElement.Q("LevelButton") as Button;
        allButtons = uIDocument.rootVisualElement.Query<Button>().ToList();
        levelChildrenButtons = uIDocument.rootVisualElement.Query<Button>(null, "levelChildren").ToList();

        //Reference Visual Elements
        trasitionTypes = uIDocument.rootVisualElement.Q("TransitionTypes") as VisualElement;
        fadeIn = uIDocument.rootVisualElement.Q("FadeIn") as VisualElement;
        squareIn = uIDocument.rootVisualElement.Q("SquareIn") as VisualElement;
        mainMenu = uIDocument.rootVisualElement.Q("MainButtons") as VisualElement;

        //Register
        levelButton.RegisterCallback<ClickEvent>(onPlayButton);
        foreach (Button button in allButtons) { button.RegisterCallback<ClickEvent>(onAllButtons); }
        foreach (Button button in levelChildrenButtons) { button.RegisterCallback<ClickEvent>(onPlayParentButtons); }

        //Miscelleaneous Things
        //Make Inactive Buttons Dissapear
        foreach (Button button in levelChildrenButtons) { if (button.ClassListContains("levelChildrenActive")) { button.AddToClassList("levelChildrenInactive"); } }
        
        //Add the Transition
        if (fadeIn.ClassListContains("fadeOut") && chooseTransition == ChooseTransition.FadeIn) { fadeIn.RemoveFromClassList("fadeOut"); transitionName = fadeIn; transitionDescription = "fadeOut"; }
        if (squareIn.ClassListContains("squareOut") && chooseTransition == ChooseTransition.SqaureIn) { squareIn.RemoveFromClassList("squareOut"); transitionName = squareIn; transitionDescription = "squareOut"; }

        //Get AudioSource
        buttonSound = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        //Deregister
        levelButton.UnregisterCallback<ClickEvent>(onPlayButton);
        foreach (Button button in allButtons) { button.UnregisterCallback<ClickEvent>(onAllButtons); }
        foreach (Button button in levelChildrenButtons) { button.UnregisterCallback<ClickEvent>(onPlayParentButtons); }
    }

    private void Start()
    {
        StartCoroutine(onTransition(transitionName));
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isTrasitioning)
        {
            if (mainMenu.ClassListContains("menuInactive")) { mainMenu.RemoveFromClassList("menuInactive"); focusMenu = true; Time.timeScale = 0; }
            else { mainMenu.AddToClassList("menuInactive"); focusMenu = false; Time.timeScale = 1; }
        }
    }

    private void onAllButtons(ClickEvent e)
    {
        //buttonSound.Play();
    }

    //Play Button
    private void onPlayButton(ClickEvent e)
    {
        foreach (Button button in levelChildrenButtons)
        {
            if (button.ClassListContains("levelChildrenInactive")) { button.RemoveFromClassList("levelChildrenInactive"); }
            else { button.AddToClassList("levelChildrenInactive"); }
        }
    }

    private void onPlayParentButtons(ClickEvent e)
    {
        Time.timeScale = 1;
        isTrasitioning = true;
        StartCoroutine(onTransition(SceneManager.GetActiveScene().name, transitionName));
    }

    IEnumerator onTransition(string sceneName, VisualElement transitionName)
    {
        transitionName.style.display = DisplayStyle.Flex;
        trasitionTypes.style.display = DisplayStyle.Flex;
        transitionName.RemoveFromClassList(transitionDescription);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator onTransition(VisualElement transitionName)
    {
        yield return new WaitForSeconds(0.5f);
        transitionName.AddToClassList(transitionDescription);
        yield return new WaitForSeconds(1);
        transitionName.style.display = DisplayStyle.None;
        trasitionTypes.style.display = DisplayStyle.None;
        Time.timeScale = 0;
        isTrasitioning = false;
    }
}
