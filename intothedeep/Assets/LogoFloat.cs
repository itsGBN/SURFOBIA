using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoFloat : MonoBehaviour
{
    [SerializeField] private float amplitude = 20f;   //vertical movement range
    [SerializeField] private float frequency = 1f;    //self explanatory lol
    [SerializeField] private float period = 20f;   //horizontal movement range

    private RectTransform rectTransform;
    private Vector2 startPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * frequency) * period;
        float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
        rectTransform.anchoredPosition = new Vector2(startPos.x + offsetX, startPos.y + offsetY);
    }
}
