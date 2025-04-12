using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] Animator mantaAnim;
    [SerializeField] Animator skeletonAnim;
    [SerializeField] Animator trickSkeletonAnim;


    #region CONTROLLER
    private PS5Input GetInputs;

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
    #endregion CONTROLLER


    // Start is called before the first frame update
    void Start()
    {
        trickSkeletonAnim.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = GetInputs.PS5Map.Move.ReadValue<Vector2>().y;

        skeletonAnim.SetFloat("Joystick", moveInput);
        mantaAnim.SetFloat("Joystick", moveInput);

    }

    public void DoTrick(string trickName)
    {
        skeletonAnim.gameObject.SetActive(false);
        trickSkeletonAnim.gameObject.SetActive(true);
        trickSkeletonAnim.SetTrigger(trickName);

        StartCoroutine(trickTimer());

        //Debug.Log(trickName + "inanimanager");
    }

    IEnumerator trickTimer()
    {
        yield return new WaitForSeconds(2f);

        skeletonAnim.gameObject.SetActive(true);
        trickSkeletonAnim.gameObject.SetActive(false);
    }
}
