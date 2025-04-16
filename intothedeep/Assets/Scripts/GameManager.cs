using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { MENU, PAUSED, COUNTING, RACING, ENDGAME, CREDITS }

    GameState gameState = GameState.MENU;
    GameState lastState;

    float globalTimeScale = 1f;
    bool playerInput = true; // can we control the character?

    [SerializeField] float countdownTime = 3;
    float countdownTimer;

    [Header("UI Elements")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject endScreen;

    public static GameManager instance;

    // PROPERTY GETTERS
    public bool InputActive { get { return playerInput; } }
    public float GlobalTimeScale { get { return globalTimeScale; } }

    private PS5Input GetInputs;
    private void Awake()
    {
        GetInputs = new PS5Input();
    }

    private void OnEnable()
    {
        GetInputs.Enable();
    }

    private void OnDisable()
    {
        GetInputs.Disable();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); gameState = GameState.MENU; }
        else { Destroy(gameObject); }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = globalTimeScale;
        
        switch (gameState)
        {
            case GameState.COUNTING:
                
            break;
        }


        if (GetInputs.PS5Map.Restart.WasPressedThisFrame())
        {
            UpdateState(GameState.MENU);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void UpdateState(GameState newState)
    {
        // exit states
        switch (gameState)
        {
            case GameState.PAUSED:
                globalTimeScale = 1f;
                //pauseMenu.SetActive(false);
                break;
        }

        lastState = gameState;
        gameState = newState;

        switch (gameState)
        {
            case GameState.MENU:
                playerInput = false;
                break;
            case GameState.PAUSED:
                playerInput = false;
                globalTimeScale = 0f;
                //pauseMenu.SetActive(true);
                break;
            case GameState.COUNTING:
                playerInput = false;
                StartCount();
                break;
            case GameState.RACING:
                playerInput = true;
                break;
            case GameState.ENDGAME:
                playerInput = false;
                //endScreen.SetActive(true);
                // init information
                break;
        }
    }

    public void PauseGame()
    {
        UpdateState(GameState.PAUSED);
    }

    public void UnpauseGame()
    {
        if (gameState == GameState.MENU) { UpdateState(GameState.RACING); }
        else { UpdateState(lastState); }
    }

    void StartCount()
    {
        countdownTimer = countdownTime;
        StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        while (countdownTimer > 0)
        {
            yield return new WaitForSeconds(1);
            countdownTimer--;
            // TODO Update number in HUD

            if (countdownTimer <= 0)
            {
                // Start game
                UpdateState(GameState.RACING);
            }
        }
    }
}
