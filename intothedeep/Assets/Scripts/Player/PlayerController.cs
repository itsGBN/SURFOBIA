using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines; // Import Unity's Spline package

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f;
    public float rotationSpeed = 10f;
    public float grindSpeed = 5f;
    public float jumpHeight = 5f; // Jump height (force)
    public float groundCheckDistance = 0.5f; // Distance to check for ground
    public LayerMask groundLayer; // Layer mask for ground detection

    private IPlayerState currentState;
    private FreeRoamState freeRoamState;
    private ZeroState zeroState;
    private GrindState grindState;

    private SplineContainer currentSpline;
    private float progressAlongSpline = 0f;
    private Collider lastGrindCollider;

    public bool isGrounded; 
    public bool grounding;

    private Rigidbody rb;
    private bool isDiving = false;
    public float diveForwardSpeed = 5f;
    public float diveFallSpeed = -2f;
    public Vector3 currentSurfaceNormal = Vector3.up;



    //START
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        freeRoamState = new FreeRoamState(this);
        zeroState = new ZeroState(this);
        grindState = new GrindState(this);
        currentState = zeroState;
        GameObject.Find("CameraControl").GetComponent<Animator>().SetInteger("State", 2);
    }

    //FIXED UPDATE
    void FixedUpdate()
    {
        currentState.UpdateState();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if(!isGrounded) { grounding = false; }
        print(currentState);
    }
    
    //UPDATE
    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        else if (Input.GetButtonDown("Jump") && !isGrounded)
        {
            StartDive();
        }
        else if (Input.GetButtonUp("Jump") && !isGrounded)
        {
            StopDive();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
            float closestT = GetClosestPointOnSpline(transform.position);
            progressAlongSpline = closestT;
            SetState(grindState);
        }
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
    IEnumerator FlashRed()
    {
        print("OK");
        gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_redPower", 0);
        yield return new WaitForSeconds(2);
        gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_redPower", 1);
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

    //STATE INTERFACE
    public interface IPlayerState
    {
        void UpdateState();
    }


    //STATE.FREEROAM
    public class FreeRoamState : IPlayerState
    {
        private PlayerController player;

        public FreeRoamState(PlayerController player)
        {
            this.player = player;
        }

        public void UpdateState()
        {
            GameObject.Find("CameraControl").GetComponent<Animator>().SetInteger("State", 0);

            float moveInput = Input.GetAxis("Vertical");
            float turnInput = Input.GetAxis("Horizontal");

            player.AlignToSurface();

            // Get direction the player is facing and project it onto the surface plane
            Vector3 inputDirection = player.transform.forward;
            Vector3 flattenedDirection = Vector3.ProjectOnPlane(inputDirection, player.currentSurfaceNormal).normalized;

            // Apply movement
            Vector3 moveDirection = flattenedDirection * moveInput * player.moveSpeed * Time.deltaTime;
            player.transform.position += moveDirection;

            // Rotate left/right
            player.transform.Rotate(Vector3.up, turnInput * player.turnSpeed * Time.deltaTime);
        }

    }

    //STATE.ZERO
    public class ZeroState : IPlayerState
    {
        private PlayerController player;
        private bool free = false;
        public ZeroState(PlayerController player)
        {
            this.player = player;
        }

        public void UpdateState()
        {
            GameObject.Find("CameraControl").GetComponent<Animator>().SetInteger("State", 2);

            if (Input.GetKeyDown(KeyCode.Escape) && !MainMenuEvents.instance.isTrasitioning)
            {
                free = true;
            }
            if (free)
            {
                player.SetState(player.freeRoamState);
            }
            print(free);
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
            GameObject.Find("CameraControl").GetComponent<Animator>().SetInteger("State", 1);

            if (player.currentSpline != null)
            {
                player.progressAlongSpline += player.grindSpeed * Time.deltaTime;

                Vector3 splinePosition = player.currentSpline.EvaluatePosition(player.progressAlongSpline);
                player.transform.position = new Vector3(splinePosition.x, splinePosition.y + 1f, splinePosition.z);

                Vector3 tangent = player.currentSpline.EvaluateTangent(player.progressAlongSpline);
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rb.velocity = Vector3.zero;
            StopDive();
            AudioManager.instance.Run();
            AudioManager.instance.GrindStop();


            // Get the ground normal at the point of contact
            ContactPoint contact = collision.contacts[0];
            Vector3 groundNormal = contact.normal;

            // Calculate the angle between the ground normal and Vector3.up
            float groundAngle = Vector3.SignedAngle(Vector3.up, groundNormal, transform.right);

            // Print based on the angle direction
            if (Mathf.Abs(groundAngle) < 15f && !grounding)
            {
                HUD.instance.onPlayerTrickHud("GOOD");
                AudioManager.instance.Land();
                grounding = true;
            }
            else if (groundAngle < 5f && !grounding)
            {
                HUD.instance.onPlayerTrickHud("OK");
                moveSpeed -= 1f;
                AudioManager.instance.BadLand();
                grounding = true;
            }
            else if (!grounding)
            {
                HUD.instance.onPlayerTrickHud("PERFECT");
                moveSpeed += 2f;
                AudioManager.instance.GoodLand();
                grounding = true;
            }
        }

        if (collision.gameObject.tag == "Grind")
        {
            HUD.instance.onPlayerTrickHud("GRIND");
            AudioManager.instance.Grind();

            SplineContainer spline = collision.gameObject.GetComponent<SplineContainer>();
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
        if(other.gameObject.tag == "Obstacle")
        {
            StartCoroutine(FlashRed());
        }
    }
}
