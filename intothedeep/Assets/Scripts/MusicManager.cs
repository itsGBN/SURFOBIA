using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip[] tracks;
    [SerializeField] float fadeSpeed = 1f;

    bool playing = false;
    int trackIndex = 0;
    float defaultVolume;

    AudioSource source;

    public static MusicManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        source = GetComponent<AudioSource>();
        defaultVolume = source.volume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTrack(int index = 0)
    {
        source.volume = defaultVolume;
        playing = true;
        trackIndex = index;

        source.clip = tracks[trackIndex];
        source.Play();
    }

    public void NextTrack()
    {

    }

    public void FadeOut()
    {
        StartCoroutine(AudioFadeOut());
    }

    IEnumerator AudioFadeOut()
    {
        while (source.volume > 0)
        {
            yield return null;
            source.volume -= fadeSpeed * Time.deltaTime;
        }
        source.volume = 0;
    }
}
