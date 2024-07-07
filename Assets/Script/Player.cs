namespace SOUL.Player
{

    //System
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;

    //UnityEngine
    using UnityEngine;
    using UnityEngine.Assertions.Must;
    
    public class Player : MonoBehaviour
    {
        [SerializeField] float WalkSpeed;
        [SerializeField] float sprintSpeed;
        [SerializeField] float smoothTime = 0.05f;
       

        //private variable
        private float moveSpeed;
        private Rigidbody rb;
        private Animator anim;
        private Vector3 dir;
        private float currentVelocity;


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
                anim.SetBool("IsMove", false);
                moveSpeed = 0f;
                anim.SetFloat("MoveSpeed", moveSpeed);
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

            dir = (Vector3.forward * vertical + Vector3.right * horizontal).normalized;

            var targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            anim.SetBool("IsMove", true);
            anim.SetFloat("MoveSpeed", moveSpeed);
        }


        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
        }
    }
}

