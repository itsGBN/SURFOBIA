using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioSource[] Audio;
    [SerializeField] AudioHighPassFilter filter;
    [SerializeField] AudioLowPassFilter lowFilter;
    [SerializeField] LayerMask layerMask;

    [SerializeField] GameObject player;

    bool isGrounded;
    bool isGrinding;
    bool isCloseToGround;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(this); }
    }

    private void Update()
    {
        isGrounded = player.GetComponent<PlayerController>().isGrounded;
        isGrinding = player.GetComponent<PlayerController>().currentState == player.GetComponent<PlayerController>().grindState;

        RaycastHit hit;
        isCloseToGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3.5f, layerMask);


        if (!isCloseToGround)
        {
            if (filter.cutoffFrequency != 3000) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 3000, 2 * Time.deltaTime);
        }
        else
        {
            if (filter.cutoffFrequency != 10) filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, 10, 12 * Time.deltaTime);
        }


        isGrinding = player.GetComponent<PlayerController>().currentState == player.GetComponent<PlayerController>().grindState;

        if (isGrinding)
        {
            if (lowFilter.cutoffFrequency != 5000) lowFilter.cutoffFrequency = Mathf.Lerp(lowFilter.cutoffFrequency, 5000, 3 * Time.deltaTime);
        }
        else
        {
            if (lowFilter.cutoffFrequency != 22000) lowFilter.cutoffFrequency = Mathf.Lerp(lowFilter.cutoffFrequency, 22000, 3 * Time.deltaTime);
        }

    }

    // Play the sound of the Player Walking
    public void Land() { if (!Audio[0].isPlaying) { Audio[0].pitch = Random.Range(0.8f, 1.2f); Audio[0].Play(); } }
    public void Run() { if (!Audio[1].isPlaying) { Audio[1].pitch = Random.Range(0.8f, 1.2f); Audio[1].Play(); } }
    public void RunStop() { Audio[1].Stop(); }
    public void Jump() { if (!Audio[2].isPlaying) { Audio[2].pitch = Random.Range(0.8f, 1.2f); Audio[2].Play(); } }
    public void BadLand() { if (!Audio[3].isPlaying) { Audio[3].pitch = Random.Range(0.8f, 1.2f); Audio[3].Play(); } }
    public void GoodLand() { if (!Audio[4].isPlaying) { Audio[4].pitch = Random.Range(0.8f, 1.2f); Audio[4].Play(); } }
    public void Grind() { if (!Audio[5].isPlaying) { Audio[5].pitch = Random.Range(0.8f, 1.2f); Audio[5].Play(); } }
    public void GrindStop() { Audio[5].Stop(); }
    public void Hit() { if (!Audio[6].isPlaying) { Audio[6].pitch = Random.Range(0.8f, 1.2f); Audio[6].Play(); } }
    public void Pop() { if (!Audio[7].isPlaying) { Audio[7].pitch = Random.Range(0.8f, 1.2f); Audio[7].Play(); } }
}
