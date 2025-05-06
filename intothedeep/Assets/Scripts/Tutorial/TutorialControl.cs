using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using UnityEngine.InputSystem;

public class TutorialControl : MonoBehaviour
{
    [SerializeField] Image mapImage;
    [SerializeField] Animator anim;

    Flowchart chart;

    bool falling = false;

    PS5Input GetInputs;

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        chart = FindObjectOfType<Flowchart>();
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            if (GetInputs.PS5Map.Menu.WasPressedThisFrame())
            {
                mapImage.gameObject.SetActive(!mapImage.gameObject.activeSelf);
            }
        }
    }

    public void SetMap()
    {
        if (GameManager.INPUT_CONTROLLER) { anim.SetTrigger("controller"); }
        else { anim.SetTrigger("keyboard"); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // disable player movement
        chart.SendFungusMessage("freefall");
        falling = true;
    }
}
