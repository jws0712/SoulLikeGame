namespace SOUL.Player
{
    //System
    using System;
    using System.Collections;
    using System.Collections.Generic;

    //Unity
    using UnityEngine;
    using Unity.VisualScripting;
    using UnityEngine.EventSystems;

    public class NewPlayerController : MonoBehaviour
    {
        [Header("PlayerMovementSetting")]
        [SerializeField] private float airMoveSpeed = default;
        [SerializeField] private float spinSpeed = default;
        [SerializeField] private float rollActionSpinSpeed = default;
        [SerializeField] private float jumpPower = default;
        [SerializeField] private float gravityMultiplier = default;
        [SerializeField] private PlayerView playerView = null;
        [Space(20f)]
        [Header("Physics")]
        [SerializeField] private float groundedGravity = default;

        private float gravity = default;
        private float moveAmount = default;
        private float inputMagnitude = default;
        private float pressTime = default;

        private PlayerInput playerInput = null;
        private Animator anim;
        private CharacterController cc;

        private Quaternion lastTargetRot = Quaternion.identity;

        private bool isPress = default;
        private bool isJump = default;
        private bool isAction = default;
        private bool isGround = default;

        private PlayerIKSystem playerIK = null;

        public bool IsGround => isGround;
        public bool IsAction => isAction;



        //������Ʈ �ʱ�ȭ
        private void Awake()
        {
            anim = GetComponent<Animator>();
            cc = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            playerIK = GetComponent<PlayerIKSystem>();
        }

        private void Start()
        {
            playerIK.lastDownRayDistance = playerIK.raycastDownDistance;
            playerIK.lastUpRayDistance = playerIK.raycastUpDistance;
        }

        private void Update()
        {
            if (isAction)
            {
                playerIK.raycastDownDistance = 0f;
                playerIK.raycastUpDistance = 0f;
            }
            else
            {
                playerIK.raycastDownDistance = playerIK.lastDownRayDistance;
                playerIK.raycastUpDistance = playerIK.lastUpRayDistance;
            }

            inputMagnitude = playerView.Dir.magnitude;                                          //�÷��̾��� ���Ⱚ�� ũ�⸦ ����
            moveAmount = Mathf.Abs(playerInput.Horizontal) + Mathf.Abs(playerInput.Vertical);   // �÷��̾ �̵��� �Ÿ��� ���밪�� �����

            PlayerAction();
            SetPlayerAnimationState();
            AirMove();
        }

        #region PlayerFunc

        #region PlayerMovement

        /// <summary>
        /// �ִϸ��̼��� ���� ĳ���͸� �̵���Ŵ
        /// </summary>
        private void OnAnimatorMove()
        {
            if (isGround)
            {
                Vector3 vel = anim.deltaPosition;
                vel.y = gravity * Time.deltaTime;
                cc.Move(vel);
            }

        }

        /// <summary>
        /// �÷��̾ ���߿� ������ ������
        /// </summary>
        private void AirMove()
        {
            if (!isGround)
            {
                Vector3 vel = playerView.Dir * inputMagnitude * airMoveSpeed;
                vel.y = gravity;

                cc.Move(vel * Time.deltaTime);
            }
        }

        /// <summary>
        /// �÷��̾��� �ִϸ��̼� ���¸� �����Ѵ�
        /// </summary>
        private void SetPlayerAnimationState()
        {
            if (moveAmount > 0f)
            {
                if (playerView.Dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(playerView.Dir);
                    lastTargetRot = targetRot;
                    PlayerRotate(targetRot, spinSpeed);

                }
                anim.SetBool("isMove", true);
            }
            else
            {
                PlayerRotate(lastTargetRot, spinSpeed);
                anim.SetBool("isMove", false);
            }

            anim.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
            anim.SetBool("isAction", isAction);

            if (!isGround)
            {
                anim.SetFloat("VelY", gravity);
            }

        }

        /// <summary>
        /// �÷��̾ ��ǥ �������� ȸ���ϰ� �Ѵ�
        /// </summary>
        /// <param name="targetRot"></param>
        private void PlayerRotate(Quaternion targetRot, float spinSpeed)
        {
            if (!isAction)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, spinSpeed * Time.deltaTime);
            }
        }

        #endregion

        #region Physics

        /// <summary>
        /// ������ �����ϴ� �Լ�
        /// </summary>
        private void SetPhysics()
        {
            if (cc.isGrounded)
            {
                Grounded();
                Jump();
            }
            else
            {
                Falling();
            }
        }


        /// <summary>
        /// ���϶��� ��츦 �����ϴ� �Լ�
        /// </summary>
        private void Grounded()
        {
            playerIK.raycastDownDistance = playerIK.lastDownRayDistance;
            gravity = -groundedGravity;

            anim.SetBool("isGround", true);
            isGround = true;

            anim.SetBool("isJump", false);
            isJump = false;

            anim.SetBool("isFall", false);
        }

        /// <summary>
        /// ���� �ƴҶ��� �����ϴ� �Լ�
        /// </summary>
        private void Falling()
        {
            if (isGround)
            {
                gravity = 0;
                isGround = false;
                anim.SetBool("isFall", true);
            }

            if ((isJump && gravity < 0) || gravity < -groundedGravity)
            {
                anim.SetBool("isFall", true);
            }

            anim.SetBool("isGround", false);
            gravity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            playerIK.raycastDownDistance = 0f;
        }

        #endregion

        #region PlayerAction
        /// <summary>
        /// �÷��̾��� �ൿ�� �����Ѵ�
        /// </summary>
        private void PlayerAction()
        {
            SetPhysics();
            RollAndRunAction();
        }

        /// <summary>
        /// �޸���� �����⸦ �����ϴ� �Լ�
        /// </summary>
        public void RollAndRunAction()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPress = true;
            }

            if (isPress && Input.GetKey(KeyCode.Space))
            {
                pressTime += Time.deltaTime;

                Run();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isPress = false;

                if (!isAction && cc.isGrounded && pressTime < 0.5f)
                {
                    Roll();
                }

                pressTime = 0;

            }
        }

        /// <summary>
        /// �÷��̾� ������
        /// </summary>
        private void Roll()
        {
            PlayerRotate(lastTargetRot, rollActionSpinSpeed);
            anim.SetTrigger("roll");
        }

        /// <summary>
        /// �÷��̾� �޸���
        /// </summary>
        private void Run()
        {
            if (pressTime > 0.5f)
            {
                inputMagnitude *= 2;
            }
        }

        /// <summary>
        /// �÷��̾� ����
        /// </summary>
        private void Jump()
        {
            if (!isAction && Input.GetKeyDown(KeyCode.F))
            {
                isGround = false;
                gravity = jumpPower;
                anim.SetBool("isJump", true);
                isJump = true;
            }
        }

        #endregion

        #region CheckAction
        /// <summary>
        /// �ִϸ��̼��� ���������� üũ�ϴ� �Լ�
        /// </summary>
        public void StartAction()
        {
            isAction = true;
        }

        public void EndAction()
        {
            isAction = false;
        }
        #endregion

        #endregion


    }
}