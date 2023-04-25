using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Swing")]
    public KeyCode swingKey;
    private Vector3 swingPoint;
    private Vector3 currentGrapplePosition;
    private SpringJoint joint;
    [SerializeField] private float maxSwingDistance;

    [Header("GasMovement")]
    [SerializeField] private float horizontalGasForce;
    [SerializeField] private float forwardGasForce;
    [SerializeField] private float extendCableSpeed;
    [SerializeField] private  Rigidbody rb;

    [Header("References")]
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Transform gunTip, cam, player;
    [SerializeField] private LayerMask grapplableLayer;

    [Header("Aim assist")]
    public RaycastHit grappleAim;
    public float aimAssistSphereCastRadius;
    public Transform aimAssistPoint;

    private void Update() 
    {
        if(Input.GetKeyDown(swingKey))
            StartSwing();

        if(Input.GetKeyUp(swingKey))
            StopSwing();

        CheckForSwingPoints();

        if(joint!= null)
            gasMovement();
    }
    private void LateUpdate() 
    {
        DrawRope();
    }

    private void StartSwing()
    {
        // no found
        if(grappleAim.point == Vector3.zero)
            return;

        PlayerMovement.instance.swinging = true;
        
        swingPoint = grappleAim.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        // 隨距離拉伸繩索
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        // joint variable
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }
    private void StopSwing()
    {
        PlayerMovement.instance.swinging = false;

        lr.positionCount = 0;
        Destroy(joint);
    }    

    private void DrawRope()
    {
        if(!joint)
            return;
        
        // 勾索 anim
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void gasMovement()
    {   
        // left and right force
        if(Input.GetKey(KeyCode.A))
            rb.AddForce(- PlayerMovement.instance.orientation.right * horizontalGasForce * Time.deltaTime);
        if(Input.GetKey(KeyCode.D))
            rb.AddForce(PlayerMovement.instance.orientation.right * horizontalGasForce * Time.deltaTime);

        // forward force
        if(Input.GetKey(KeyCode.W))
            rb.AddForce(PlayerMovement.instance.orientation.forward * forwardGasForce * Time.deltaTime);

        // 收放繩
        if(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime > 0)
        {
            // 隨距離產生加速效果
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardGasForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        if(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime < 0)
        {
            float extendDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendDistanceFromPoint * 0.8f;
            joint.minDistance = extendDistanceFromPoint * 0.25f;
        }
        
    }

    private void CheckForSwingPoints()
    {
        if(joint != null)
            return;
        
        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, aimAssistSphereCastRadius, cam.forward, out sphereCastHit, maxSwingDistance, grapplableLayer);

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
            aimAssistPoint.gameObject.SetActive(true);
            aimAssistPoint.position = realHitPoint;
        }
        else
        {
            aimAssistPoint.gameObject.SetActive(false);
        }

        grappleAim = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }
}
