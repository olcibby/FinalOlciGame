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
    float m_GroundCheckDistance = 0.1f;
    [SerializeField]
    GameObject m_AirPushPrefab;

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
    Transform bottom;
    bool m_airPushFired;

    // Use this for initialization
    void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        m_DoubleJump = true;
        bottom = transform.FindChild("bottom");
        m_Animator = GetComponent<Animator>();
        m_airPushFired = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move(Vector3 move, bool jump)
    {
        if (!m_airPushFired)
        {
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
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
                m_GroundCheckDistance = 0.1f;
            }
            else
                if (jump && m_DoubleJump && !m_IsGrounded)
            {
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_DoubleJump = false;
                m_GroundCheckDistance = 0.1f;
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

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(bottom.position + (Vector3.up * 0.05f), bottom.position + (Vector3.up * 0.05f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(bottom.position + (Vector3.up * 0.05f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            m_DoubleJump = true;
            //m_Animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            //m_Animator.applyRootMotion = false;
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
        if (m_IsGrounded)
        {
            m_airPushFired = true;
            m_Animator.SetBool("AirPush", true);
        }
    }

    public void HandleAirAnimation()
    {
        if (m_Rigidbody.velocity.y > 0)
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


}
