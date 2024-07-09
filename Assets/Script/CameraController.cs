using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float rotSensitive = 3f;
    [SerializeField] private float dis = 5f;
    [SerializeField] private float RotationMin = -10f;
    [SerializeField] private float RotationMax = 80f;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private Transform target;

    private float Yaxis;
    private float Xaxis;

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
