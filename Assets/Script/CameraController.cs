using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float Yaxis;
    public float Xaxis;

    public Transform target;

    private const float rotSensitive = 3f;
    private const float dis = 5f;
    private const float RotationMin = -10f;
    private const float RotationMax = 80f;
    private const float smoothTime = 0.12f;

    private Vector3 targetRotation;
    private Vector3 currentVel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        Yaxis += Input.GetAxis("Mouse X") * rotSensitive;
        Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive;

        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        transform.position = target.position - transform.forward * dis;
    }
}
