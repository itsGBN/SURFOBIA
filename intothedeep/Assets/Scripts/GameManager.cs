using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { MAIN, READY, PAUSED, COUNTING, RACING, ENDGAME, CREDITS, TUTORIAL }

    [SerializeField] GameState startingState = GameState.READY;

    [HideInInspector] public GameState gameState = GameState.MAIN;
    GameState lastState;

    float globalTimeScale = 1f;
    bool playerInput = true; // can we control the character?

    [SerializeField] float countdownTime = 3;
    float countdownTimer;

    public static GameManager instance;

    private float savedIdleFloat;
    
    public PlayableDirector openingCutscene;
    public Image openingCutsceneImage;

    public static bool INPUT_CONTROLLER = true; // if false, keyboard

    // PROPERTY GETTERS
    public bool InputActive { get { return playerInput; } }
    public float GlobalTimeScale { get { return globalTimeScale; } }

    private PS5Input GetInputs;
    private void Awake()
    {
        GetInputs = new PS5Input();
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        UpdateState(startingState);
    }

    private void OnEnable()
    {
        GetInputs.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GetInputs.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if (scene.name == "Leve1")
        {
            var cutDirObj = GameObject.Find("CutsceneDirector");
            openingCutscene = cutDirObj ? cutDirObj.GetComponent<PlayableDirector>() : null;

            var cutImgObj = GameObject.Find("cutsceneTransition");
            openingCutsceneImage = cutImgObj ? cutImgObj.GetComponent<Image>() : null;
            if (gameState == GameState.READY && openingCutscene != null)
                    openingCutscene.Play();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //openingCutscene = GameObject.Find("CutsceneDirector").GetComponent<PlayableDirector>();
        //openingCutsceneImage= GameObject.Find("cutsceneTransition").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Time.timeScale = globalTimeScale;

        // complete reset
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(0);
            //GameManager.instance.UpdateState(GameManager.GameState.MAIN);
            Destroy(this);
        }

        switch (gameState)
        {
            case GameState.ENDGAME:
                if (Input.anyKeyDown)
                {
                    // go to main menu
                    //SceneManager.LoadScene(0);
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    //UpdateState(GameState.MAIN);
                    //MusicManager.instance.FadeOut();
                }
                break;
            case GameState.MAIN:
                if (Input.anyKeyDown)
                {
                    //UpdateState(GameState.READY);
                }
                break;
        }

        if (GetInputs.PS5Map.Restart.WasPressedThisFrame() && MainMenuEvents.instance.focusMenu == true)
        {
            
            UpdateState(GameState.READY);
            CheckPointScript.RestartCheckpoint();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void UpdateState(GameState newState)
    {
        lastState = gameState;
        gameState = newState;
        Debug.Log("Game state changed: " + gameState.ToString());

        // TODO FIX LOOPING INPUT

        switch (gameState)
        {
            case GameState.MAIN:
                playerInput = false;
                Time.timeScale = 1f;
                break;
            case GameState.READY:
                {
                    PlayerController tempPlayer = FindObjectOfType<PlayerController>();
                    if (CheckPointScript.checkpointPosition != Vector3.zero){
                        tempPlayer.gameObject.transform.position = CheckPointScript.checkpointPosition;
                        tempPlayer.gameObject.transform.rotation = CheckPointScript.checkpointRotation;
                        Debug.Log("Player position set to checkpoint: " + CheckPointScript.checkpointPosition);
                    }
                    playerInput = false;
                    Time.timeScale = 1f;
                    break;
                }
            case GameState.PAUSED:
                playerInput = false;
                Time.timeScale = 0f;
                //globalTimeScale = 0f;
                //pauseMenu.SetActive(true);
                break;
            case GameState.COUNTING:
                {
                    PlayerController tempPlayer = FindObjectOfType<PlayerController>();
                    if (tempPlayer != null)
                    {
                        savedIdleFloat = tempPlayer.idleFloat;
                        tempPlayer.idleFloat = 0;
                    }

                    playerInput = false;
                    if(openingCutscene != null) { openingCutscene.Stop(); }
                    if(openingCutsceneImage != null) { openingCutsceneImage.color = new Color(1f, 1f, 1f, 0f); }
                    StartCount();
                    break;
                }
            case GameState.RACING:
                {
                    PlayerController tempPlayer = FindObjectOfType<PlayerController>();
                    if (tempPlayer != null && lastState==GameState.COUNTING)
                    {
                        tempPlayer.idleFloat = savedIdleFloat;
                    }

                    playerInput = true;
                    Time.timeScale = 1f;
                    break;
                }
            case GameState.ENDGAME:
                playerInput = false;
                HUD.instance.Endscreen();
                CheckPointScript.RestartCheckpoint();
                break;
            case GameState.TUTORIAL:
                {
                    PlayerController tempPlayer = FindObjectOfType<PlayerController>();
                    if (tempPlayer != null)
                    {
                        savedIdleFloat = tempPlayer.idleFloat;
                        tempPlayer.idleFloat = 0;
                    }
                    playerInput = false;
                    Time.timeScale = 1f;
                    break;
                }
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
                //MusicManager.instance.StartTrack();
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
    
    public static void SetInputType(bool controller)
    {
        INPUT_CONTROLLER = controller;
    }
}
