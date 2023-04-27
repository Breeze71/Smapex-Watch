using UnityEngine;
using Unity.Netcode;
using Unity.Collections;    // fixedString
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance
    {
        get; 
        set;
    }

    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    private float moveSpeed;
    private Vector3 moveDirection;
    public Transform orientation;

    [Header("Sprint")]
    public KeyCode sprintKey;
    [SerializeField] private float sprintSpeed;
    
    [Header("Crouching")]
    public KeyCode crouchKey;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float OriginYScale;

    [Header("Jump")]
    public KeyCode jumpKeyCode;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCD;
    [SerializeField] private float airMultiplier; // 空中移速
    private bool canJump = true;

    [Header("SlpoeMove")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitngSlope;

    [Header("Slide")]
    public bool haveMomentum;   // 避免滑到一半因為 slideCD站起來
    public bool sliding;
    [SerializeField] private float slideSpeed;

    [Header("Wallrun")]
    public bool wallrunning;
    [SerializeField] private float wallrunSpeed;
    
    [Header("Grapple")]
    [SerializeField] private float swingSpeed;
    public bool activeGrapple;
    public bool swinging;

    [Header("Movemetum")]
    [SerializeField] private float speedIncreaseMultiple;
    [SerializeField] private float slopeIncreaseMutiple;
    private float expectedMoveSpeed;
    private float finalExpectedMoveSpeed;

    [Header("GroundCheck")]
    [SerializeField] private float groundDrag;    // 摩擦力
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private bool grounded;
    
    [Header("InputAxis")]
    private float horizontalInput;
    private float verticalInput;

    [Header("MovementState")]
    public MovementState state;
    public enum MovementState
    {
        swinging,
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air,
    }

    private void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;   // 確保開始時不會受到旋轉的干擾穿模

        OriginYScale = transform.localScale.y;
    }   
    
    /*
    // netWork variable
    private NetworkVariable<int> text1 = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<MyCustomData> text2 = new NetworkVariable<MyCustomData>(
        new MyCustomData
        {
            _int = 56,
            _bool = true,
        },
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // struct for variable
    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
    // start()
    public override void OnNetworkSpawn()
    {
        text2.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + ";" + newValue._int + ";" + newValue._bool + ";" + newValue.message);
        };
    }
    */

    private void Update() 
    {
        /*
        // not owner
        if(!IsOwner)
            return;
        if(Input.GetKeyDown(KeyCode.T))
        {
            text2.Value = new MyCustomData{
                _int = 10,
                _bool = false,
                message = "wow"
            };
        }*/

        /**/
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
        if(Input.GetKeyDown(jumpKeyCode) && canJump && grounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(JumpReset), jumpCD);            
        }
        // crouh
        if(Input.GetKeyDown(crouchKey) && !Input.GetKey(sprintKey)) // 跑步時不會變蹲
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

        // wallrunning
        if(wallrunning)
        {
            state = MovementState.wallrunning;
            expectedMoveSpeed = wallrunSpeed;
        }

        // sliding
        else if(sliding)
        {
            state = MovementState.sliding;
            
            // 在斜坡上滑鏟控制
            if((OnSlope() && rb.velocity.y < 0.1f))
            {
                expectedMoveSpeed = slideSpeed;

                //haveMomentum = true;    // 避免滑到一半因為 slideCD站起來 
            }
            else
            {
                expectedMoveSpeed = sprintSpeed;
            }
        }

        // swinging
        else if(swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
        }

        // crouching
        else if(grounded && Input.GetKey(crouchKey)) // 滑鏟時不會變蹲速
        {
            state = MovementState.crouching;
            expectedMoveSpeed = crouchSpeed;
        }

        // sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            expectedMoveSpeed = sprintSpeed;
        }

        // walking
        else if(grounded)
        {
            state = MovementState.walking;
            expectedMoveSpeed = walkSpeed;
        }

        // floating
        else
        {
            state = MovementState.air;
        }

        // sprint = 10, walk = 7, 10 - 7 = 3
        // quick change between sprint and walk
        // slowly change between fast and slow
        if(Mathf.Abs(expectedMoveSpeed - finalExpectedMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothMoveSpeed());
        }
        else
        {
            // 沒 movementum 後直接變成 expectSpeed
            moveSpeed = expectedMoveSpeed;

            //haveMomentum = false;
        }

        finalExpectedMoveSpeed = expectedMoveSpeed;
    }
    
    private void MovePlayer()
    {
        // 避免牛頓詐屍
        if(swinging)
            return;

        // moveDirection
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(OnSlope() && exitngSlope)
        {
            rb.AddForce(SlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
        }

        // ForceMode.Force 施加持續的力。這將導致物體在施加力的方向上加速，直到受到反作用力或其他力的干擾。
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
        else// if(!swinging)
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;  // 依照 x, z 軸權重 * moveSpeed

                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    /* keep the momentum */
    private IEnumerator SmoothMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(expectedMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while(time < difference)
        {
            // 由最高速度開始遞減至常規速度
            moveSpeed = Mathf.Lerp(startValue, expectedMoveSpeed, time / difference);

            if(OnSlope())
            {
                // 隨角度增加 movmentum
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float sloopAngleIncrease = 1 + (slopeAngle / 90f);
                
                time += Time.fixedDeltaTime * speedIncreaseMultiple * slopeIncreaseMutiple * sloopAngleIncrease;
            }
            else
            {
                time += Time.fixedDeltaTime * speedIncreaseMultiple;
            }

            yield return null;
        }

        moveSpeed = expectedMoveSpeed;
    }
    
    private void GroundCheck()
    {
        // 玩家半個身長加 0.2 確保 raycast 能打到地面
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, GroundMask);

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

    public bool OnSlope()
    {                                                     // store what it hit
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); // normal 為被擊中物的法線

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 SlopeMoveDirection(Vector3 direction)
    {
        // 將 Direction 投影到 slopeHit 平面上
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;   // 剛好向斜坡下
    }
}
