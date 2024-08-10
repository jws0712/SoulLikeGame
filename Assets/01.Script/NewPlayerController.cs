using SOUL.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewPlayerController : MonoBehaviour
{
    [Header("PlayerMovementSetting")]
    [SerializeField] private float airMoveSpeed = default;
    [SerializeField] private float spinSpeed = default;
    [SerializeField] private float jumpPower = default;
    [SerializeField] private PlayerView playerView = null;
    [SerializeField] private float gravityMultiplier = default;


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
    private bool isGround = default;



    //������Ʈ �ʱ�ȭ
    private void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        inputMagnitude = playerView.Dir.magnitude;                                          //�÷��̾��� ���Ⱚ�� ũ�⸦ ����
        moveAmount = Mathf.Abs(playerInput.Horizontal) + Mathf.Abs(playerInput.Vertical);   // �÷��̾ �̵��� �Ÿ��� ���밪�� �����

        PlayerAction();
        SetPlayerAnimationState();
        AirMove();
    }

    #region PlayerFunc
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
        if(!isGround)
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
                PlayerRotate(targetRot);

            }
            anim.SetBool("isMove", true);
        }
        else
        {
            PlayerRotate(lastTargetRot);
            anim.SetBool("isMove", false);
        }

        anim.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
    }

    /// <summary>
    /// �÷��̾ ��ǥ �������� ȸ���ϰ� �Ѵ�
    /// </summary>
    /// <param name="targetRot"></param>
    private void PlayerRotate(Quaternion targetRot)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, spinSpeed * Time.deltaTime);
    }

    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    private void Jump()
    {
        if (cc.isGrounded)
        {
            gravity = -2f;

            anim.SetBool("isGround", true);
            isGround = true;
            anim.SetBool("isJump", false);
            isJump = false;
            anim.SetBool("isFall", false);

            if (Input.GetKeyDown(KeyCode.F))
            {
                gravity = jumpPower;
                anim.SetBool("isJump", true);
                isJump = true;
            }
        }
        else
        {
            gravity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            anim.SetBool("isGround", false);
            isGround = false;
            if((isJump && gravity < 0) || gravity < -2f)
            {
                anim.SetBool("isFall", true);
            }

        }
    }

    /// <summary>
    /// �÷��̾� ������
    /// </summary>
    private void Roll()
    {
        anim.SetTrigger("roll");
    }

    /// <summary>
    /// �÷��̾��� �ൿ�� �����Ѵ�
    /// </summary>
    private void PlayerAction()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPress = true;
        }

        if (isPress && Input.GetKey(KeyCode.Space))
        {
            pressTime += Time.deltaTime;

            if (pressTime > 0.5f)
            {
                inputMagnitude /= 2;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isPress = false;

            if (pressTime < 0.5f)
            {
                Roll();
            }

            pressTime = 0;

        }

        Jump();
    }
    #endregion

}
