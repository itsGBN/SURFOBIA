using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { MENU, PAUSED, COUNTING, RACING, ENDGAME }

    GameState gameState = GameState.COUNTING;
    GameState lastState;

    float globalTimeScale = 1f;
    bool playerInput = true; // can we control the character?

    [SerializeField] float countdownTime = 3;

    [Header("UI Elements")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject endScreen;

    public static GameManager instance;

    // PROPERTY GETTERS
    public bool InputActive { get { return playerInput; } }
    public float GlobalTimeScale { get { return globalTimeScale; } }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) { instance = this; }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = globalTimeScale;
    }

    void UpdateState(GameState newState)
    {
        switch (gameState)
        {
            case GameState.PAUSED:
                globalTimeScale = 1f;
                pauseMenu.SetActive(false);
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
        UpdateState(lastState);
    }

    void StartCount()
    {
        StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        yield return new WaitForSeconds(countdownTime);
        UpdateState(GameState.RACING);
    }
}
