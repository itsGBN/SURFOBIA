using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioSource[] Audio; 
    [SerializeField] LayerMask layerMask;

    [Header("Music Triggers")]
    [SerializeField] GameObject musicTrigger1;
    [SerializeField] GameObject musicTrigger2;
    [SerializeField] GameObject musicTrigger3;

    [Header("GameObjects")]
    [SerializeField] GameObject player;

    bool isGrounded;
    bool isGrinding;
    bool isCloseToGround;
    bool isMutingGrind;

    //Components
    AudioHighPassFilter filter;
    AudioHighPassFilter filter2;
    AudioHighPassFilter filter3;
    AudioLowPassFilter lowFilter;
    AudioLowPassFilter lowFilter2;
    AudioLowPassFilter lowFilter3;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(this); }

        filter = musicTrigger1.GetComponent<AudioHighPassFilter>();
        lowFilter = musicTrigger1.GetComponent<AudioLowPassFilter>();

        filter2 = musicTrigger2.GetComponent<AudioHighPassFilter>();
        lowFilter2 = musicTrigger2.GetComponent<AudioLowPassFilter>();
        
        filter3 = musicTrigger3.GetComponent<AudioHighPassFilter>();
        lowFilter3 = musicTrigger3.GetComponent<AudioLowPassFilter>();
    }

    private void Update()
    {
        isGrounded = player.GetComponent<PlayerController>().isGrounded;
        isGrinding = player.GetComponent<PlayerController>().currentState == player.GetComponent<PlayerController>().grindState;

        RaycastHit hit;
        isCloseToGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3.5f, layerMask);


        if (!isCloseToGround)
        {
            if (filter.cutoffFrequency != 2000) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 2000, 1 * Time.deltaTime);
            if (filter2.cutoffFrequency != 2000) filter2.cutoffFrequency = Mathf.Lerp(filter2.cutoffFrequency, 2000, 1 * Time.deltaTime);
            if (filter3.cutoffFrequency != 2000) filter3.cutoffFrequency = Mathf.Lerp(filter3.cutoffFrequency, 2000, 1 * Time.deltaTime);
        }
        else
        {
            if (filter.cutoffFrequency != 10) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 10, 12 * Time.deltaTime);
            if (filter2.cutoffFrequency != 10) filter2.cutoffFrequency = Mathf.Lerp(filter2.cutoffFrequency, 10, 12 * Time.deltaTime);
            if (filter3.cutoffFrequency != 10) filter3.cutoffFrequency = Mathf.Lerp(filter3.cutoffFrequency, 10, 12 * Time.deltaTime);
        }


        isGrinding = player.GetComponent<PlayerController>().currentState == player.GetComponent<PlayerController>().grindState;

        if (isGrinding)
        {
            if (lowFilter.cutoffFrequency != 5000) lowFilter.cutoffFrequency = Mathf.Lerp(lowFilter.cutoffFrequency, 5000, 3 * Time.deltaTime);
            if (lowFilter2.cutoffFrequency != 5000) lowFilter2.cutoffFrequency = Mathf.Lerp(lowFilter2.cutoffFrequency, 5000, 3 * Time.deltaTime);
            if (lowFilter3.cutoffFrequency != 5000) lowFilter3.cutoffFrequency = Mathf.Lerp(lowFilter3.cutoffFrequency, 5000, 3 * Time.deltaTime);

            if (!Audio[5].isPlaying) { 
                Audio[5].pitch = Random.Range(0.8f, 1.2f);
                Audio[5].volume = 1;
                Audio[5].Play();
            }

        }
        else
        {
            if (lowFilter.cutoffFrequency != 22000) lowFilter.cutoffFrequency = Mathf.Lerp(lowFilter.cutoffFrequency, 22000, 3 * Time.deltaTime);
            if (lowFilter2.cutoffFrequency != 22000) lowFilter2.cutoffFrequency = Mathf.Lerp(lowFilter2.cutoffFrequency, 22000, 3 * Time.deltaTime);
            if (lowFilter3.cutoffFrequency != 22000) lowFilter3.cutoffFrequency = Mathf.Lerp(lowFilter3.cutoffFrequency, 22000, 3 * Time.deltaTime);

            if (Audio[5].isPlaying)
            {
                Audio[5].volume = Mathf.Lerp(Audio[5].volume, 0, 2 * Time.deltaTime);
                if (Audio[5].volume < 0.01f) Audio[5].Stop();
            }
        }

    }

    // Play the sound of the Player Walking
    public void Land() { if (!Audio[0].isPlaying) { Audio[0].pitch = Random.Range(0.8f, 1.2f); Audio[0].Play(); } }
    public void Run() { if (!Audio[1].isPlaying) { Audio[1].pitch = Random.Range(0.8f, 1.2f); Audio[1].Play(); } }
    public void RunStop() { Audio[1].Stop(); }
    public void Jump() { if (!Audio[2].isPlaying) { Audio[2].pitch = Random.Range(0.8f, 1.2f); Audio[2].Play(); } }
    public void BadLand() { if (!Audio[3].isPlaying) { Audio[3].pitch = Random.Range(0.8f, 1.2f); Audio[3].Play(); } }
    public void GoodLand() { if (!Audio[4].isPlaying) { Audio[4].pitch = Random.Range(0.8f, 1.2f); Audio[4].Play(); } }
    public void Grind() { /*if (!Audio[5].isPlaying) { Audio[5].pitch = Random.Range(0.8f, 1.2f); isMutingGrind = false; Audio[5].volume = 1; Audio[5].Play(); } */ }
    public void GrindStop() { /*isMutingGrind = true;*/ }
    public void Hit() { if (!Audio[6].isPlaying) { Audio[6].pitch = Random.Range(0.8f, 1.2f); Audio[6].Play(); } }
    public void Pop() { if (!Audio[7].isPlaying) { Audio[7].pitch = Random.Range(0.8f, 1.2f); Audio[7].Play(); } }
    public void Trick() { 
        if (!Audio[8].isPlaying) { 
            //Audio[8].pitch = Random.Range(0.8f, 1.2f); 
            Audio[8].Play(); 
        } 
    }

    public void Boost()
    {
        if (!Audio[9].isPlaying)
        {
            //Audio[8].pitch = Random.Range(0.8f, 1.2f); 
            Audio[9].Play();
        }
    }


}
