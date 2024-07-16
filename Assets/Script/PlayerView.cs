namespace SOUL.Player
{

    //System
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    //UnityEngine
    using UnityEngine;

    public class PlayerView : MonoBehaviour
    {
        [Header("SensSetting")]
        [SerializeField] private float sensDistance = default;
        [SerializeField] private LayerMask sensLayer = default;
        [Header("ViewSetting")]
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private PlayerInput playerInput = null;
        [SerializeField] private Transform playerViewPos = null;

        private bool isSens = default;
        private Collider nearEnemy = default;
        private Vector3 forward = default;
        private Vector3 right = default;


        //������Ƽ
        public bool IsSens => isSens;
        public Collider NearEnemy => nearEnemy;


        public Vector3 dir = default;

        private void Update()
        {
            transform.position = playerViewPos.position;

            SensEnemy();
            MakeDirection();

            transform.LookAt(transform.position + dir);

            if (isSens)
            {
                transform.LookAt(nearEnemy.transform);
            }
        }


        /// <summary>
        /// �÷��̾ ������ ������ ����� �Լ�
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
        /// ���� �����ϴ� �Լ�
        /// </summary>
        private void SensEnemy()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
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


