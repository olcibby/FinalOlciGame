using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MainCharacterController : MonoBehaviour {

    [SerializeField]
    float m_MovingTurnSpeed = 360;
    [SerializeField]
    float m_StationaryTurnSpeed = 180;
    [SerializeField]
    float m_JumpPower = 50f;
    [Range(1f, 4f)]
    [SerializeField]
    float m_GravityMultiplier = 2f;
    [SerializeField]
    float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField]
    float m_MoveSpeedMultiplier = 1f;
    [SerializeField]
    float m_AnimSpeedMultiplier = 1f;
    [SerializeField]
    float m_GroundCheckDistance = 0.35f;
    [SerializeField]
    public GameObject m_AirPushPrefab;
    [SerializeField]
    public GameObject DoubleJumpMask;
    [SerializeField]
    public GameObject AirPushMask;
    [SerializeField]
    public float m_timeScale;
    [SerializeField]
    public GameObject DoubleJumpMaskHand;
    [SerializeField]
    public GameObject AirPushMaskHand;
    [SerializeField]
    public GameObject DoubleJumpMaskInventory;
    [SerializeField]
    public GameObject AirPushMaskInventory;
    [SerializeField]
    public float climbSpeed = 1;
    public Transform checkReachedTop;

    Rigidbody m_Rigidbody;
    Animator m_Animator;
    bool m_IsGrounded;
    bool m_DoubleJump;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    CapsuleCollider m_Capsule;
    GameObject[] groundChecks;
    bool m_airPushFired;
    public bool wallClimb;
    public Transform climbingWall;
    private bool m_doubleJumpMaskOn;
    private bool m_airPushMaskOn;
    private bool m_swapMasks;
    private bool m_maskAction;
    public bool climbTopEvent;
    private bool climbStateUp;
    private bool climbStateForward;
    private float climbTopSpeed = 2.3f;

    private bool displayMask1InInventory = true;
    private bool displayMask2InInventory = true;

    // Use this for initialization
    void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        m_DoubleJump = true;
        groundChecks = GameObject.FindGameObjectsWithTag("GroundCheck");
        m_Animator = GetComponent<Animator>();
        m_airPushFired = false;
        m_swapMasks = false;
        m_maskAction = false;
    }
	
	// Update is called once per frame
	void Update () {
        Time.timeScale = m_timeScale;
    }

    public void Move(float verticalInput, Vector3 move, bool jump)
    {
        CheckGroundStatus();
        m_Rigidbody.isKinematic = wallClimb && !m_IsGrounded || climbTopEvent;
        if ((!wallClimb || m_IsGrounded) && !climbTopEvent)
        {
            if (!m_airPushFired && !m_maskAction)
            {
                if (move.magnitude > 1f) move.Normalize();
                move = transform.InverseTransformDirection(move);
                move = Vector3.ProjectOnPlane(move, m_GroundNormal);
                m_TurnAmount = Mathf.Atan2(move.x, move.z);
                m_ForwardAmount = move.z;

                ApplyExtraTurnRotation();

                if (move.magnitude > 0f)
                {
                    m_Rigidbody.velocity = new Vector3(transform.forward.x * m_MoveSpeedMultiplier * move.magnitude, m_Rigidbody.velocity.y, transform.forward.z * m_MoveSpeedMultiplier * move.magnitude);
                    m_Animator.SetBool("Running", true);
                }
                else
                {
                    m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
                    m_Animator.SetBool("Running", false);
                }

                // check whether conditions are right to allow a jump:
                if (m_IsGrounded && jump)
                {
                    // jump!
                    m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                    m_IsGrounded = false;
                }
                else
                    if (jump && m_doubleJumpMaskOn && m_DoubleJump && !m_IsGrounded)
                {
                    m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                    m_DoubleJump = false;
                    m_Animator.SetBool("DoubleJump", true);
                }

                HandleAirborneMovement();
            }
            else
            {
                m_Rigidbody.velocity = new Vector3(0, 0, 0);
                if (move.magnitude > 0f)
                    m_Animator.SetBool("Running", true);
                else
                    m_Animator.SetBool("Running", false);
            }
        }
        else
        {
            HandleVerticalInput(verticalInput);
            m_Animator.SetBool("Running", false);
            if (climbingWall != null)
            {
                if (transform.rotation != Quaternion.Euler(new Vector3(0, climbingWall.rotation.eulerAngles.y + 90, 0)))
                transform.rotation = Quaternion.Euler(new Vector3(0, climbingWall.rotation.eulerAngles.y + 90, 0));
            }
        }

        if (climbStateUp && climbTopEvent)
        {
            m_Rigidbody.MovePosition(new Vector3(m_Rigidbody.position.x, m_Rigidbody.position.y + climbTopSpeed * Time.deltaTime, m_Rigidbody.position.z));
        }

        if (climbStateForward && climbTopEvent)
        {
            m_Rigidbody.MovePosition(new Vector3(m_Rigidbody.position.x + transform.forward.x * Time.deltaTime * climbTopSpeed/2, m_Rigidbody.position.y, m_Rigidbody.position.z + transform.forward.z * Time.deltaTime * climbTopSpeed/2));
        }

        DoubleJumpMaskInventory.SetActive(displayMask1InInventory && GameController.mask1Collected);
        AirPushMaskInventory.SetActive(displayMask2InInventory && GameController.mask2Collected);

        m_Animator.SetBool("Climbing", climbingWall);
    }

    void CheckGroundStatus()
    {
        if (groundChecks.Length != 0)
        {
            foreach (GameObject groundCheck in groundChecks)
            {
                int layer_mask = LayerMask.GetMask("Ground");
                RaycastHit hitInfo;
#if UNITY_EDITOR
                // helper to visualise the ground check ray in the scene view
                Debug.DrawLine(groundCheck.transform.position + (Vector3.up * 0.2f), groundCheck.transform.position + (Vector3.up * 0.2f) + (Vector3.down * m_GroundCheckDistance));
#endif
                // 0.1f is a small offset to start the ray from inside the character
                // it is also good to note that the transform position in the sample assets is at the base of the character
                if (Physics.Raycast(groundCheck.transform.position + (Vector3.up * 0.2f), Vector3.down, out hitInfo, m_GroundCheckDistance, layer_mask))
                {
                    m_GroundNormal = hitInfo.normal;
                    m_IsGrounded = true;
                    m_DoubleJump = true;
                    //transform.rotation = Quaternion.LookRotation(transform.forward, hitInfo.normal);
                    //m_Animator.applyRootMotion = true;
                }
                else
                {
                    m_IsGrounded = false;
                    m_GroundNormal = Vector3.up;
                    //m_Animator.applyRootMotion = false;
                }
            }
        }
        if (!m_IsGrounded)
            {
                HandleAirAnimation();
            }
        m_Animator.SetBool("Grounded", m_IsGrounded);

    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);
        
    }

    public void AirPush()
    {
        Instantiate(m_AirPushPrefab, transform.position + transform.forward * 1.5f + Vector3.up * 1.2f, Quaternion.LookRotation(transform.forward, Vector3.up));
    }

    public void AirPushEnd()
    {
        m_Animator.SetBool("AirPush", false);
        m_airPushFired = false;
    }

    public void AirPushAction()
    {
        if (m_IsGrounded && m_airPushMaskOn)
        {
            m_airPushFired = true;
            m_Animator.SetBool("AirPush", true);
        }
    }

    public void ToggleDoubleJumpOn()
    {
        if (!m_maskAction && m_IsGrounded)
        {
            m_maskAction = true;
            m_doubleJumpMaskOn = !m_doubleJumpMaskOn;
            m_airPushMaskOn = false;
            SetMasks();
        }
    }

    public void ToggleAirPushMaskOn()
    {
        if (!m_maskAction && m_IsGrounded)
        {
            m_maskAction = true;
            m_doubleJumpMaskOn = false;
            m_airPushMaskOn = !m_airPushMaskOn;
            SetMasks();
        }
    }

    private void SetMasks()
    {
            if (!AirPushMask.activeSelf && !DoubleJumpMask.activeSelf)
            {
                m_Animator.SetBool("MaskOn", true);
            }
            else
                if ((AirPushMask.activeSelf && m_doubleJumpMaskOn) || (DoubleJumpMask.activeSelf && m_airPushMaskOn))
            {
                m_Animator.SetBool("MaskSwap", true);
            }
            else
            {
                m_Animator.SetBool("MaskOff", true);
            }
    } 

    void HandleAirAnimation()
    {
        if (m_Rigidbody.velocity.y > 0.1f)
        {
            m_Animator.SetBool("Ascending", true);
            m_Animator.SetBool("Descending", false);
        }
        else
        {
            m_Animator.SetBool("Ascending", false);
            m_Animator.SetBool("Descending", true);
        }
    }

    public void PickUpMask()
    {
        DoubleJumpMaskHand.SetActive(m_doubleJumpMaskOn);
        AirPushMaskHand.SetActive(m_airPushMaskOn);

        displayMask1InInventory = !m_doubleJumpMaskOn;
        displayMask2InInventory = !m_airPushMaskOn;

        DoubleJumpMaskInventory.SetActive(!m_doubleJumpMaskOn && GameController.mask1Collected);
        AirPushMaskInventory.SetActive(!m_airPushMaskOn && GameController.mask2Collected);
    }
    public void PlaceMaskOn()
    {
        DoubleJumpMask.SetActive(m_doubleJumpMaskOn);
        AirPushMask.SetActive(m_airPushMaskOn);

        DoubleJumpMaskHand.SetActive(false);
        AirPushMaskHand.SetActive(false);
    }
    public void TakeOffMask()
    {
        DoubleJumpMaskHand.SetActive(DoubleJumpMask.activeSelf);
        AirPushMaskHand.SetActive(AirPushMask.activeSelf);

        DoubleJumpMask.SetActive(false);
        AirPushMask.SetActive(false);
    }
    public void SwapMasks()
    {
        DoubleJumpMaskInventory.SetActive(!m_doubleJumpMaskOn && GameController.mask1Collected);
        AirPushMaskInventory.SetActive(!m_airPushMaskOn && GameController.mask2Collected);

        displayMask1InInventory = !m_doubleJumpMaskOn;
        displayMask2InInventory = !m_airPushMaskOn;

        DoubleJumpMaskHand.SetActive(m_doubleJumpMaskOn);
        AirPushMaskHand.SetActive(m_airPushMaskOn);
    }
    public void PlaceMask()
    {
        DoubleJumpMaskInventory.SetActive(GameController.mask1Collected);
        AirPushMaskInventory.SetActive(GameController.mask2Collected);

        displayMask1InInventory = GameController.mask1Collected;
        displayMask2InInventory = GameController.mask2Collected;

        DoubleJumpMaskHand.SetActive(false);
        AirPushMaskHand.SetActive(false);
    }

    public void TriggerDoubleJump()
    {
        m_Animator.SetBool("DoubleJump", false);
    }

    public void MaskActionEnd()
    {
        m_maskAction = false;
        m_Animator.SetBool("MaskOn", false);
        m_Animator.SetBool("MaskOff", false);
        m_Animator.SetBool("MaskSwap", false);
    }
    private void HandleVerticalInput(float input)
    {
        if (!climbTopEvent)
        {
            float velocityInput = input > 0 ? climbSpeed : input < 0 ? -climbSpeed : 0;
            float climbingAnim = input > 0 ? 1 : input < 0 ? -1 : 0;
            m_Animator.SetFloat("climbingDirection", climbingAnim);

            m_Rigidbody.MovePosition(new Vector3(m_Rigidbody.position.x, m_Rigidbody.position.y + input * Time.deltaTime, m_Rigidbody.position.z));
        }
        if (climbingWall != null)
        {
            if (checkReachedTop.position.y >= climbingWall.GetChild(0).transform.position.y && !climbTopEvent)
            {
                climbTopEvent = true;
                climbStateUp = true;
                m_Animator.SetBool("ClimbEvent", true);
            }
        }
    }

    public void ClimbGoForward()
    {
        climbStateForward = true;
    }

    public void ClimbEnd()
    {
        climbStateForward = false;
        climbStateUp = false;
        wallClimb = false;
        climbTopEvent = false;
        m_Animator.SetBool("ClimbEvent", false);
    }
}
