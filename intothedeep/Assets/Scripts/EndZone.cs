using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            StartCoroutine(EndGame());
            HighScore.instance.scoreActive = true;
            other.GetComponent<PlayerController>().enabled = false;
        }
    }

    //create endgame coroutine
    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.UpdateState(GameManager.GameState.ENDGAME);
    }
}
