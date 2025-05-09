using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScene : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 10f;
    [SerializeField] RectTransform container;
    [SerializeField] int menuSceneIndex = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        container.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        if (container.anchoredPosition.y > 1500)
        {
            SceneManager.LoadScene(menuSceneIndex);
        }
    }
}
