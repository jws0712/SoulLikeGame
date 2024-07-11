namespace SOUL.Player
{
    using SOUL.Camera;

    //System
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Unity.VisualScripting;
    using Unity.VisualScripting.FullSerializer;

    //UnityEngine
    using UnityEngine;
    using UnityEngine.Assertions.Must;
    using static UnityEditor.Searcher.SearcherWindow.Alignment;
    using static UnityEngine.GraphicsBuffer;

    //Project

    
    public class Player : MonoBehaviour
    {
        [Header("PlayerMovementSetting")]
        [SerializeField] private float WalkSpeed = default;
        [SerializeField] private float sprintSpeed = default;
        [SerializeField] private float spinSpeed = default;

        [Header("PlayerCameraSetting")]
        [SerializeField] private GameObject mainCamera = default;
        [SerializeField] private CameraController cameraCon = default;


       

        //private variable
        private float moveSpeed;
        private float currentVelocity;
        private float targetAngle;
        private float angle;
        private bool isSens;
        private float horizontal;
        private float vertical;

        private Rigidbody rb;
        private Animator anim;

        private Vector3 dir;
        private Vector3 lastDirection;
        private Vector3 forward;
        private Vector3 right;



        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();

            moveSpeed = WalkSpeed;
        }
        

        private void Update()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if(cameraCon.isSens == true )
            {
                Vector3 dir = cameraCon.nearEnemy.transform.position - transform.position;

                PlayerRotate(dir);

                lastDirection = dir;

            }

            if (horizontal == 0f && vertical == 0f)
            {
                dir = Vector3.zero;
                
                moveSpeed = 0f;


                anim.SetBool("IsMove", false);
                anim.SetFloat("MoveSpeed", moveSpeed);

                PlayerRotate(lastDirection);

                return;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = sprintSpeed;
            }
            else
            {
                moveSpeed = WalkSpeed;
            }

            MakeDirection();

            anim.SetBool("IsMove", true);
            anim.SetFloat("MoveSpeed", moveSpeed);
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * dir);
        }

        /// <summary>
        /// 플레이어를 회전시키는 함수
        /// </summary>
        private void PlayerRotate(Vector3 direction)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * spinSpeed);
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

            dir = (forward * vertical + right * horizontal).normalized;

            lastDirection = dir;

            if (cameraCon.isSens == false)
            {
                PlayerRotate(dir);
            }
        }
    }
}

