using UnityEngine;
using System.Collections.Generic;

public class Swinging : MonoBehaviour
{
    #region GasMovement
    [Header("GasMovement")]
    [SerializeField] private float horizontalGasForce;
    [SerializeField] private float forwardGasForce;
    [SerializeField] private float extendCableSpeed;
    #endregion

    #region GasMovement
    [Header("References")]
    [SerializeField] private float maxSwingDistance;
    [SerializeField] private  Rigidbody rb;
    [SerializeField] private KeyCode leftGrap;
    [SerializeField] private KeyCode rightGrap;
    [SerializeField] private Transform cam, player;
    [SerializeField] private LayerMask grapplableLayer;
    [SerializeField] private List<LineRenderer> lr;
    #endregion

    #region AimAssist
    [Header("Aim assist")]
    public float aimAssistSphereCastRadius;
    private List<RaycastHit> predictionHits;
    [SerializeField] private List<Transform> predictionPoint;
    #endregion

    #region 立體機動
    [Header("立體機動裝置")]
    [SerializeField] private List<Transform> gunTips, pointAimers;  // 起點,終點
    private List<Vector3> swingPoints;
    private List<Vector3> currentGrapplePositions;
    private int amountOfSwingPoint = 2;
    private List<SpringJoint> joints;
    private List<bool> swingActive;
    #endregion

    private void Start() 
    {
        ListSetUp();    
    }
    private void Update() 
    {
        // 左繩
        if(Input.GetKeyDown(leftGrap))
            StartSwing(0);
        if(Input.GetKeyUp(leftGrap))
            StopSwing(0);

        // 右繩
        if(Input.GetKeyDown(rightGrap))
            StartSwing(1);
        if(Input.GetKeyUp(rightGrap))
            StopSwing(1);


        if(joints[0] != null || joints[1] != null)
            gasMovement();

        CheckForSwingPoints();
    }
    private void LateUpdate() 
    {
        DrawRope();
    }

    private void StartSwing(int swingIndex)
    {
        // no found
        if(predictionHits[swingIndex].point == Vector3.zero)
            return;

        // start swing
        swingActive[swingIndex] = true;
        PlayerMovement.instance.swinging = true;
        
        swingPoints[swingIndex] = predictionHits[swingIndex].point;
        joints[swingIndex] = player.gameObject.AddComponent<SpringJoint>();
        joints[swingIndex].autoConfigureConnectedAnchor = false;
        joints[swingIndex].connectedAnchor = swingPoints[swingIndex];

        float distanceFromPoint = Vector3.Distance(player.position, swingPoints[swingIndex]);

        // 隨距離拉伸繩索
        joints[swingIndex].maxDistance = distanceFromPoint * 0.8f;
        joints[swingIndex].minDistance = distanceFromPoint * 0.25f;

        // joint variable
        joints[swingIndex].spring = 15f;
        joints[swingIndex].damper = 7f;
        joints[swingIndex].massScale = 4.5f;

        lr[swingIndex].positionCount = 2;
        currentGrapplePositions[swingIndex] = gunTips[swingIndex].position;
    }

    private void StopSwing(int swingIndex)
    {
        PlayerMovement.instance.swinging = false;
        swingActive[swingIndex] = false;

        lr[swingIndex].positionCount = 0;
        Destroy(joints[swingIndex]);
    }    

    private void DrawRope()
    {
        for(int i = 0; i < amountOfSwingPoint; i++)
        {
            if(!swingActive[i])
            {
                lr[i].positionCount = 0;
            }
            else
            {
                // 勾索 anim
                currentGrapplePositions[i] = Vector3.Lerp(currentGrapplePositions[i], swingPoints[i], Time.deltaTime * 8f);

                lr[i].SetPosition(0, gunTips[i].position);
                lr[i].SetPosition(1, currentGrapplePositions[i]);
            }

        }
    }

    private void gasMovement()
    {   
        // gas
        if(Input.GetKey(KeyCode.A))
            rb.AddForce(- PlayerMovement.instance.orientation.right * horizontalGasForce * Time.deltaTime);
        if(Input.GetKey(KeyCode.D))
            rb.AddForce(PlayerMovement.instance.orientation.right * horizontalGasForce * Time.deltaTime);

        if(Input.GetKey(KeyCode.W))
            rb.AddForce(PlayerMovement.instance.orientation.forward * forwardGasForce * Time.deltaTime);


        // find pull direction
        Vector3 pullPoint = Vector3.zero;
        
        if(swingActive[0] && !swingActive[1])
            pullPoint = swingPoints[0];
        if(!swingActive[0] && swingActive[1])
            pullPoint = swingPoints[1];
        if(swingActive[0] && swingActive[1])
        {
            Vector3 direction = swingPoints[1] - swingPoints[0];
            pullPoint = swingPoints[0] + direction * 0.5f;
        }

        // 收放繩
        if(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime > 0)
        {
            // 隨距離產生加速效果
            Vector3 directionToPoint = pullPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardGasForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, pullPoint);

            UpdateJoinsLenths(distanceFromPoint);
        }
        if(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime < 0)
        {
            float extendDistanceFromPoint = Vector3.Distance(transform.position, pullPoint) + extendCableSpeed;

            UpdateJoinsLenths(extendCableSpeed);
        }
        
    }

    private void CheckForSwingPoints()
    {
        for(int i = 0; i < amountOfSwingPoint; i++)
        {
            if(swingActive[i])
                return;

            RaycastHit sphereCastHit;
            Physics.SphereCast(pointAimers[i].position, aimAssistSphereCastRadius, pointAimers[i].forward, out sphereCastHit, maxSwingDistance, grapplableLayer);

            RaycastHit raycastHit;
            Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, grapplableLayer);

            Vector3 realHitPoint;
            // 是否需要自瞄
            if(raycastHit.point != Vector3.zero)
                realHitPoint = raycastHit.point;
            else if(sphereCastHit.point != Vector3.zero)
                realHitPoint = sphereCastHit.point;
            else
                realHitPoint = Vector3.zero;

            // 自瞄光點
            if(realHitPoint != Vector3.zero)
            {
                predictionPoint[i].gameObject.SetActive(true);
                predictionPoint[i].position = realHitPoint;
            }
            else
            {
                predictionPoint[i].gameObject.SetActive(false);
            }

            predictionHits[i] = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
        }
    }

    private void ListSetUp()
    {
        predictionHits = new List<RaycastHit>();
        swingPoints = new List<Vector3>();
        joints = new List<SpringJoint>();
        swingActive = new List<bool>();

        currentGrapplePositions = new List<Vector3>();

        for(int i = 0; i < amountOfSwingPoint; i++)
        {
            predictionHits.Add(new RaycastHit());
            joints.Add(null);
            swingPoints.Add(Vector3.zero);
            swingActive.Add(false);

            currentGrapplePositions.Add(Vector3.zero);
        }
    }

    private void UpdateJoinsLenths(float distanceFromPoint)
    {
        for(int i = 0; i < joints.Count; i++)
        {
            if(joints[i] != null)
            {
                joints[i].maxDistance = distanceFromPoint * 0.8f;
                joints[i].minDistance = distanceFromPoint * 0.25f;
            }
        }
    }
}
