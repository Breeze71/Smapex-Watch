using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private Vector3 moveDirection;
    public Transform orientation;
    public float moveSpeed;
    private Rigidbody rb;
    public float groundDrag;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask GroundMask;
    private bool grounded;
    
    [Header("InputAxis")]
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;   // 確保開始時不會受到旋轉的干擾穿模
    }
    private void Update() 
    {
        MyInput();
        GroundCheck();
    }

    private void FixedUpdate() 
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        //                      上下                                左右
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // ForceMode.Force 施加持續的力。這將導致物體在施加力的方向上加速，直到受到反作用力或其他力的干擾。
        // 以受到其他物體的碰撞和物理力的影響。
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
    }
    
    /* 用於控制玩家在地面和空中時的運動行為 */
    private void GroundCheck()
    {
        // 玩家半個身長加 0.2 確保 raycast 能打到地面
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, GroundMask);

        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }
}
