using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation, firePoint;
    float xRotation;
    float yRotation;

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        firePoint.rotation = Quaternion.Euler(0, 0, 0);
    }

    // LateUpdate 最後操作，避免更改攝影機的旋轉可能會與（例如玩家移動）相互干擾，導致視覺上的抖動或不正確的行為。
    private void LateUpdate() 
    {
        float mouseX = Input.GetAxisRaw("Mouse X")* Time.deltaTime * sensX; // sensitivity
        float mouseY = Input.GetAxisRaw("Mouse Y")* Time.deltaTime * sensY; 

        yRotation += mouseX; //  y  旋轉 y 軸是轉玩家左右
        xRotation -= mouseY; // xz  旋轉 x 軸是轉玩家上下
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 不會仰頭到背後

        // rotate the camera and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);   // 玩家模型面向的左右

        firePoint.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
