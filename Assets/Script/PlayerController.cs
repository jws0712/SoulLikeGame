namespace SOUL.Player
{
    //System
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    //UnityEngine
    using UnityEngine;

    //Project
    using SOUL.Camera;
    using Unity.VisualScripting;

    public class PlayerController : MonoBehaviour
    {
        [Header("PlayerMovementSetting")]
        [SerializeField] private float WalkSpeed = default;
        [SerializeField] private float sprintSpeed = default;
        [SerializeField] private float spinSpeed = default;
        [SerializeField] private PlayerInput playerInput = null;
        [SerializeField] private PlayerView playerView = null;
        [SerializeField] private Animator anim = null;

        //private variable
        private float moveSpeed;
        private bool isSens;

        private Rigidbody rb;
        


        private Vector3 lastDirection;
        private Vector3 forward;
        private Vector3 right;



        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            moveSpeed = WalkSpeed;
        }
        

        private void Update()
        {
            anim.SetFloat("Horizontal", playerInput.Horizontal);
            anim.SetFloat("MoveSpeed", moveSpeed);
            anim.SetBool("isSens", playerView.IsSens);

            if (playerInput.Horizontal == 0f && playerInput.Vertical == 0f)
            {
                moveSpeed = 0f;

                PlayerRot();

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

            PlayerRot();            
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * playerView.Dir);
        }

        private void PlayerRot()
        {
            float yRotation = playerView.transform.rotation.eulerAngles.y; //플레이어뷰의 오일러의 y값을 가지고 옴

            Vector3 currentEuler = transform.rotation.eulerAngles; //현재 회전값을 가지고옴

            Quaternion targetRotation = Quaternion.Euler(currentEuler.x, yRotation, currentEuler.z);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * spinSpeed);
        }


    }
}

