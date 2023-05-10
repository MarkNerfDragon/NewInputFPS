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
    public float standingSpeed = 6f;

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

    Vector2 input;

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
    }

    private void PlayerMove()
    {
        grounded = Physics.CheckSphere(groundCheck.position, distanceToGround, whatIsGround);

        input = controls.PlayerActions.Movement.ReadValue<Vector2>();

        Vector3 movement = (input.y * transform.forward) + (input.x * transform.right);

        if(grounded)
            rb.AddForce(movement.normalized * moveSpeed, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(movement.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        
        //camera tilt
        tilt = Mathf.Lerp(tilt, -input.x, Time.deltaTime * tiltSpeed);

        //handle drag
        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        SpeedControl();

        bool isJumping = controls.PlayerActions.Jump.ReadValue<float>() > 0.1f;

        if(isJumping)
        {
            if(grounded && readyToJump)
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

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
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
