using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioHighPassFilter filter;
    AudioLowPassFilter lowFilter;
    [SerializeField] GameObject player;
    [SerializeField] LayerMask layerMask;

    bool isGrinding;
    bool isCloseToGround;

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<AudioHighPassFilter>();
        lowFilter = GetComponent<AudioLowPassFilter>();
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        isCloseToGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3.5f, layerMask);


        if (!isCloseToGround)
        {
            if (filter.cutoffFrequency != 4000) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 4000, 2 * Time.deltaTime);
        } else
        {
            if (filter.cutoffFrequency != 10) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 10, 12 * Time.deltaTime);
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
