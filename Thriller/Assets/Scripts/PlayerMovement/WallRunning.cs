using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    #region WallRun
    [Header("Wallrunning")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public float wallRunForce;
    [SerializeField] public float wallRunTime;
    [SerializeField] private float wallClimbSpeed;//
    [SerializeField] private float wallRunTimer;
    #endregion

    #region wallJump
    [Header("WallJump")]
    [SerializeField] private float wallJumpFoce;
    [SerializeField] private float wallJumpSideForce;
    [SerializeField] private KeyCode jumpKey;
    #endregion

    #region Detection
    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool isWallLeft;
    private bool isWallRight;
    public KeyCode upwardClimbKey;//
    //public KeyCode downwardClimbKey;//
    private bool upwardClimb;//
    //private bool downwardClimb;//
    #endregion

    #region Exiting
    [Header("Exiting")]
    [SerializeField] private float exitingWallTime;
    private bool exitingWall;
    private float exitingWallTimer;
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Transform orientation;
    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    #endregion

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update() 
    {
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate() 
    {
        if(PlayerMovement.instance.wallrunning)
            WallRunMovement();
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardClimb = Input.GetKey(upwardClimbKey);//
        //downwardClimb = Input.GetKey(downwardClimbKey);//

        // wallrunning
        if((isWallLeft || isWallRight) && (horizontalInput != 0 || verticalInput !=0 ) && !GroundCheck() && !exitingWall)
        {
            if(!PlayerMovement.instance.wallrunning)
            {
                StartWallRun();
                Debug.Log("startWallRun");
            }

            // wallRunTime
            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.fixedDeltaTime;
            }
            if(wallRunTimer <= 0 && PlayerMovement.instance.wallrunning)
            {
                StopWallRun();
                exitingWall = true;
            }

            // wallJump
            if(Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
        }

        // exiting wall (避免前一偵剛跳，下一偵又 wallRun)
        else if(exitingWall)
        {
            if(PlayerMovement.instance.wallrunning)
                StopWallRun();

            if(exitingWallTimer > 0)
                exitingWallTimer -= Time.fixedDeltaTime;

            if(exitingWallTimer <= 0)
                exitingWall = false;
        }

        else
        {
            if(PlayerMovement.instance.wallrunning)
            {
                StopWallRun();
                Debug.Log("stopWallRun");
            }
        }
    }

    #region wallRun
    private void WallRunMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // 法向量和ｚ軸 cross 得出前方向
        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // cross 的只有向前，判斷是否向後
        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // push to curve wall
        if((!isWallLeft && horizontalInput > 0) && !(isWallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        // upward force
        if(upwardClimb)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        /*
        if(downwardClimb)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);    */
    }

    private void StartWallRun()
    {
        PlayerMovement.instance.wallrunning = true;

        wallRunTimer = wallRunTime;
    }
    private void StopWallRun()
    {
        PlayerMovement.instance.wallrunning = false;
        rb.useGravity = true;
    }
    #endregion
    
    #region wallJump
    private void WallJump()
    {
        exitingWall = true;
        exitingWallTimer = exitingWallTime;

        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;

        // 除了往上還要往對面推
        Vector3 JumpForce = transform.up * wallJumpFoce + wallNormal * wallJumpSideForce;

        // reset y even falling
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(JumpForce, ForceMode.Impulse);
    }
    #endregion

    #region wallCheck
    /* ground and wall check */
    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wallLayer);
        isWallLeft = Physics.Raycast(transform.position, - orientation.right, out leftWallHit, wallCheckDistance, wallLayer);
    }
    private bool GroundCheck()
    {
        // 離開地面才能 wallrun
        return Physics.Raycast(transform.position, Vector3.down, minJumpHight, groundLayer);
    }
    #endregion


}
