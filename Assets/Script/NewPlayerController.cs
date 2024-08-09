using SOUL.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewPlayerController : MonoBehaviour
{
    [Header("PlayerMovementSetting")]
    [SerializeField] private float spinSpeed = default;
    [SerializeField] private PlayerView playerView = null;
    [SerializeField] private float gravityMultiplier = default;


    private float gravity = default;
    private float moveAmount = default;
    private float inputMagnitude = default;

    private PlayerInput playerInput = null;
    private Animator anim;
    private CharacterController cc;

    private Quaternion lastTargetRot = Quaternion.identity;

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

        PlayerRoll();

        if(cc.isGrounded)
        {
            gravity = -2f;
        }
        else
        {
            gravity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        Debug.Log(gravity);

        SetPlayerAnimationState();
    }

    #region PlayerFunc
    /// <summary>
    /// �ִϸ��̼��� ���� ĳ���͸� �̵���Ŵ
    /// </summary>
    private void OnAnimatorMove()
    {
        Vector3 vel = anim.deltaPosition;
        vel.y = gravity * Time.deltaTime;
        cc.Move(vel);
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

        if (Input.GetKey(KeyCode.Space))
        {
            inputMagnitude /= 2;
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
    /// �÷��̾� �����⸦ ������ �Լ�
    /// </summary>
    private void PlayerRoll()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            anim.SetTrigger("roll");
        }
    }
    #endregion

}
