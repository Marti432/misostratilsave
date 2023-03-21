using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercam : MonoBehaviour
{
    [SerializeField] Transform Orientation;
    public static playercam Instance;
    [SerializeField] Camera cam;
    [SerializeField] float MouseSens = 2000f;
    Vector2 Rotation;
    public float fov = 90;
    [HideInInspector] public float Zangle;
    [HideInInspector] public float plusFov = 0;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
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

        transform.rotation = Quaternion.Euler(Rotation.x, Rotation.y, Zangle);
        Orientation.rotation = Quaternion.Euler(Orientation.rotation.x, Rotation.y, Orientation.rotation.z);
        cam.fieldOfView = plusFov + fov;
    }
}
