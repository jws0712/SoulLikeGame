namespace SOUL.Player
{
    using SOUL.Camera;

    //System
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    //UnityEngine
    using UnityEngine;
    using static UnityEngine.GraphicsBuffer;

    public class PlayerView : MonoBehaviour
    {
        [Header("SensSetting")]
        [SerializeField] private float sensDistance = default;
        [SerializeField] private LayerMask sensLayer = default;
        [Header("ViewSetting")]
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private Transform playerViewPos = null;
        [Header("ProjectScript")]
        [SerializeField] private PlayerInput playerInput = null;
        [SerializeField] private CameraController cameraController = null;

        private bool isSens = default;
        private Collider nearEnemy = default;
        private Vector3 forward = default;
        private Vector3 right = default;
        private Vector3 dir = default;



        //프로퍼티
        public bool IsSens => isSens;
        public Vector3 Dir => dir;

        public Collider NearEnemy => nearEnemy;

        private void Update()
        {
            if (playerInput.Horizontal == 0f && playerInput.Vertical == 0f)
            {   
                dir = Vector3.zero;
            }

            transform.position = playerViewPos.position;

            SensEnemy();
            MakeDirection();

            transform.LookAt(transform.position + dir);

            if (isSens && nearEnemy != null)
            {
                transform.LookAt(nearEnemy.transform);
            }
        }


        /// <summary>
        /// 플레이어가 움직일 방향을 만드는 함수
        /// </summary>
        private void MakeDirection()
        {
            forward = mainCamera.transform.forward;
            right = mainCamera.transform.right;

            forward.y = 0f;
            forward.Normalize();

            dir = (forward * playerInput.Vertical + right * playerInput.Horizontal).normalized;
        }


        /// <summary>
        /// 적을 감지하는 함수
        /// </summary>
        private void SensEnemy()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                cameraController.transform.forward = transform.forward;
                cameraController.transform.position = transform.position - transform.forward * cameraController.Dis;
                isSens = !isSens;
            }

            if (isSens)
            {
                
                return;
            }

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, sensDistance);
        }

    }
}


