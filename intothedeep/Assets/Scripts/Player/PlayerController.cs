using System.Collections;
using Unity.Splines.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines; // Import Unity's Spline package
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEngine.UIElements;
using static System.TimeZoneInfo;
using static GameManager;

public class PlayerController : MonoBehaviour
{
    public bool TUTORIAL = false;

    [Header("Speed")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float grindSpeed = 5f;
    public float accel = 20;
    public float decel = 7;
    private float currentSpeed;

    [Header("Turning")]
    public float turnSpeed = 110f;
    public float turnAccel = 70f;
    [Tooltip("Get to this speed with faster acceleration, so it doesn't feel draggy")] public float initTurnSpeed = 80f;
    public float initTurnAccel = 90f;
    private float currentTurnSpeed;
    private float turnHold; // how long we've been holding turn

    private float savedIdleFloat;
    public float idleFloat = 0.2f;
    [Header("Auto Deceleration")]
    [Tooltip("After forward is released，speed decelerates to idleSpeed")]
    public float decelerationSmooth = 0.2f;
    public float decelerationDelay = 0.2f;

// 运行时用，不要在 Inspector 显示
    [HideInInspector] public float timeSinceRelease = 0f;
    [HideInInspector] public bool hasReachedMaxSpeed = false;
    [HideInInspector] public bool decelStarted = false;

    [Header("Braking")]
    public float brakeDecel = 8;
    [Range(0, 1)][Tooltip("0.5 will half the speed on brake")] public float initialBrakeMultiplier = 0.7f;
    public float brakeTurnDecel = 70;
    [Range(-1, 0)][Tooltip("How far back the stick needs to be pulled back to brake")] public float brakeThreshold = -0.45f;
    private bool isBraking = false;
    private float curBrakeSpeed;
    private float brakeTurnDir;

    [Header("Jumping and Grounded")]
    public float jumpHeight = 5f; // Jump height (force)
    public float groundCheckDistance = 0.5f; // Distance to check for ground
    public LayerMask groundLayer; // Layer mask for ground detection
    public bool isGrounded;
    public bool grounding;
    public float fallOffset = -100;

    public IPlayerState currentState;
    public FreeRoamState freeRoamState;
    public ZeroState zeroState;
    public GrindState grindState;
    public FreefallState freefallState;

    private SplineContainer currentSpline;
    private float progressAlongSpline = 0f;
    private Collider lastGrindCollider;

    private Rigidbody rb;
    private bool isDiving = false;
    public float diveForwardSpeed = 5f;
    public float diveFallSpeed = -2f;

    public bool isBoosting = false;
    public Vector3 currentSurfaceNormal = Vector3.up;

    [Header("Graphics")]
    public Transform graphics;
    private float boardRoll;
    private float boardYaw;
    private float curBoardRoll;
    private float curBoardYaw;
    public float rollSpeed = 2.5f;
    public float boardRollAmount = 25f;
    
    [Header("Camera Animator")]
    [SerializeField] Animator cameraAnimator;

    public CinemachineVirtualCamera airCam;
    bool prevIsGrounded;
    private bool airFovSet = false;
    CinemachineBasicMultiChannelPerlin airCamNoise;
    private CinemachineComposer airCamComposer;
    [SerializeField] float defaultAirFOV = 70f;
    [SerializeField] float defaultNoiseFreq     = 4f;
    [SerializeField] private float defaultVerticalDamping = 0.6f;
    [SerializeField] float maxAirFOV     = 80f;
    [SerializeField] float maxNoiseFreq         = 8f;
    [SerializeField] float maxVerticalDamping = 1f;
    
    [Range(0,1)] public float startThresholdFraction = 0.5f;
    [SerializeField] float fovResetDuration = 0.21f;
    
    [Header("Landing Shake")]
    [SerializeField] CinemachineImpulseSource landingImpulse;
    [SerializeField] float maxAirTime        = 2f;
    [SerializeField] float minImpulseY       = -0.3f;
    [SerializeField] float maxImpulseY       =  -1.5f;
    [SerializeField] float minAirTimeForShake = 0.2f; 
    private bool wasInAir = false;  
    float airTime;
    
    [Header("Special Spline")]
    [SerializeField] CinemachineStateDrivenCamera stateDrivenCamera;
    [HideInInspector] public bool isOnSpecialSpline = false;
    [SerializeField] private CinemachineVirtualCamera[] specialSplineCams;
    
    #region CONTROLLER
    private PS5Input GetInputs;

    private void Awake()
    {
        GetInputs = new PS5Input();
        if (cameraAnimator == null)
            cameraAnimator = GameObject.Find("CameraControl").GetComponent<Animator>();
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
    //START
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        freeRoamState = new FreeRoamState(this);
        zeroState = new ZeroState(this);
        grindState = new GrindState(this);
        freefallState = new FreefallState(this);
        currentState = !TUTORIAL ? zeroState : freeRoamState;
        cameraAnimator.SetInteger("State", 2);
        curBoardRoll = graphics.transform.localEulerAngles.z;
        curBoardYaw = graphics.transform.localEulerAngles.y;
        prevIsGrounded = isGrounded;
        cameraAnimator.SetBool("inAir", !isGrounded);
        if (airCam != null)
        {
            airCamNoise = airCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            airCamComposer = airCam.GetCinemachineComponent<CinemachineComposer>();
            if (airCamComposer != null)
                defaultVerticalDamping = airCamComposer.m_VerticalDamping;

            airCam.m_Lens.FieldOfView                  = defaultAirFOV;
            if (airCamNoise != null) airCamNoise.m_FrequencyGain   = defaultNoiseFreq;
        }

        if (specialSplineCams != null)
        {
            foreach (var cam in specialSplineCams)
                cam.Priority = 0;
        }
        
    }

    //FIXED UPDATE
    void FixedUpdate()
    {
        currentState.UpdateState();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (!isGrounded)
        {
            grounding = false;
            rb.AddForce(Vector3.down * 20f);
        }
    }

    //UPDATE
    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (GameManager.instance.InputActive)
        {
            if (GetInputs.PS5Map.Jump.WasPressedThisFrame() && isGrounded)
            {
                Jump();
            }
            else if (GetInputs.PS5Map.Jump.WasPressedThisFrame() && !isGrounded)
            {
                StartDive();
            }
            else if (GetInputs.PS5Map.Menu.WasPressedThisFrame() && !isGrounded)
            {
                StopDive();
            }
        }
        if (GetInputs.PS5Map.Menu.WasPressedThisFrame() && currentState is ZeroState && !MainMenuEvents.instance.isTrasitioning)
        {
                Debug.Log("control press");
                SetState(freeRoamState);
                Debug.Log("Escape registered in Update() - transition from ZeroState");

        }
        
        float fallThreshold = CheckPointScript.checkpointPosition.y +fallOffset;
        if (!TUTORIAL && transform.position.y < fallThreshold)
        {
            ResetMovement();
            GameManager.instance.UpdateState(GameState.READY);
            StartCoroutine(MainMenuEvents.instance.onCheckTransition(SceneManager.GetActiveScene().name, MainMenuEvents.instance.transitionName, 1f));
        }
        
       
        bool wasGrounded = prevIsGrounded;


        if (!isGrounded)
        {
            if (wasGrounded)
            {
                airTime = 0f;
                wasInAir = true;        
            }
            airTime += Time.deltaTime;
        }
        
        if (isGrounded != wasGrounded)
        {
            
            cameraAnimator.SetBool("inAir", !isGrounded);

           
            if (!isGrounded && !airFovSet)
            {
                float dynamicThreshold = (moveSpeed - currentSpeed) * startThresholdFraction;
                if (currentSpeed >= dynamicThreshold)
                {
                    SetAirFOV();
                    airFovSet = true;
                }
            }
            

            prevIsGrounded = isGrounded;
        }
        
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
    }
    
