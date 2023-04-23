using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement")]
    public Transform orientation;
    public float walkSpeed;
    private Vector3 moveDirection;
    private float moveSpeed;

    [Header("Sprint")]
    public KeyCode sprintKey;
    public float sprintSpeed;
    
    [Header("Crouching")]
    public KeyCode crouchKey;
    public float crouchSpeed;
    public float crouchYScale;
    private float OriginYScale;

    [Header("Jump")]
    public KeyCode jumpKeyCode;
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier; // 空中移速
    private bool canJump = true;

    [Header("Slpoe")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitngSlope;

    [Header("GroundCheck")]
    public float groundDrag;    // 摩擦力
    public float playerHeight;
    public LayerMask GroundMask;
    private bool grounded;
    
    [Header("InputAxis")]
    private float horizontalInput;
    private float verticalInput;

    [Header("MovementState")]
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;   // 確保開始時不會受到旋轉的干擾穿模

        OriginYScale = transform.localScale.y;
    }
    
    private void Update() 
    {
        // 玩家半個身長加 0.2 確保 raycast 能打到地面
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, GroundMask);

        PlayerInput();
        SpeedControl();
        StateController();
        GroundCheck();
    }
    
    private void FixedUpdate() 
    {
        MovePlayer();
    }
    
    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //jump
        if(Input.GetKey(jumpKeyCode) && canJump && grounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(JumpReset), jumpCD);            
        }
        // crouh
        if(Input.GetKeyDown(crouchKey))
        {
            // 縮小一半，但會因為 上下都縮小而滯空
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            // 沒 getKeyDown 會一直下壓
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        // stop crouch
        if(Input.GetKeyUp(crouchKey))
        {
            // 縮小一半，但會因為 上下都縮小而滯空
            transform.localScale = new Vector3(transform.localScale.x, OriginYScale, transform.localScale.z);
        }
    }

    private void StateController()
    {
        /* 和 playerInput 分開是因為 getKey, getKeyDown, getKeyUp 問題*/
        if(Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        
        // Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if(grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
    
    private void MovePlayer()
    {
        // moveDirection
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(OnSlope() && exitngSlope)
        {
            rb.AddForce(SlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        }

        // ForceMode.Force 施加持續的力。這將導致物體在施加力的方向上加速，直到受到反作用力或其他力的干擾。
        // 以受到其他物體的碰撞和物理力的影響。
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);


        //rb.useGravity = !OnSlope(); // 避免站在斜坡上會自動滑
    }
    
    /* limit velocity */
    private void SpeedControl()
    {
        // limiting speed on slope
        if(OnSlope() && !exitngSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or air
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;  // 依照 x, z 軸權重 * moveSpeed

                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    /* 用於控制玩家在地面和空中 */
    private void GroundCheck()
    {
        if(grounded)
            rb.drag = groundDrag;   // 著地時，有摩擦力(避免 addForce 無限增加)
        else
            rb.drag = 0;
    }

    private void Jump()
    {
        exitngSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpReset()
    {
        canJump = true;

        exitngSlope = false;
    }

    private bool OnSlope()
    {                                                     // store what it hit
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); // normal 為被擊中物的法線

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 SlopeMoveDirection()
    {
        // 將 Direction 投影到 slopeHit 平面上
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;   // 剛好向斜坡下
    }
}
