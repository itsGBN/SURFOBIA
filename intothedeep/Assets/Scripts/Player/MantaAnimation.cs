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
    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject trickSkeleton;
    [SerializeField] GameObject mantaRay;
    [SerializeField] GameObject graphics;

    [Header("Changable Variables")]
    [SerializeField] float trickAnimationSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] LayerMask layerMask;

    bool isGrounded;
    bool wasGrounded;
    bool isCloseToGround;
    bool wasSpinning;

    float totalRotation;
    float comboMultiplier;
    float comboThreshold;
    float joystick;

    //public
    public float totalPoints;



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
        ActivateMove(); //Activates movement, deactivates trick
        trickSkeletonAnim.speed = trickAnimationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        wasGrounded = isGrounded; //Checks if was grounded in previous frame

        float moveInput = GetInputs.PS5Map.Move.ReadValue<Vector2>().y;
        joystick = moveInput;

        Vector2 trickInput = GetInputs.PS5Map.TrickStick.ReadValue<Vector2>();
        isGrounded = player.GetComponent<PlayerController>().isGrounded;

        RaycastHit hit;
        isCloseToGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.5f, layerMask);

        

        if (GetInputs.PS5Map.Jump.WasPressedThisFrame())
        {
            //ActivateTrick();
            //trickSkeletonAnim.SetTrigger("Jump");
        }

        


        if (isGrounded)
        {
            if (mantaRay.transform.rotation != player.transform.rotation) mantaRay.transform.rotation = Quaternion.Lerp(mantaRay.transform.rotation, player.transform.rotation, 0.05f);
            if(skeleton.transform.rotation != player.transform.rotation)
            {
                skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, player.transform.rotation, 0.01f);
                trickSkeleton.transform.rotation = Quaternion.Lerp(trickSkeleton.transform.rotation, player.transform.rotation, 0.01f);
            }

            if (totalRotation != 0)
            {
                totalRotation = 0;
                comboMultiplier = 0;
                rotationSpeed = 0.75f;
            }
        }
        else
        {
            /*
            if (trickInput.y > 0.1f) {
                graphics.transform.Rotate(new Vector3(0, 0, 0.75f));
                joystick = trickInput.y;
            }

            if (trickInput.y < -0.1f) {
                graphics.transform.Rotate(new Vector3(0, 0, -0.75f));
                joystick = trickInput.y;
            } 
            */


            if (Input.GetKey(KeyCode.T) || GetInputs.PS5Map.LeftTrigger.WasPressedThisFrame())
            {

                if (skeleton.transform.rotation != mantaRay.transform.rotation)
                {
                    skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, mantaRay.transform.rotation, 0.01f);
                    trickSkeleton.transform.rotation = Quaternion.Lerp(trickSkeleton.transform.rotation, mantaRay.transform.rotation, 0.01f);
                }
                else
                {
                    skeleton.transform.rotation = mantaRay.transform.rotation;
                    trickSkeleton.transform.rotation = mantaRay.transform.rotation;
                }

                Debug.Log("Pressed T");
            }

            if (trickInput.x > 0.5f && moveInput == 0)
            {
                mantaRay.transform.Rotate(new Vector3(0, rotationSpeed, 0));

                if (totalRotation < 0)
                {
                    totalRotation = 0;
                    rotationSpeed = 0.75f;
                    comboMultiplier = 0;
                }

                totalRotation += 1;
                comboThreshold = 352 / rotationSpeed; //Amount of +1 per rotation, might need to find a better way to do this


                if (totalRotation > comboThreshold)
                {
                    totalPoints += 100;
                    if (rotationSpeed < 2) rotationSpeed += 0.25f;
                    HUD.instance.onPlayerTrickHud("Spin +100");
                    totalRotation = 0;
                }

            }
            if (trickInput.x < -0.5f && moveInput == 0)
            {
                mantaRay.transform.Rotate(new Vector3(0, -rotationSpeed, 0));

                totalRotation -= 1;
                comboThreshold = (-352 / rotationSpeed); //Amount of +1 per rotation, might need to find a better way to do this

                if (totalRotation > 0)
                {
                    totalRotation = 0;
                    rotationSpeed = 0.75f;
                    comboMultiplier = 0;
                }

                if (totalRotation < comboThreshold)
                {
                    totalPoints += 100;
                    if (rotationSpeed < 2) rotationSpeed += 0.25f;
                    HUD.instance.onPlayerTrickHud("Spin +100");
                    totalRotation = 0;
                }

                if (moveInput > 0)
                {
                    mantaRay.transform.rotation = Quaternion.Lerp(mantaRay.transform.rotation, player.transform.rotation, 0.01f);
                    skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, player.transform.rotation, 0.01f);
                    trickSkeleton.transform.rotation = Quaternion.Lerp(trickSkeleton.transform.rotation, player.transform.rotation, 0.01f);

                }
            }
        }

        if (!wasGrounded && isGrounded) {
            //trickSkeletonAnim.SetTrigger("EndJump");
            if(totalRotation != 0) LandingCheck();
        }

        skeletonAnim.SetFloat("Joystick", joystick);
        mantaAnim.SetFloat("Joystick", joystick);

    }

    public void LandingCheck()
    {
        float totalRotationDisplacement = 0;
        totalRotationDisplacement = Mathf.Abs(player.transform.rotation.y - mantaRay.transform.rotation.y);

        if (totalRotationDisplacement < comboThreshold)
        {
            HUD.instance.onPlayerTrickHud("Perfect +50");
            totalPoints += 50;
        } else if(totalRotationDisplacement < 0.2f)
        {
            HUD.instance.onPlayerTrickHud("Good +30");
            totalPoints += 50;
        }
        else if (totalRotationDisplacement < 0.6f)
        {
            HUD.instance.onPlayerTrickHud("Fine +10");
            totalPoints += 50;
        }
        else
        {
            HUD.instance.onPlayerTrickHud("Awful +0");
            totalPoints += 50;
        }
    }

    void ActivateTrick()
    {
        trickSkeletonMesh.enabled = true;
        movementSkeletonMesh.enabled = false;
    }

    void ActivateMove()
    {
        movementSkeletonMesh.enabled = true;
        trickSkeletonMesh.enabled = false;
    }

    void ActivateGrind()
    {
        int grindFrame = (int)Random.Range(0, 4);
        ActivateTrick();
        trickSkeletonAnim.SetInteger("FreezeFrame", grindFrame);
    }
    
    void DeactivateGrind()
    {
        trickSkeletonAnim.SetTrigger("EndGrind");
        ActivateMove();
    }

}
