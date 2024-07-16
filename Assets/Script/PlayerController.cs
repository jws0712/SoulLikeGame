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

            if (playerInput.Horizontal == 0f && playerInput.Vertical == 0f)
            {
                playerView.dir = Vector3.zero;
                
                moveSpeed = 0f;


                anim.SetBool("IsMove", false);
                anim.SetFloat("MoveSpeed", moveSpeed);

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

            anim.SetBool("IsMove", true);
            anim.SetFloat("MoveSpeed", moveSpeed);
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * playerView.dir);
        }

        private void PlayerRot()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, playerView.transform.rotation, Time.deltaTime * spinSpeed);
        }


    }
}

