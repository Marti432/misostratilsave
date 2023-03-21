using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] Transform Orientation;
    [SerializeField] MovementController mc;
    [SerializeField] Camera cam;
    [SerializeField] float MouseSens = 2000f;
    Vector2 Rotation;
    Vector3 originPos;
    public float fov = 90;
    [HideInInspector] public float Zangle;
    [HideInInspector] public float plusFov = 0;
    void Start()
    {
        originPos = transform.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (IGManagerUI.isPaused) return;
        float mouseX = Input.GetAxis("Mouse X") * MouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSens;

        Rotation.x -= mouseY;
        Rotation.y += mouseX;
        
        Rotation.x = Mathf.Clamp(Rotation.x, -90f, 90f);

        Vector3 targetPos = originPos + Vector3.up * Mathf.Clamp(mc.rb.velocity.y * 0.2f, -2, 2);

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 10);

        switch (mc.state)
        {
            case States.Wallrunning:
                Zangle = mc.wallrunMovement.LerpCamtoLeft > 0 ?
                Mathf.Lerp(Zangle, -15, Time.deltaTime * 6) : Mathf.Lerp(Zangle, 15, Time.deltaTime * 6);
                plusFov = Mathf.Lerp(plusFov, Mathf.Clamp(mc.rb.velocity.magnitude, 0, 10), Time.deltaTime * 10);
            break;
            case States.Walking:
                Zangle = Mathf.Lerp(Zangle, -Mathf.Clamp(Orientation.InverseTransformVector(mc.rb.velocity).x, -2, 2), Time.deltaTime * 10);
                plusFov = Mathf.Lerp(plusFov, Mathf.Clamp(mc.Orientation.InverseTransformVector(mc.rb.velocity).z, -5, 10), Time.deltaTime * 10);
            break;
            case States.Sprinting:
                Zangle = Mathf.Lerp(Zangle, -Mathf.Clamp(Orientation.InverseTransformVector(mc.rb.velocity).x, -2, 2), Time.deltaTime * 10);
                plusFov = Mathf.Lerp(plusFov, Mathf.Clamp(mc.Orientation.InverseTransformVector(mc.rb.velocity).z, -5, 10), Time.deltaTime * 10);
            break;
            case States.AirMovement:
                Zangle = Mathf.Lerp(Zangle, 0, Time.deltaTime * 6);
                plusFov = Mathf.Lerp(plusFov, Mathf.Clamp(mc.rb.velocity.magnitude, 0, 10), Time.deltaTime * 10);
            break;
        }


        transform.rotation = Quaternion.Euler(Rotation.x, Rotation.y, Zangle);
        Orientation.rotation = Quaternion.Euler(Orientation.rotation.x, Rotation.y, Orientation.rotation.z);
        cam.fieldOfView = plusFov + fov;
    }
}
