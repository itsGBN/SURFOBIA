using System.Collections;
using System.Collections.Generic;
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
        float trickInput = GetInputs.PS5Map.TrickStick.ReadValue<Vector2>().x;
        isGrounded = player.GetComponent<PlayerController>().isGrounded;

        skeletonAnim.SetFloat("Joystick", moveInput);
        mantaAnim.SetFloat("Joystick", moveInput);

        if (GetInputs.PS5Map.Jump.WasPressedThisFrame())
        {
            DoTrick("Jump");
        }

        if(!isGrounded && trickInput != 0)
        {
            if (trickInput > 0.5f) mantaRay.transform.Rotate(new Vector3(0, 0, 0.75f));
            if (trickInput < 0.5f) mantaRay.transform.Rotate(new Vector3(0, 0, -0.75f));
        } else
        {
            //mantaRay.transform.rotation = Quaternion.identity;
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
