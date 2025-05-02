using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScene : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 10f;
    [SerializeField] RectTransform text;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }
}
