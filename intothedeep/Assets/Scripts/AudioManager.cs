using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioSource[] Audio;
    [SerializeField] AudioSource musicNoDrums;
    [SerializeField] AudioSource drumsOnly;

    [SerializeField] GameObject player;

    bool isGrounded;
    bool isGrinding;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(this); }
    }

    private void Update()
    {
        isGrounded = player.GetComponent<PlayerController>().isGrounded;
        isGrinding = player.GetComponent<PlayerController>().currentState == player.GetComponent<PlayerController>().grindState;


        if (isGrinding)
        {
            drumsOnly.volume = 1.0f;
        } else if(drumsOnly.volume != 0.6f)
        {
            musicNoDrums.volume = 0.6f;
        }

        if (isGrounded)
        {
            if (drumsOnly.volume != 1) drumsOnly.volume = 0.6f;
        }
        else
        {
            drumsOnly.volume = 0.2f;
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
