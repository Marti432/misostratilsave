using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bobbing : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] MovementController mc;
    [SerializeField] Transform[] transforms;
    [Header("state")]
    float sinTime;
    [Header("Bobbing")]
    [SerializeField] float BobbingSpeed;
    [SerializeField] float bobbingIntensityY, bobbingIntensityX;
    float LerpVarX, LerpVarZ;
    Vector3[] originalOffset;
    Vector3[] camOffset;
    void Start()
    {
        originalOffset = new Vector3[transforms.Length];
        camOffset = new Vector3[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            originalOffset[i] = transforms[i].localPosition;
        }
    }
    void LateUpdate()
    {
        Bobbing();
    }

    void Bobbing()
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            if (mc.useBobbing)
            {
                sinTime += Time.deltaTime * BobbingSpeed * Mathf.Clamp(rb.velocity.magnitude * 0.25f, 0, 2.5f);

                if (i == 0)
                camOffset[i] = Vector3.Lerp(camOffset[i], new Vector3 (
                    originalOffset[i].x + bobbingIntensityX * Mathf.Cos(sinTime) * bobbingIntensityX,
                    originalOffset[i].y + bobbingIntensityY * -Mathf.Abs(Mathf.Sin(sinTime)),
                    originalOffset[i].z ), Time.deltaTime * 2);
                else
                camOffset[i] = Vector3.Lerp(camOffset[i], new Vector3 (
                    originalOffset[i].x + bobbingIntensityX * Mathf.Cos(sinTime) * bobbingIntensityX * Mathf.Clamp(rb.velocity.magnitude * 0.25f, 0, 2.5f),
                    originalOffset[i].y + bobbingIntensityY * -Mathf.Abs(Mathf.Sin(sinTime)),
                    originalOffset[i].z ), Time.deltaTime * 2);
                


                transforms[i].localPosition = camOffset[i];
            }
            else
            {
                sinTime = 0;
                camOffset[i] = Vector3.Lerp(camOffset[i], originalOffset[i], Time.deltaTime * 2);
                transforms[i].localPosition = camOffset[i];
            }
        }
    }
}
