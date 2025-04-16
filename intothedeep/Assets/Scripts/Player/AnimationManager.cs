using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] Animator mantaAnim;
    [SerializeField] Animator skeletonAnim;
    [SerializeField] Animator trickSkeletonAnim;

    [SerializeField] SkinnedMeshRenderer movementSkeletonMesh;
    [SerializeField] SkinnedMeshRenderer trickSkeletonMesh;


    [Header("Changable Variables")]
    [SerializeField] float trickAnimationSpeed;

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
        //trickSkeletonAnim.gameObject.SetActive(false);
        trickSkeletonMesh.enabled = false;
        trickSkeletonAnim.speed = trickAnimationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = GetInputs.PS5Map.Move.ReadValue<Vector2>().y;

        skeletonAnim.SetFloat("Joystick", moveInput);
        mantaAnim.SetFloat("Joystick", moveInput);

        if (GetInputs.PS5Map.Jump.WasPressedThisFrame())
        {
            DoTrick("Jump");
        }


    }

    public void DoTrick(string trickName)
    {
        //skeletonAnim.gameObject.SetActive(false);
        movementSkeletonMesh.enabled = false;

        //trickSkeletonAnim.gameObject.SetActive(true);
        trickSkeletonMesh.enabled = true;

        trickSkeletonAnim.SetTrigger(trickName);

        StartCoroutine(trickTimer());

        //Debug.Log(trickName + "inanimanager");
    }

    IEnumerator trickTimer()
    {
        yield return new WaitForSeconds(1.5f);

        //skeletonAnim.gameObject.SetActive(true);
        //trickSkeletonAnim.gameObject.SetActive(false);

        movementSkeletonMesh.enabled = true;
        trickSkeletonMesh.enabled = false;

    }

    public void DoJump()
    {
        //skeletonAnim.gameObject.SetActive(false);
        movementSkeletonMesh.enabled = false;

        //trickSkeletonAnim.gameObject.SetActive(true);
        trickSkeletonMesh.enabled = true;

        int randomFrame = (int) Mathf.Floor(Random.Range(0, 4));

        trickSkeletonAnim.SetInteger("JumpFrame", randomFrame);

        StartCoroutine(freezeTimer());
    }

    IEnumerator freezeTimer()
    {
        yield return new WaitForSeconds(0.5f);

        //skeletonAnim.gameObject.SetActive(true);
        //trickSkeletonAnim.gameObject.SetActive(false);

        trickSkeletonAnim.SetTrigger("EndJump");

        movementSkeletonMesh.enabled = true;
        trickSkeletonMesh.enabled = false;

    }
}
