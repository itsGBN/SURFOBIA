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
    [SerializeField] Image fadeout;
    [SerializeField] AudioClip[] sounds;
    private PS5Input GetInputs;
    private PlayableDirector director;

    bool playing = false;
    int soundIndex = 0;
    AudioSource source;

    private void Awake()
    {
        GetInputs = new PS5Input();
        director = FindObjectOfType<PlayableDirector>();
        source = GetComponent<AudioSource>();
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
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            Color fadeoutC = fadeout.color;
            fadeoutC.a = alpha;
            fadeout.color = fadeoutC;
            yield return null;
        }
        SceneManager.LoadScene(sceneIndex);
    }

    public void PlaySound()
    {
        source.PlayOneShot(sounds[soundIndex]);
        if (soundIndex < sounds.Length) { soundIndex++; }
    }
}
