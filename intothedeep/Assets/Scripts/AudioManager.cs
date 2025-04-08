using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioSource[] Audio;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(this); }
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
}
