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
    bool waiting = false;
    bool mapActive = false;

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
        anim.SetTrigger("controller");
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            if (GetInputs.PS5Map.Menu.WasPressedThisFrame())
            {
                mapActive = !mapActive;
                if (mapActive) { StartCoroutine(FadeIn(mapImage, 2.2f)); }
                else { StartCoroutine(FadeOut(mapImage, 2.2f)); }
            }

            if (GetInputs.PS5Map.TutorialForward.WasPressedThisFrame() && waiting)
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
        mapActive = true;
        StartCoroutine(FadeIn(mapImage, 2.2f));
    }

    private void OnTriggerEnter(Collider other)
    {
        chart.SendFungusMessage("freefall");
        falling = true;
        FindObjectOfType<PlayerController>().SetState(FindObjectOfType<PlayerController>().freefallState);
        Destroy(GetComponent<Collider>());
    }

    public void Wait()
    {
        waiting = true;
        StartCoroutine(MoveMap());
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
        StartCoroutine(FadeIn(blackout, 0.8f));
    }

    IEnumerator MoveMap()
    {
        while (mapImage.rectTransform.anchoredPosition.y > 0)
        {
            mapImage.rectTransform.anchoredPosition += new Vector2(0, -130 * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator FadeIn(Image image, float speed)
    {
        Debug.Log("Fade in");
        float a = 0;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        while (a < 1)
        {
            a += speed * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        if (image == blackout) { LoadMenu(); }
    }

    IEnumerator FadeOut(Image image, float speed)
    {
        Debug.Log("Fade out");
        float a = 1;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        while (a > 0)
        {
            a -= speed * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
}
