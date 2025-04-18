using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MantaAnimation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator mantaAnim;
    [SerializeField] Animator skeletonAnim;
    [SerializeField] Animator trickSkeletonAnim;

    [SerializeField] SkinnedMeshRenderer movementSkeletonMesh;
    [SerializeField] SkinnedMeshRenderer trickSkeletonMesh;

    [Header("GameObjects")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject mantaRay;


    [Header("Changable Variables")]
    [SerializeField] float trickAnimationSpeed;

    bool isGrounded;

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
        Vector2 trickInput = GetInputs.PS5Map.TrickStick.ReadValue<Vector2>();
        isGrounded = player.GetComponent<PlayerController>().isGrounded;

        skeletonAnim.SetFloat("Joystick", moveInput);
        mantaAnim.SetFloat("Joystick", moveInput);

        if (GetInputs.PS5Map.Jump.WasPressedThisFrame())
        {
            DoTrick("Jump");
        }

        if(!isGrounded)
        {
            if (trickInput.y > 0.5f) mantaRay.transform.Rotate(new Vector3(0, 0, 0.75f));
            if (trickInput.y < -0.5f) mantaRay.transform.Rotate(new Vector3(0, 0, -0.75f));

            if (trickInput.x > 0.5f) mantaRay.transform.Rotate(new Vector3(0, 0.75f, 0));
            if (trickInput.x < -0.5f) mantaRay.transform.Rotate(new Vector3(0, -0.75f,  0));

            if (moveInput > 0) mantaRay.transform.rotation = Quaternion.Lerp(mantaRay.transform.rotation, player.transform.rotation, 0.01f);

            //if (GetInputs.PS5Map.LeftTrigger.WasPressedThisFrame()) mantaRay.transform.Rotate(new Vector3(0, 0.75f, 0));
            //if (GetInputs.PS5Map.RightTrigger.WasPressedThisFrame()) mantaRay.transform.Rotate(new Vector3(0, -0.75f, 0));


        } else
        {
            if(mantaRay.transform.rotation != player.transform.rotation) mantaRay.transform.rotation = Quaternion.Lerp(mantaRay.transform.rotation, player.transform.rotation, 0.01f);
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

}