    void SetAirFOV()
    {
        if (airCam == null) return;
        // 线性插到 [defaultAirFOV, maxAirFOV]
        float t = Mathf.Clamp01(currentSpeed / moveSpeed);
        airCam.m_Lens.FieldOfView = Mathf.Lerp(defaultAirFOV, maxAirFOV, t);
        if (airCamNoise != null)
            airCamNoise.m_FrequencyGain = Mathf.Lerp(defaultNoiseFreq, maxNoiseFreq, t);
        if (airCamComposer != null)
            airCamComposer.m_VerticalDamping = Mathf.Lerp(defaultVerticalDamping, maxVerticalDamping, t);
    }

    IEnumerator ResetAirFOVSmooth()
    {
        if (airCam == null) yield break;
        float startFov = airCam.m_Lens.FieldOfView;
        float startNoiseFreq = airCamNoise.m_FrequencyGain;
        float startDamp   = airCamComposer != null ? airCamComposer.m_VerticalDamping : defaultVerticalDamping;
        float elapsed  = 0f;

        while (elapsed < fovResetDuration)
        {
            elapsed += Time.deltaTime;
            airCam.m_Lens.FieldOfView = Mathf.Lerp(startFov, defaultAirFOV, elapsed / fovResetDuration);
            airCamNoise.m_FrequencyGain = Mathf.Lerp(startNoiseFreq, defaultNoiseFreq, elapsed / fovResetDuration);
            if (airCamComposer != null)
                airCamComposer.m_VerticalDamping = Mathf.Lerp(startDamp, defaultVerticalDamping, elapsed / fovResetDuration);
            yield return null;
        }
        airCam.m_Lens.FieldOfView = defaultAirFOV;
        airCamNoise.m_FrequencyGain = defaultNoiseFreq;
        if (airCamComposer != null) airCamComposer.m_VerticalDamping = defaultVerticalDamping;
    }

