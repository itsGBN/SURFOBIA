using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CreditsScene : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 10f;
    [SerializeField] RectTransform container;
    [SerializeField] int menuSceneIndex = 1;

    float scrollMultiply = 1;

    private PS5Input GetInputs;

    void Awake()
    {
        GetInputs = new PS5Input();
    }

    private void OnEnable()
    {
        GetInputs.Enable();
    }

    private void OnDisable()
    {
        GetInputs.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        container.anchoredPosition += new Vector2(0, scrollSpeed * scrollMultiply * Time.deltaTime);
        if (container.anchoredPosition.y > 1500)
        {
            SceneManager.LoadScene(menuSceneIndex);
        }

        if (GetInputs.PS5Map.Menu.WasPressedThisFrame())
        {
            SceneManager.LoadScene(menuSceneIndex);
        }

        if (GetInputs.PS5Map.Jump.WasPressedThisFrame()) { scrollMultiply = 2.75f; }
        if (GetInputs.PS5Map.Jump.WasReleasedThisFrame()) { scrollMultiply = 1; }
    }
}
