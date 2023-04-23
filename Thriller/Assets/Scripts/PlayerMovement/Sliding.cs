using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    private Rigidbody rb;
    private PlayerMovement movementScript;

    [Header("Sliging")]
    public KeyCode sprintKey;
    public KeyCode slideKey;
    public float maxSlideTime;
    public float slideForce;
    private float slideTime;

    [Header("Sliding Scale")]
    public float slideYScale;
    private float startYScale;

    private float horizontalInput;
    private float verticalInput;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
        movementScript = GetComponent<PlayerMovement>();

        startYScale = player.localScale.y;
    }

    private void Update() 
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // 跑步才能滑鏟
        if(Input.GetKeyDown(slideKey) && Input.GetKey(sprintKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();
        // stopSlide
        if(Input.GetKeyUp(slideKey) && movementScript.sliding && movementScript.haveMomentum)
            StopSlide();
    }

    private void FixedUpdate() 
    {
        if(movementScript.sliding)
            SlideMovement(); 
    }

    private void SlideMovement()
    {
        Vector3 slideDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // normalSlide
        if(!movementScript.OnSlope() || rb.velocity.y > -0.1)
        {
            rb.AddForce(slideDirection.normalized * slideForce, ForceMode.Force);

            slideTime -= Time.deltaTime;

            if(slideTime <= 0)
                StopSlide();    // 不能一直滑鏟
        }
        else
        {
            rb.AddForce(movementScript.SlopeMoveDirection(slideDirection) * slideForce, ForceMode.Force);
        }
    }
    private void StartSlide()
    {
        movementScript.sliding = true;

        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // 避免浮空

        slideTime = maxSlideTime; // reset slide cd
    }
    private void StopSlide()
    {
        movementScript.sliding = false;
        player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
    }
}
