namespace SOUL.Camera
{
    using SOUL.Player;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public class CameraController : MonoBehaviour
    {
        [Header("CameraSetting")]
        [SerializeField] private float rotSensitive = 3f;
        [SerializeField] private float dis = 5f;
        [SerializeField] private float RotationMin = -10f;
        [SerializeField] private float RotationMax = 80f;
        [SerializeField] private Transform target = null;
        [SerializeField] private Camera mainCamera = null;
        //[SerializeField] private float cameraChaseTime = default;

        [Header("Script")]
        [SerializeField] private PlayerView playerView = null;

        private float Yaxis = default;
        private float Xaxis = default;
        private Vector3 targetRotation = default;

        private void LateUpdate()
        {
            if (playerView.IsSens == false)
            {
                CameraMovement();
            }
            else if(playerView.IsSens == true && playerView.NearEnemy != null)
            {
                LockDir();
            }
        }

        /// <summary>
        /// 마우스에 따라 카메라를 움직이는 함수
        /// </summary>
        private void CameraMovement()
        {
            Yaxis += Input.GetAxis("Mouse X") * rotSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive;

            Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

            targetRotation = new Vector3(Xaxis, Yaxis);
            this.transform.eulerAngles = targetRotation;

            transform.position = target.position - transform.forward * dis;

            mainCamera.transform.LookAt(target.position);
            
        }

        private void LockDir()
        {

            transform.forward = target.forward;
            transform.position = target.position - transform.forward * dis;
            Debug.Log("고정");
        }
    }
}


