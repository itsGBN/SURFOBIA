using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenControl : MonoBehaviour
{
    [SerializeField] int sceneIndex = 1;
    private PS5Input GetInputs;
    private PlayableDirector director;

    bool playing = false;

    private void Awake()
    {
        GetInputs = new PS5Input();
        director = FindObjectOfType<PlayableDirector>();
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
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO convert to new input system
        if (Input.anyKeyDown && !playing)
        {
            playing = true;
            director.Play();
        }
    }

    public void LoadGame()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float alpha = 100;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneIndex);
    }
}
