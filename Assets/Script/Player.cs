namespace SOUL.Player
{

    //System
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Unity.VisualScripting;
    using Unity.VisualScripting.FullSerializer;

    //UnityEngine
    using UnityEngine;
    using UnityEngine.Assertions.Must;
    
    public class Player : MonoBehaviour
    {
        [Header("PlayerMovementSetting")]
        [SerializeField] private float WalkSpeed = default;
        [SerializeField] private float sprintSpeed = default;
        [SerializeField] private float spinSpeed = default;
        [Header("PlayerCameraSetting")]
        [SerializeField] private Camera mainCamera = default;


       

        //private variable
        private float moveSpeed;
        private float currentVelocity;
        private float targetAngle;
        private float angle;

        private Rigidbody rb;
        private Animator anim;

        private Vector3 dir;
        private Vector3 lastDirection;



        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();

            moveSpeed = WalkSpeed;
        }
        

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal == 0f && vertical == 0f)
            {
                dir = Vector3.zero;
                
                moveSpeed = 0f;

                anim.SetFloat("MoveSpeed", moveSpeed);
                anim.SetBool("IsMove", false);

                if (lastDirection != Vector3.zero)
                {
                    PlayerRotate(lastDirection);
                }

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

            Vector3 forward = mainCamera.transform.forward;

            Vector3 right = mainCamera.transform.right;

            forward.y = 0f;
            forward.Normalize();

            //방향을 저장한다
            dir = (forward * vertical + right * horizontal).normalized;

            //마지막 방향을 저장한다
            lastDirection = dir;

            PlayerRotate(dir);

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

            float angle = Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetAngle, ref currentVelocity, spinSpeed);

            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }
}