    public void ResetMovement()
    {
        rb.velocity        = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentSpeed       = 0f;
        currentTurnSpeed   = 0f;
        savedIdleFloat = idleFloat;
        idleFloat = 0;
    }
    public void RestoreIdleFloatAfterRespawn()
    {
        idleFloat = savedIdleFloat;
    }

    //SETTING THE NEW STATE
    public void SetState(IPlayerState newState)
    {
        rb.velocity = Vector3.zero;
        if (newState == freeRoamState)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                transform.position = hit.point + Vector3.up * 0.1f;
            }
        }
        if (newState == freeRoamState && lastGrindCollider != null)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), lastGrindCollider, false);
            lastGrindCollider = null;
        }

        currentState = newState;
        int stateInt = 0;
        if (newState == freeRoamState) stateInt = 0;
        else if (newState == grindState)  stateInt = 1;
        else if (newState == zeroState)   stateInt = 2;
        cameraAnimator.SetInteger("State", stateInt);
    }

    //ALIGN PLAYER TO SURFACE
    public void AlignToSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            currentSurfaceNormal = hit.normal; // Save the normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, currentSurfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            currentSurfaceNormal = Vector3.up;
        }

        if (!isGrounded)
        {
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }


    //START GRIND
    public void StartGrind(SplineContainer splineContainer)
    {
        currentSpline = splineContainer;
        if (currentSpline != null && currentSpline.Splines.Count > 0)
        {
            if (specialSplineCams != null)
            {
                foreach (var cam in specialSplineCams)
                    cam.Priority = 0;
            }

            if (splineContainer.TryGetComponent<SpecialSpline>(out var special))
            {
                special._vcam.Priority = stateDrivenCamera.Priority + 1;
                isOnSpecialSpline = true;
            }
            else
            {
                isOnSpecialSpline = false;
            }
            float closestT = GetClosestPointOnSpline(transform.position);
            progressAlongSpline = closestT;
            SetState(grindState);
        }
    }

    public IEnumerator BoostActivate(float duration)
    {
        Debug.Log("Boosting");
        isBoosting = true;
        yield return new WaitForSeconds(duration);
        isBoosting = false;
    }


    //JUMP
    public void Jump()
    {
        AudioManager.instance.Jump();
        float angle = Vector3.SignedAngle(Vector3.up, currentSurfaceNormal, transform.right);

        if (angle > -15f)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight * 2, rb.velocity.z);
        }
        airTime = 0f;
    }

    //DIVE
    public void StartDive()
    {
        isDiving = true;
        rb.velocity = new Vector3(transform.forward.x * diveForwardSpeed, diveFallSpeed, transform.forward.z * diveForwardSpeed);
    }

    public void StopDive()
    {
        isDiving = false;
    }

    //RED FLASH
    void FlashRed()
    {
        StartCoroutine(HUD.instance.onRed());
        AudioManager.instance.Hit();
        HUD.instance.onPlayerTrickHud("CRASH!", -20);
        GameManager.instance.FreezeFrame(0.08f);
    }

    private void ApplySpeedBoost()
    {
        float boostForce = 20f; // Tune this value for your desired punch
        Vector3 boostDirection = transform.forward;

        rb.AddForce(boostDirection * boostForce, ForceMode.Impulse);

        HUD.instance.onPlayerTrickHud("BOOST!", 10);
        // AudioManager.instance.Boost(); // Uncomment if sound exists
    }


    //FIND THE CLOSEST POINT ON THE GRIND FOR GRINDING
    public float GetClosestPointOnSpline(Vector3 position)
    {
        if (currentSpline == null)
        {
            return 0f;
        }

        float closestT = 0f;
        float minDistance = Mathf.Infinity;

        for (float t = 0f; t <= 1f; t += 0.01f)
        {
            Vector3 pointOnSpline = currentSpline.EvaluatePosition(t);
            float distance = Vector3.Distance(position, pointOnSpline);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestT = t;
            }
        }

        return closestT;
    }
    
    public float GetCurrentSpeed() {
        return currentSpeed;
    }

    //STATE INTERFACE
    public interface IPlayerState
    {
        void UpdateState();
    }


    //STATE.FREEROAM
    public class FreeRoamState : IPlayerState
    {
        private PlayerController player;
        private ReceivingAngle ra;

        public FreeRoamState(PlayerController player)
        {
            this.player = player;
            this.ra = GameObject.Find("RA").GetComponent<ReceivingAngle>();
        }

        public void UpdateState()
        {
            player.rb.isKinematic = false;

            float moveInput = 0;
            float turnInput = 0;
            float deadZone = 0.1f;
            if (GameManager.instance.InputActive)
            {
                moveInput = player.GetInputs.PS5Map.Move.ReadValue<Vector2>().y;
                turnInput = player.GetInputs.PS5Map.Move.ReadValue<Vector2>().x;
            }

            player.AlignToSurface();

            // Get direction the player is facing and project it onto the surface plane
            Vector3 inputDirection = player.transform.forward;
            Vector3 flattenedDirection = Vector3.ProjectOnPlane(inputDirection, player.currentSurfaceNormal).normalized;
            if (player.idleFloat == 0f && Mathf.Abs(moveInput) > deadZone)
            {
                player.idleFloat = player.savedIdleFloat;
            }
            // Braking, threshold makes deadzone for pulling the stick back
            if (moveInput < player.brakeThreshold && player.currentSpeed > 0 && player.isGrounded)
            {
                if (!player.isBraking)
                {
                    player.isBraking = true;
                    player.currentSpeed *= player.initialBrakeMultiplier;
                    player.currentTurnSpeed *= player.initialBrakeMultiplier;
                    player.curBrakeSpeed = 0;
                    player.brakeTurnDir = Mathf.Sign(turnInput);
                }
            }
            else if (moveInput >= 0)
            {
                if (player.isBraking)
                {
                    player.isBraking = false;
                    player.curBrakeSpeed = 0;
                    if (RumbleManager.instance != null) { RumbleManager.instance.SetRumbleActive(false); }
                }
            }

            if (player.isBraking)
            {
                player.curBrakeSpeed += player.brakeDecel * Time.fixedDeltaTime;
                player.currentSpeed -= player.curBrakeSpeed * Time.fixedDeltaTime;
                if (RumbleManager.instance != null) { RumbleManager.instance.SetRumbleActive(player.currentSpeed / player.moveSpeed * 0.75f, player.currentSpeed / player.moveSpeed * 0.7f); }
            }

            // --- acceleration / deceleration after release ---
            if (moveInput > deadZone || player.isBoosting)
            {
                // 1) 有输入：立即加速，重置所有延迟相关的状态
                player.timeSinceRelease = 0f;
                player.decelStarted = false;
                player.hasReachedMaxSpeed = false;

                // 原先的加速逻辑
                //!!adding boost possibilty
                if (player.isBoosting)
                {
                    player.currentSpeed += player.accel * 2.5f * Time.fixedDeltaTime;
                    player.currentSpeed = Mathf.Min(player.currentSpeed, player.moveSpeed * 2f);
                }
                else
                {
                    player.currentSpeed += player.accel * moveInput * Time.fixedDeltaTime;
                    player.currentSpeed = Mathf.Min(player.currentSpeed, player.moveSpeed);
                }
                //player.currentSpeed += player.accel * moveInput * Time.fixedDeltaTime;
                

                // 2) 如果达到了极限速，就打标记
                if (player.currentSpeed >= player.moveSpeed)
                    player.hasReachedMaxSpeed = true;
            }
            else
            {
                // 玩家松开前进杆
                if (player.hasReachedMaxSpeed && !player.decelStarted)
                {
                    // 3) 只有在“曾经跑满”且“还未开始减速”时，才累加延迟计时
                    player.timeSinceRelease += Time.fixedDeltaTime;
                    if (player.timeSinceRelease >= player.decelerationDelay)
                    {
                        // 4) 延迟结束，正式启动减速
                        player.decelStarted = true;
                    }
                }

                // 5) 如果尚未跑满 或者 已经开始减速，都执行平滑减速
                if (!player.hasReachedMaxSpeed || player.decelStarted)
                {
                    player.currentSpeed = Mathf.MoveTowards(
                        player.currentSpeed,
                        player.idleFloat,
                        player.decelerationSmooth * Time.fixedDeltaTime
                    );
                }
                // 否则：仍在等待延迟，不做任何改动，保持当前速度
            }


            // Turning (y-axis rotation)
            if (player.isBraking)
            {
                player.currentTurnSpeed -= player.brakeTurnDecel * player.brakeTurnDir * Time.fixedDeltaTime;
                // Clamp, so it doesn't go past 0 in the opposite direction
                //if (player.brakeTurnDir < 0) { player.currentTurnSpeed = Mathf.Min(player.currentTurnSpeed, 0); }
                //else if (player.brakeTurnDir > 0) { player.currentTurnSpeed = Mathf.Max(0, player.currentTurnSpeed); }
            }
            else if (Mathf.Abs(turnInput) > 0 && !player.isBraking)
            {
                player.turnHold += Time.deltaTime;
                //player.currentTurnSpeed = turnInput * player.turnSpeed;
                if (Mathf.Abs(player.currentTurnSpeed) < player.initTurnSpeed) { player.currentTurnSpeed += player.initTurnAccel * Time.fixedDeltaTime; }
                else if (player.turnHold >= 1f && Mathf.Abs(turnInput) > 0.65f) { player.currentTurnSpeed += player.turnAccel * Mathf.Abs(turnInput) * Time.fixedDeltaTime; }
                if (Mathf.Abs(turnInput) <= 0.65f)
                {
                    player.turnHold = 0;
                    player.currentTurnSpeed = Mathf.MoveTowards(player.currentTurnSpeed, 0, player.turnAccel * 1.45f * Time.fixedDeltaTime);
                }
            }
            player.currentTurnSpeed = Mathf.Clamp(player.currentTurnSpeed, 0, player.turnSpeed);

            // ✅ Calculate forward direction constrained to terrain surface
            Vector3 forwardOnSurface = Vector3.ProjectOnPlane(player.transform.forward, player.currentSurfaceNormal).normalized;

            // ✅ Move player only forward, terrain-conforming
            Vector3 moveDirection = forwardOnSurface * player.currentSpeed * Time.fixedDeltaTime;
            player.transform.position += moveDirection;

            // Rotate left/right
            player.transform.Rotate(Vector3.up, player.currentTurnSpeed * turnInput * Time.fixedDeltaTime);

            //Debug.Log(player.currentTurnSpeed);
            // Player graphics
            if (player.isBraking) { player.boardRoll = -player.brakeTurnDir * player.currentSpeed * 1.75f; }
            else if (turnInput == 0) { player.boardRoll = 0; }
            else { player.boardRoll = -Mathf.Sign(turnInput) * player.boardRollAmount; }

            if (player.isBraking) { player.boardYaw = player.brakeTurnDir * Mathf.Abs(player.currentSpeed) * 3f; }
            else { player.boardYaw = turnInput; }

            //player.boardRoll = Mathf.Clamp(player.boardRoll, -player.boardRollAmount, player.boardRollAmount);
            player.boardYaw = Mathf.Clamp(player.boardYaw, -70, 70);

            float yawSpeed = player.isBraking ? player.rollSpeed : player.rollSpeed / 2;

            player.curBoardRoll = Mathf.LerpAngle(player.curBoardRoll, player.boardRoll, player.rollSpeed * Time.fixedDeltaTime);
            player.curBoardYaw = Mathf.LerpAngle(player.curBoardYaw, player.boardYaw, yawSpeed * Time.fixedDeltaTime);

            player.graphics.localEulerAngles = new Vector3(0, player.curBoardYaw, player.curBoardRoll);

            float angle = Vector3.SignedAngle(Vector3.up, player.currentSurfaceNormal, player.transform.right);
            if (ra != null)
            {
                ra.setAngle(angle);
            }
        }


    }

    //STATE.ZERO
    public class ZeroState : IPlayerState
    {
        private PlayerController player;

        public ZeroState(PlayerController player)
        {
            this.player = player;
        }

        public void UpdateState()
        {

        }
    }

    //STATE.GRIND
    public class GrindState : IPlayerState
    {
        private PlayerController player;

        public GrindState(PlayerController player)
        {
            this.player = player;
        }

        public void UpdateState()
        {
            

            if (player.currentSpline != null)
            {
                //this should normalize the speed so its consistent even if the rail is short or long
                //float splineLength = player.currentSpline.CalculateLength();
                //float normalizedSpeed = player.grindSpeed / splineLength;
                //player.progressAlongSpline += normalizedSpeed * Time.deltaTime;
                player.progressAlongSpline += player.grindSpeed * Time.deltaTime;

                Vector3 splinePosition = player.currentSpline.EvaluatePosition(player.progressAlongSpline);
                player.transform.position = new Vector3(splinePosition.x, splinePosition.y + 1f, splinePosition.z);

                Vector3 tangent = player.currentSpline.EvaluateTangent(player.progressAlongSpline);
                if (player.currentSpline.TryGetComponent<Grind>(out var grind))
                {
                    tangent += new Vector3(0, grind.tangentOffset, 0);
                }
                Vector3 up = player.currentSpline.transform.up;

                if (Vector3.Dot(tangent.normalized, up) > 0.99f)
                {
                    up = player.currentSpline.transform.forward;
                }
                if (tangent != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(tangent, up);
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * player.rotationSpeed);
                }

                if (player.progressAlongSpline >= 1f)
                {
                    if (player.specialSplineCams != null)
                    {
                        foreach (var cam in player.specialSplineCams)
                            cam.Priority = 0;
                        player.isOnSpecialSpline = false;
                    }
                    player.SetState(player.freeRoamState);
                    
                }

                if (Input.GetButtonDown("Jump") && !player)
                {
                    player.SetState(player.freeRoamState);
                    player.Jump();
                }
            }
        }


    }

    // (tutorial)
    // STATE.FREEFALL
    public class FreefallState : IPlayerState
    {
        private PlayerController player;

        public FreefallState(PlayerController player)
        {
            this.player = player;
        }

        public void UpdateState()
        {
            player.rb.isKinematic = true;
            player.SetAirFOV();
            player.airFovSet = true;
            player.transform.eulerAngles = Vector3.Lerp(player.transform.eulerAngles, new Vector3(60,0), 80 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "HighGround")
        {
            if (wasInAir && airTime >= minAirTimeForShake && landingImpulse != null)
            {
                float t         = Mathf.InverseLerp(0f, maxAirTime, airTime);
                float easedT    = t * t;   // ease-in
                float velocityY = Mathf.Lerp(minImpulseY, maxImpulseY, easedT);
                landingImpulse.GenerateImpulse(Vector3.up * velocityY);
                Debug.Log($"[LandingShake] airTime={airTime:F2}s, velocityY={velocityY:F2}");
                wasInAir = false;          // ← 重置，避免连续触发
            }
            if (airFovSet)
            {
                StartCoroutine(ResetAirFOVSmooth());
                airFovSet = false;
            }
            Debug.Log("is touching ground now");
            rb.velocity = Vector3.zero;
            StopDive();
            AudioManager.instance.Run();
            AudioManager.instance.GrindStop();

            if (collision.gameObject.tag == "Ground") { moveSpeed = 30f; }
            if (collision.gameObject.tag == "HighGround") { moveSpeed = 100f; }


            // Get the ground normal at the point of contact
            ContactPoint contact = collision.contacts[0];
            Vector3 groundNormal = contact.normal;

            // Calculate the angle between the ground normal and Vector3.up
            float groundAngle = Vector3.SignedAngle(Vector3.up, groundNormal, transform.right);

            // Print based on the angle direction
            if (Mathf.Abs(groundAngle) < 15f && !grounding)
            {
                //HUD.instance.onPlayerTrickHud("GOOD", 10);
                //AudioManager.instance.Land();
                //grounding = true;
                if (RumbleManager.instance != null) { RumbleManager.instance.RumbleForTime(0.2f, 0.1f, 0.5f); }
            }
            else if (groundAngle < 5f && !grounding)
            {
                //HUD.instance.onPlayerTrickHud("OK", 10);
                //moveSpeed -= 1f;
                //AudioManager.instance.BadLand();
                grounding = true;
                if (RumbleManager.instance != null) { RumbleManager.instance.RumbleForTime(0.2f, 0.1f, 0.5f); }
            }
            else if (!grounding)
            {
                //HUD.instance.onPlayerTrickHud("PERFECT", 10);
                //moveSpeed += 2f;
                //AudioManager.instance.GoodLand();
                grounding = true;
                if (RumbleManager.instance != null) { RumbleManager.instance.RumbleForTime(0.2f, 0.1f, 0.5f); }
            }
        }

        if (collision.gameObject.tag == "Grind")
        {
            HUD.instance.onPlayerTrickHud("GRIND", 10);
            AudioManager.instance.Grind();

            SplineContainer spline = collision.gameObject.GetComponent<SplineContainer>();
            grindSpeed = collision.gameObject.GetComponent<LoftRoadBehaviour>().splineSpeed;
            if (spline != null)
            {
                lastGrindCollider = collision.collider;
                Physics.IgnoreCollision(GetComponent<Collider>(), lastGrindCollider, true); // <-- key line
                StartGrind(spline);
            }
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            AudioManager.instance.RunStop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            FlashRed();
            RumbleManager.instance.RumblePulse(0.05f, 0.1f, new Vector2(1, 1), new Vector2(1, 1));
        }

        if (other.CompareTag("Boost"))
        {
            ApplySpeedBoost();
        }

        if (other.CompareTag("Bubble"))
        {
            AudioManager.instance.Pop();
            Destroy(other.gameObject);
        }

    }
}
