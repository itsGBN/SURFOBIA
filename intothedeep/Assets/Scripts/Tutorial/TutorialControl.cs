using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialControl : MonoBehaviour
{
    [SerializeField] Image mapImage;
    [SerializeField] Animator anim;
    [SerializeField] Image blackout;

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
        SetMap();
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

            if (GetInputs.PS5Map.Jump.WasPressedThisFrame())
            {
                Fall();
            }
        }
    }

    public void SetMap()
    {
        if (GameManager.INPUT_CONTROLLER) { anim.SetTrigger("controller"); }
        else { anim.SetTrigger("keyboard"); }
        mapImage.SetNativeSize();
    }

    private void OnTriggerEnter(Collider other)
    {
        chart.SendFungusMessage("freefall");
        falling = true;
        FindObjectOfType<PlayerController>().SetState(FindObjectOfType<PlayerController>().freefallState);
        Destroy(GetComponent<Collider>());
    }

    public void StartMovement()
    {
        GameManager.instance.UpdateState(GameManager.GameState.RACING);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Leve1");
        GameManager.instance.UpdateState(GameManager.GameState.READY);
    }

    public void Fall()
    {
        FindObjectOfType<PlayerController>().SetState(FindObjectOfType<PlayerController>().freeRoamState);
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float a = 0;
        while (a < 1)
        {
            a += 1.2f * Time.deltaTime;
            blackout.color = new Color(blackout.color.r, blackout.color.g, blackout.color.b, a);
            yield return null;
        }
        LoadMenu();
    }
}
