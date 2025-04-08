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

    public bool isGrounded; // To check if the player is grounded\
    public bool grounding;

    private Rigidbody rb;
    private bool isDiving = false;
    public float diveForwardSpeed = 5f;
    public float diveFallSpeed = -2f;
    public Vector3 currentSurfaceNormal = Vector3.up;



    //START
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the player's Rigidbody
        freeRoamState = new FreeRoamState(this);
        zeroState = new ZeroState();
        grindState = new GrindState(this);
        currentState = freeRoamState;
        GameObject.Find("CameraControl").GetComponent<Animator>().SetInteger("State", 0);
    }

    //FIXED UPDATE
    void FixedUpdate()
    {
        currentState.UpdateState();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if(!isGrounded) { grounding = false; }

        //print(GameObject.Find("CameraControl").GetComponent<Animator>().GetInteger("State"));
        //print(currentState.ToString());
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
            //Debug.Log("Jump prevented due to steep slope: " + angle);
        }
    }

    public void StartDive()
    {
        isDiving = true;
        rb.velocity = new Vector3(transform.forward.x * diveForwardSpeed, diveFallSpeed, transform.forward.z * diveForwardSpeed);
    }

    public void StopDive()
    {
        isDiving = false;
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
        public void UpdateState()
        {
            // No movement or actions allowed
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
                player.transform.position = new Vector3(splinePosition.x, splinePosition.y + 0.4f, splinePosition.z); 

                player.AlignToSurface();

                Vector3 splineDirection = player.currentSpline.EvaluateTangent(player.progressAlongSpline);
                if (splineDirection != Vector3.zero)
                {
                    player.transform.rotation = Quaternion.LookRotation(splineDirection);
                }

                if (player.progressAlongSpline >= 1f)
                {
                    player.SetState(player.freeRoamState);
                }

                if (Input.GetButtonDown("Jump")) // "Jump" is the default input for spacebar in Unity
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

            // Get the ground normal at the point of contact
            ContactPoint contact = collision.contacts[0];
            Vector3 groundNormal = contact.normal;

            // Calculate the angle between the ground normal and Vector3.up
            float groundAngle = Vector3.SignedAngle(Vector3.up, groundNormal, transform.right);

            // Print based on the angle direction
            if (Mathf.Abs(groundAngle) < 15f && !grounding)
            {
                Debug.Log("OK"); // Flat
                AudioManager.instance.Land();
                grounding = true;
            }
            else if (groundAngle < 0f && !grounding)
            {
                Debug.Log("Bad"); // Going downhill
                moveSpeed -= 2f;
                AudioManager.instance.BadLand();
                grounding = true;
            }
            else if (!grounding)
            {
                Debug.Log("Perfect"); // Going uphill
                moveSpeed += 3f;
                AudioManager.instance.GoodLand();
                grounding = true;
            }
        }


        if (collision.gameObject.tag == "Grind")
        {
            SplineContainer spline = collision.gameObject.GetComponent<SplineContainer>();
            if (spline != null)
            {
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
}
