using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishColorRandomizer : MonoBehaviour
{
    private Renderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer component not found on this object.");
            return;
        }

        ChangeMaterialColor();
    }

    public void ChangeMaterialColor()
    {
        // Generate a random color using HSV
        Color randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        // Apply the random color to the material
        if (_renderer != null && _renderer.material != null)
        {
            _renderer.material.color = randomColor;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
