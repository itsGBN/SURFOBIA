using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] float volumeThreshold;
    [SerializeField] float changeSpeed;

    AudioSource music;
    bool isRaising;
    bool isLowering;
    bool hasStarted;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isRaising)
        {
            music.volume = Mathf.Lerp(music.volume, volumeThreshold, changeSpeed * Time.deltaTime);

            if(music.volume == volumeThreshold) isRaising = false;
        }

        if (isLowering)
        {
            music.volume = Mathf.Lerp(music.volume, 0, changeSpeed * 1 * Time.deltaTime);

            if (music.volume == 0) isLowering = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            if (!hasStarted) {
                music.Play();
                hasStarted = true;
            }
            isLowering = false;
            isRaising = true;
        } 
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            isLowering = true;
            isRaising = false;
        } 
    }
}
