using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public float wallRunForce;
    public float wallRunTime;
    public float wallClimbSpeed;//
    private float wallRunTimer;

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

    [Header("References")]
    public Transform orientation;
    private PlayerMovement movementScript;
    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
        movementScript = GetComponent<PlayerMovement>();
    }
    private void Update() 
    {
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate() 
    {
        if(movementScript.wallrunning)
            WallRunMovement();
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardClimb = Input.GetKey(upwardClimbKey);//
        //downwardClimb = Input.GetKey(downwardClimbKey);//

        // wallrunning
        if((isWallLeft || isWallRight) && (horizontalInput != 0 || verticalInput !=0 ) && !GroundCheck())
        {
            if(!movementScript.wallrunning)
                StartWallRun();
        }
        else
        {
            if(movementScript.wallrunning)
                StopWallRun();
        }
    }

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
        movementScript.wallrunning = true;
    }
    private void StopWallRun()
    {
        movementScript.wallrunning = false;
        rb.useGravity = true;
    }

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
}
