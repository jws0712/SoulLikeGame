namespace SOUL.Camera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        [Header("SensSetting")]
        [SerializeField] private float sensDistance = default;
        [SerializeField] private LayerMask sensLayer = default;

        [Header("CamearSetting")]
        [SerializeField] private float rotSensitive = 3f;
        [SerializeField] private float dis = 5f;
        [SerializeField] private float RotationMin = -10f;
        [SerializeField] private float RotationMax = 80f;
        [SerializeField] private float smoothTime = 0.12f;
        [SerializeField] private float spinSpeed = default;

        public Transform player;
        public Transform cameraPos;


        private float Yaxis;
        private float Xaxis;

        private Vector3 targetRotation;
        private Vector3 currentVel;
        private Quaternion targetRot;
        private Vector3 direction;

        public bool isSens;
        public Collider nearEnemy;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            SensEnemy();
        }



        void LateUpdate()
        {
            if (isSens)
            {
                LookEnemy();
                return;
            }

            CameraMovement();
        }

        /// <summary>
        /// 적을 쳐다보는 함수
        /// </summary>
        private void SensEnemy()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                isSens = !isSens;
            }

            if (!isSens)
            {


                Collider[] enemys = Physics.OverlapSphere(transform.position, sensDistance, sensLayer);

                if (enemys.Length > 0)
                {
                    float nearDistance = Vector3.Distance(transform.position, enemys[0].transform.position);
                    nearEnemy = enemys[0];

                    foreach (Collider col in enemys)
                    {
                        float distacne = Vector3.Distance(transform.position, col.transform.position);

                        if (distacne < nearDistance)
                        {
                            nearDistance = distacne;
                            nearEnemy = col;
                        }
                    }
                }
            }
        }

        private void LookEnemy()
        {
            transform.position = player.position - transform.forward * dis;

            direction = nearEnemy.transform.position - transform.position;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            targetRot = Quaternion.Euler(0, targetAngle, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * spinSpeed);
        }

        /// <summary>
        /// 마우스에 따라 카메라를 움직이는 함수
        /// </summary>
        private void CameraMovement()
        {
            Yaxis += Input.GetAxis("Mouse X") * rotSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive;

            Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

            targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
            this.transform.eulerAngles = targetRotation;

            transform.position = player.position - transform.forward * dis;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, sensDistance);
        }
    }
}


