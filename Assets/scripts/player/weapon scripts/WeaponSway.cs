using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    Vector2 mouseInput;
    Vector3 rotation;
    Quaternion FinalRotation;
    [SerializeField] Transform Orientation;
    [SerializeField] Rigidbody rb;
    [SerializeField] float MaxBonusPos = 1;
    [SerializeField] float Power = 15;
    [SerializeField] float speed = 5;
    [SerializeField] float Maximum = 10;
    Quaternion originRotation;
    Vector3 originPosition;
    void Start()
    {
        originPosition = transform.localPosition;
        originRotation = transform.localRotation;
    }
    void LateUpdate()
    {
        GetInput();
        Quaternion XRotation = Quaternion.AngleAxis(-Mathf.Clamp(mouseInput.y * Power, -Maximum, Maximum), Vector3.right);
        Quaternion YRotation = Quaternion.AngleAxis(Mathf.Clamp(mouseInput.x * Power, -Maximum, Maximum), Vector3.up);
        // Quaternion XRotation = Quaternion.AngleAxis(rotation.x, transform.right);

        FinalRotation = YRotation * XRotation * originRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, FinalRotation, Time.deltaTime * speed);

        Vector3 targetPos = originPosition - Mathf.Clamp(rb.velocity.y * 0.02f, -MaxBonusPos, MaxBonusPos) * Vector3.up;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 2);
    }
    void GetInput()
    {
        mouseInput.x = Input.GetAxis("Mouse X");
        mouseInput.y = Input.GetAxis("Mouse Y");
    }
}
