using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioHighPassFilter filter;
    AudioLowPassFilter lowFilter;
    [SerializeField] GameObject player;

    bool isGrinding;

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<AudioHighPassFilter>();
        lowFilter = GetComponent<AudioLowPassFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (!player.GetComponent<PlayerController>().isGrounded)
        {
            if(filter.cutoffFrequency != 4000) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 4000, 3 * Time.deltaTime);
        } else
        {
            if (filter.cutoffFrequency != 10) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 10, 3 * Time.deltaTime);
        }
        

        isGrinding = player.GetComponent<PlayerController>().currentState == player.GetComponent<PlayerController>().grindState;

        if (isGrinding)
        {
            if (lowFilter.cutoffFrequency != 5000) lowFilter.cutoffFrequency = Mathf.Lerp(lowFilter.cutoffFrequency, 5000, 3 * Time.deltaTime);
        } else
        {
            if (lowFilter.cutoffFrequency != 22000) lowFilter.cutoffFrequency = Mathf.Lerp(lowFilter.cutoffFrequency, 22000, 3 * Time.deltaTime);
        }
        
    }
}
