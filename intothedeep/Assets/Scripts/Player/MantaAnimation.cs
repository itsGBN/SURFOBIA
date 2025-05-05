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

    [Header("Particles")]
    [SerializeField] ParticleSystem perfectTrick;
    [SerializeField] ParticleSystem goodTrick;
    [SerializeField] ParticleSystem badTrick;
    [SerializeField] ParticleSystem landing;

    [Header("GameObjects")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject trickSkeleton;
    [SerializeField] GameObject mantaRay;
    [SerializeField] GameObject graphics;

    [Header("Changable Variables")]
    [SerializeField] float trickAnimationSpeed;
    [SerializeField] float startRotationSpeed;
    [SerializeField] float startHoldingRotationSpeed;
    [SerializeField] float startBodyRotationSpeed;
    [SerializeField] float rotationMultiplier; // SHOULD BE 26. Previously 36.
    [SerializeField] LayerMask layerMask;

    bool isGrounded;
    bool wasGrounded;
    bool isCloseToGround;
    bool wasSpinning;

    bool isGrinding;
    bool wasGrinding;

    bool isHolding;
    bool isDoingTrick;
    bool isMove;

    float totalRotation;
    float comboMultiplier;
    float comboThreshold;
    float joystick;

    float totalBodyRotation;

    float rotationSpeed;
    float holdingRotationSpeed;
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
        holdingRotationSpeed = startHoldingRotationSpeed;
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

        if (GetInputs.PS5Map.Move.ReadValue<Vector2>().y != 0 && !GetInputs.PS5Map.LeftTrigger.WasPressedThisFrame() && !isGrinding && !isHolding && !isMove)
        {
            ActivateMove();
        }
        //RaycastHit hit;
        //isCloseToGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.5f, layerMask);

        if (!wasGrinding && isGrinding) ActivateGrind();
        if (wasGrinding && !isGrinding) DeactivateGrind();

        if (GetInputs.PS5Map.LeftTrigger.WasPressedThisFrame() && !isGrinding)
        {
            isHolding = true;
            totalRotation = 0; //Resets rotation tracking
            trickSkeletonAnim.SetTrigger("StartGrab");
            ActivateTrick(); //Switches to trick skeleton
            Debug.Log("grabbed");
        }
        if (GetInputs.PS5Map.LeftTrigger.WasReleasedThisFrame() && !isGrinding)
        {
            trickSkeletonAnim.SetTrigger("EndGrab");
            isHolding = false;
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
            //if (graphics.transform.rotation != player.transform.rotation) graphics.transform.rotation = Quaternion.Lerp(graphics.transform.rotation, player.transform.rotation, 0.05f); ;
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
            }
        }

        if (!wasGrounded && isGrounded) {

            if (isDoingTrick) {
                isDoingTrick = false;
                LandingCheck();

                totalRotation = 0;
                totalBodyRotation = 0;
                comboMultiplier = 0;
                rotationSpeed = startRotationSpeed;
                holdingRotationSpeed = startHoldingRotationSpeed;
                bodyRotationSpeed = startBodyRotationSpeed;

                graphics.transform.rotation = player.transform.rotation;
                Debug.Log("stoppedTrick");
            } 
           
            ResetPositions();
            if(!isHolding && !isMove) ActivateMove();
        }

        skeletonAnim.SetFloat("Joystick", joystick);
        mantaAnim.SetFloat("Joystick", joystick);

    }

    private void FixedUpdate()
    {

        if (isGrinding)
        {
            //mantaRay.transform.Rotate(new Vector3(0, rotationSpeed, 0));
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
                

                comboThreshold = bodyRotationSpeed * (36 / 10);

                if (totalBodyRotation > comboThreshold)
                {
                    totalPoints += 200;
                    if (bodyRotationSpeed < 15) bodyRotationSpeed += 1;

                    HUD.instance.onPlayerTrickHud("Flip", 20);
                    perfectTrick.Play();

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

                comboThreshold = bodyRotationSpeed * (36 / 10);

                if (totalBodyRotation < -comboThreshold)
                {
                    totalPoints += 200;
                    if (bodyRotationSpeed < 15) bodyRotationSpeed += 1;

                    HUD.instance.onPlayerTrickHud("Flip" ,20);
                    perfectTrick.Play();

                    totalBodyRotation = 0;
                }

            }

            //if (trickInput.x > 0.5f)
            if(trickInput.x > 0.5f || isTurningRight)
            {
                isDoingTrick = true;
                if(isHolding) mantaRay.transform.Rotate(new Vector3(0, holdingRotationSpeed, 0)); else mantaRay.transform.Rotate(new Vector3(0, rotationSpeed, 0));

                if (totalRotation < 0)
                {
                    totalRotation = 0;
                    rotationSpeed = startRotationSpeed;
                    comboMultiplier = 0;
                }

                if(isHolding)
                {
                    totalRotation += 1;
                    //comboThreshold = 0.1f; //Amount of +1 per rotation, might need to find a better way to do this
                    comboThreshold = holdingRotationSpeed * (36 / 10);
                    Debug.Log(totalRotation);

                    if (totalRotation > comboThreshold)
                    {
                        totalPoints += 100;
                        if (holdingRotationSpeed < 18) rotationSpeed += 2;
                        HUD.instance.onPlayerTrickHud("Grab Spin", 10);
                        goodTrick.Play();
                        totalRotation = 0;
                    }
                } else
                {
                    totalRotation += 1;
                    //comboThreshold = 0.1f; //Amount of +1 per rotation, might need to find a better way to do this
                    comboThreshold = rotationSpeed * (26 / 14);
                    Debug.Log(totalRotation);

                    if (totalRotation > comboThreshold)
                    {
                        totalPoints += 50;
                        if (rotationSpeed < 22) rotationSpeed += 1;
                        HUD.instance.onPlayerTrickHud("Spin", 5);

                        badTrick.Play();
                        //AudioManager.instance.Trick();

                        totalRotation = 0;
                    }
                }

                
            }
            if (trickInput.x < -0.5f || isTurningLeft)
            {
                isDoingTrick = true;
                if (isHolding) mantaRay.transform.Rotate(new Vector3(0, -holdingRotationSpeed, 0)); else mantaRay.transform.Rotate(new Vector3(0, -rotationSpeed, 0));

                if (isHolding)
                {
                    totalRotation -= 1;
                    comboThreshold = holdingRotationSpeed * (36 / 10);

                    if (totalRotation > 0)
                    {
                        totalRotation = 0;
                        holdingRotationSpeed = startHoldingRotationSpeed;
                        comboMultiplier = 0;
                    }

                    if (totalRotation < -comboThreshold)
                    {
                        totalPoints += 100;
                        if (holdingRotationSpeed < 18) holdingRotationSpeed += 2;
                        HUD.instance.onPlayerTrickHud("Grab Spin", 10);
                        goodTrick.Play();
                        totalRotation = 0;
                    }
                } else
                {
                    totalRotation -= 1;
                    comboThreshold = rotationSpeed * (26 / 14);

                    if (totalRotation > 0)
                    {
                        totalRotation = 0;
                        rotationSpeed = startRotationSpeed;
                        comboMultiplier = 0;
                    }

                    if (totalRotation < -comboThreshold)
                    {
                        totalPoints += 50;
                        if (rotationSpeed < 22) rotationSpeed += 1;
                        HUD.instance.onPlayerTrickHud("Spin", 5);
                        badTrick.Play();
                        totalRotation = 0;
                    }
                }

                
            }
        }

        
    }

    public void LandingCheck()
    {
        float totalRotationDisplacement = 0;
        float graphicsPercent = 0;

        //Debug.Log(totalRotation);

        float mantaAngle;
        Vector3 axis;
        mantaRay.transform.rotation.ToAngleAxis(out mantaAngle, out axis);

        graphicsPercent = 100 * totalBodyRotation / (bodyRotationSpeed * (36 / 10));
        
        //Debug.Log("Manta: " + mantaAngle + " Graphics: " + graphicsPercent);

        if((graphicsPercent < 10 || graphicsPercent > 90) && (mantaAngle < 45 || mantaAngle > 315))
        {
            landing.Play();
            StartCoroutine(player.GetComponent<PlayerController>().BoostActivate(0.25f));
            HUD.instance.onPlayerTrickHud("Perfect Landing!", 10);
        } else if((graphicsPercent < 30 || graphicsPercent > 70) && (mantaAngle < 70 || mantaAngle > 290))
        {
            StartCoroutine(player.GetComponent<PlayerController>().BoostActivate(0.15f));
            HUD.instance.onPlayerTrickHud("Good Landing", 5);
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
        isMove = false;
    }

    void ActivateMove()
    {
        movementSkeletonMesh.enabled = true;
        trickSkeletonMesh.enabled = false;
        isMove = true;
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

        ResetSkeleton(Quaternion.identity);
        graphics.transform.rotation = Quaternion.identity;
        ResetPositions();

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
