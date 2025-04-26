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
    [SerializeField] float startRotationSpeed;
    [SerializeField] float startBodyRotationSpeed;
    [SerializeField] LayerMask layerMask;

    bool isGrounded;
    bool wasGrounded;
    bool isCloseToGround;
    bool wasSpinning;

    bool isGrinding;
    bool wasGrinding;

    bool isHolding;
    bool isDoingTrick;

    float totalRotation;
    float comboMultiplier;
    float comboThreshold;
    float joystick;

    float totalBodyRotation;

    float rotationSpeed;
    float bodyRotationSpeed;

    bool isTurningRight;
    bool isTurningLeft;

    float moveInput;
    Vector2 trickInput;

    Vector3 mantaDisplacement;
    Vector3 skeletonDisplacement;
    Vector3 trickDisplacement;


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

        rotationSpeed = startRotationSpeed;
        bodyRotationSpeed = startBodyRotationSpeed;

        mantaDisplacement = player.transform.position - mantaRay.transform.position;
        skeletonDisplacement = player.transform.position - skeleton.transform.position;
        trickDisplacement = player.transform.position - trickSkeleton.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        wasGrounded = isGrounded; //Checks if was grounded in previous frame
        wasGrinding = isGrinding;

        moveInput = GetInputs.PS5Map.Move.ReadValue<Vector2>().y;
        joystick = moveInput;

        trickInput = GetInputs.PS5Map.TrickStick.ReadValue<Vector2>();
        isGrounded = player.GetComponent<PlayerController>().isGrounded;
        isGrinding = player.GetComponent<PlayerController>().currentState == player.GetComponent<PlayerController>().grindState;


        //RaycastHit hit;
        //isCloseToGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.5f, layerMask);

        if (!wasGrinding && isGrinding) ActivateGrind();
        if (wasGrinding && !isGrinding) DeactivateGrind();

        


        if (GetInputs.PS5Map.Jump.WasPressedThisFrame())
        {
            //ActivateTrick();
            //trickSkeletonAnim.SetTrigger("Jump");
        }

        if (GetInputs.PS5Map.LeftTrigger.WasPressedThisFrame())
        {
            isHolding = true;
            trickSkeletonAnim.SetTrigger("StartGrab");
            ActivateTrick();
            Debug.Log("grabbed");
        }
        if (GetInputs.PS5Map.LeftTrigger.WasReleasedThisFrame())
        {
            isHolding = false;
            trickSkeletonAnim.SetTrigger("EndGrab");
            Debug.Log("let go");
        }

        if(GetInputs.PS5Map.LeftBumper.WasPressedThisFrame()) {
            isTurningLeft = true;
            isTurningRight = false;
        }
        if(GetInputs.PS5Map.LeftBumper.WasReleasedThisFrame())
        {
            isTurningLeft = false;
        }

        if (GetInputs.PS5Map.RightBumper.WasPressedThisFrame())
        {
            isTurningRight = true;
            isTurningLeft = false;
        }
        if (GetInputs.PS5Map.RightBumper.WasReleasedThisFrame())
        {
            isTurningRight = false;
        }


        if (isGrounded)
        {
            if (mantaRay.transform.rotation != player.transform.rotation) mantaRay.transform.rotation = Quaternion.Lerp(mantaRay.transform.rotation, player.transform.rotation, 0.05f);
            if (skeleton.transform.rotation != player.transform.rotation)
            {
                skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, player.transform.rotation, 0.01f);
                trickSkeleton.transform.rotation = Quaternion.Lerp(trickSkeleton.transform.rotation, player.transform.rotation, 0.01f);
            }

            if (totalRotation != 0)
            {
                totalRotation = 0;
                comboMultiplier = 0;
                rotationSpeed = startRotationSpeed;
            }
        }
        else
        {
            

            if (isHolding)
            {

                skeleton.transform.rotation = mantaRay.transform.rotation;
                trickSkeleton.transform.rotation = mantaRay.transform.rotation;

                /*
                if (skeleton.transform.rotation != mantaRay.transform.rotation)
                {
                    skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, mantaRay.transform.rotation, 0.01f);
                    trickSkeleton.transform.rotation = Quaternion.Lerp(trickSkeleton.transform.rotation, mantaRay.transform.rotation, 0.01f);
                }
                else
                {
                    
                }
                */
            }

            /*
            if (moveInput > 0)
            {
                mantaRay.transform.rotation = Quaternion.Lerp(mantaRay.transform.rotation, player.transform.rotation, 0.01f);
                skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, player.transform.rotation, 0.01f);
                trickSkeleton.transform.rotation = Quaternion.Lerp(trickSkeleton.transform.rotation, player.transform.rotation, 0.01f);
            }
            */
        }

        if (!wasGrounded && isGrounded) {

            if (totalRotation != 0)
            {
                totalRotation = 0;
                comboMultiplier = 0;
                rotationSpeed = startRotationSpeed;
            }

            if (isDoingTrick) {
                isDoingTrick = false;
                LandingCheck();
                Debug.Log("stoppedTrick");
            } 
           
            ResetPositions();
            //if (player.transform.rotation.y != mantaRay.transform.rotation.y) 
        }

        skeletonAnim.SetFloat("Joystick", joystick);
        mantaAnim.SetFloat("Joystick", joystick);

    }

    private void FixedUpdate()
    {

        if (isGrinding)
        {
            mantaRay.transform.Rotate(new Vector3(0, rotationSpeed, 0));
            skeleton.transform.Rotate(new Vector3(0, rotationSpeed, 0));
            trickSkeleton.transform.Rotate(new Vector3(0, rotationSpeed, 0));
        }

        if(!isGrounded)
        {
            if (isHolding && trickInput.y > 0.5f)
            {
                isDoingTrick = true;
                graphics.transform.Rotate(new Vector3(bodyRotationSpeed, 0, 0));

                if (totalBodyRotation < 0)
                {
                    totalBodyRotation = 0;
                    bodyRotationSpeed = startBodyRotationSpeed;
                    comboMultiplier = 0;
                }

                totalBodyRotation += 1;

                comboThreshold = rotationSpeed * (36 / 10);

                if (totalBodyRotation > comboThreshold)
                {
                    totalPoints += 200;
                    if (bodyRotationSpeed < 15) bodyRotationSpeed += 1;
                    HUD.instance.onPlayerTrickHud("Flip +200");
                    totalBodyRotation = 0;
                }

            }
            if (isHolding && trickInput.y < -0.5f)
            {
                isDoingTrick = true;
                graphics.transform.Rotate(new Vector3(-bodyRotationSpeed, 0, 0));

                if (totalBodyRotation > 0)
                {
                    totalBodyRotation = 0;
                    bodyRotationSpeed = startBodyRotationSpeed;
                    comboMultiplier = 0;
                }

                totalBodyRotation -= 1;

                comboThreshold = rotationSpeed * (36 / 10);

                if (totalBodyRotation < -comboThreshold)
                {
                    totalPoints += 200;
                    if (bodyRotationSpeed < 15) bodyRotationSpeed += 1;
                    HUD.instance.onPlayerTrickHud("Flip +200");
                    totalBodyRotation = 0;
                }

            }

            //if (trickInput.x > 0.5f)
            if(trickInput.x > 0.5f || isTurningRight)
            {
                isDoingTrick = true;
                mantaRay.transform.Rotate(new Vector3(0, rotationSpeed, 0));

                if (totalRotation < 0)
                {
                    totalRotation = 0;
                    rotationSpeed = startRotationSpeed;
                    comboMultiplier = 0;
                }

                totalRotation += 1;
                //comboThreshold = 0.1f; //Amount of +1 per rotation, might need to find a better way to do this
                comboThreshold = rotationSpeed * (36 / 10);

                if (totalRotation > comboThreshold)
                {
                    totalPoints += 100;
                    if (rotationSpeed < 18) rotationSpeed += 2;
                    HUD.instance.onPlayerTrickHud("Spin +100");
                    totalRotation = 0;
                }
            }
            //if ()
            if (trickInput.x < -0.5f || isTurningLeft)
            {
                isDoingTrick = true;
                mantaRay.transform.Rotate(new Vector3(0, -rotationSpeed, 0));

                totalRotation -= 1;
                comboThreshold = rotationSpeed * (36 / 10);

                if (totalRotation > 0)
                {
                    totalRotation = 0;
                    rotationSpeed = startRotationSpeed;
                    comboMultiplier = 0;
                }

                if (totalRotation < -comboThreshold)
                {
                    totalPoints += 100;
                    if (rotationSpeed < 18) rotationSpeed += 2;
                    HUD.instance.onPlayerTrickHud("Spin +100");
                    totalRotation = 0;
                }
            }
        }

        
    }

    public void LandingCheck()
    {
        float totalRotationDisplacement = 0;
        totalRotationDisplacement = Mathf.Abs(player.transform.rotation.y - mantaRay.transform.rotation.y);

        if (totalRotationDisplacement < 0.3f)
        {
            HUD.instance.onPlayerTrickHud("Perfect +50");
            totalPoints += 50;
        } else if(totalRotationDisplacement < 0.5f)
        {
            HUD.instance.onPlayerTrickHud("Good +30");
            totalPoints += 50;
        }
        else if (totalRotationDisplacement < 0.8f)
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

    public bool GetIsDoingTrick()
    {
        return isDoingTrick;
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
        int grindFrame = (int) Mathf.Floor(Random.Range(0, 4));
        Debug.Log(grindFrame);
        ActivateTrick();
        trickSkeletonAnim.SetTrigger("StartGrind");
        trickSkeletonAnim.SetInteger("FreezeFrame", grindFrame);
    }

    void DeactivateGrind()
    {
        trickSkeletonAnim.SetTrigger("EndGrind");
        ActivateMove();
        ResetSkeleton(player.transform.rotation);
        Debug.Log("EndGrind");
    }

    void ResetSkeleton(Quaternion resetRotation)
    {
        skeleton.transform.rotation = resetRotation;
        trickSkeleton.transform.rotation = resetRotation;
    }

    void ResetPositions()
    {
        mantaRay.transform.position = player.transform.position - mantaDisplacement;
        skeleton.transform.position = player.transform.position - skeletonDisplacement;
        trickSkeleton.transform.position = player.transform.position - trickDisplacement;
    }

}
