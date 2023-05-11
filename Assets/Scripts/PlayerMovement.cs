using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private InputMaster controls;
    private Rigidbody rb;
    public float moveSpeed = 6f;
    [SerializeField] float walkSpeed = 5f;
    public float wallrunSpeed = 6.5f;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    public float groundDrag;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float distanceToGround;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Sprinting")]
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float acceleration = 10f;

    [Header("Crouching")]
    public float crouchScale;
    public float playerScale;
    public float crouchSpeed;

    Vector2 input;

    public bool isJumping;
    bool isCrouching;
    bool isSprinting;
    public bool wallrunning;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Awake()
    {
        controls = new InputMaster();
    }

    void Update()
    {
        PlayerMove();

        isJumping = controls.PlayerActions.Jump.ReadValue<float>() > 0.1f;
        isCrouching = controls.PlayerActions.Crouch.ReadValue<float>() > 0.1f;
        isSprinting = controls.PlayerActions.Sprinting.ReadValue<float>() > 0.1f;
    }

    private void PlayerMove()
    {
        grounded = Physics.CheckSphere(groundCheck.position, distanceToGround, whatIsGround);

        input = controls.PlayerActions.Movement.ReadValue<Vector2>();

        Vector3 movement = (input.y * transform.forward) + (input.x * transform.right);

        if (grounded)
            rb.AddForce(movement.normalized * moveSpeed, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(movement.normalized * moveSpeed * airMultiplier, ForceMode.Force);

        isJumping = controls.PlayerActions.Jump.ReadValue<float>() > 0.1f;
        isCrouching = controls.PlayerActions.Crouch.ReadValue<float>() > 0.1f;

        //handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        //handle crouch
        if (isCrouching && !isJumping || !wallrunning)
            StartCrouch();
        if (!isCrouching || wallrunning)
            StopCrouch();

        SpeedControl();

        if (isJumping)
        {
            if (grounded && readyToJump && !isCrouching)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        readyToJump = true;
    }

    private void StartCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchScale, transform.localScale.z);
    }

    private void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, playerScale, transform.localScale.z);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        isSprinting = controls.PlayerActions.Sprinting.ReadValue<float>() > 0.1f;

        if (isSprinting && !isCrouching)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }

        if(isCrouching)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }

        if(wallrunning)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, wallrunSpeed, acceleration * Time.deltaTime);
        }
        else
        {

        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
