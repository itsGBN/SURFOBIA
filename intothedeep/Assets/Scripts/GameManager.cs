using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { MAIN, READY, PAUSED, COUNTING, RACING, ENDGAME, CREDITS }

    [SerializeField] GameState startingState = GameState.READY;

    GameState gameState = GameState.MAIN;
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
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
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
        
        //UpdateState(GameState.MAIN);
    }

    // Update is called once per frame
    void Update()
    {
        //Time.timeScale = globalTimeScale;

        switch (gameState)
        {
            case GameState.ENDGAME:
                if (Input.anyKeyDown)
                {
                    // go to main menu
                    SceneManager.LoadScene(0);
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    UpdateState(GameState.MAIN);
                    MusicManager.instance.FadeOut();
                }
            break;
            case GameState.MAIN:
                if (Input.anyKeyDown)
                {
                    //UpdateState(GameState.READY);
                }
                break;
        }

        if (GetInputs.PS5Map.Restart.WasPressedThisFrame())
        {
            UpdateState(GameState.READY);
            MusicManager.instance.FadeOut();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void UpdateState(GameState newState)
    {
        lastState = gameState;
        gameState = newState;

        // TODO FIX LOOPING INPUT

        switch (gameState)
        {
            case GameState.MAIN:
                playerInput = true;
                Time.timeScale = 1f;
                break;
            case GameState.READY:
                playerInput = true;
                Time.timeScale = 1f;
                break;
            case GameState.PAUSED:
                playerInput = true;
                Time.timeScale = 0f;
                //globalTimeScale = 0f;
                //pauseMenu.SetActive(true);
                break;
            case GameState.COUNTING:
                playerInput = true;
                StartCount();
                break;
            case GameState.RACING:
                playerInput = true;
                Time.timeScale = 1f;
                break;
            case GameState.ENDGAME:
                playerInput = true;
                HUD.instance.Endscreen();
                break;
        }

        Debug.Log("Game state updated: " + gameState.ToString());
    }

    public void PauseGame()
    {
        UpdateState(GameState.PAUSED);
    }

    public void UnpauseGame()
    {
        if (gameState == GameState.READY) { UpdateState(GameState.COUNTING); }
        else { UpdateState(lastState); }
    }

    void StartCount()
    {
        countdownTimer = countdownTime;
        HUD.instance.UpdateCountdown(countdownTimer);
        StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        while (countdownTimer > 0)
        {
            yield return new WaitForSeconds(1);
            countdownTimer--;
            HUD.instance.UpdateCountdown(countdownTimer);

            if (countdownTimer <= 0)
            {
                // Start game
                UpdateState(GameState.RACING);
                MusicManager.instance.StartTrack();
            }
        }
    }

    public void FreezeFrame(float time = 1f)
    {
        StartCoroutine(Freeze(time));
    }

    IEnumerator Freeze(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }
}
