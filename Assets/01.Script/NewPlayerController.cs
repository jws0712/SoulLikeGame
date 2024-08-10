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
    private float pressTime = default;

    private PlayerInput playerInput = null;
    private Animator anim;
    private CharacterController cc;

    private Quaternion lastTargetRot = Quaternion.identity;

    private bool isRun = default;
    private bool isRoll = default;
    private bool isJump = default;
    private bool isPress = default;



    //컴포넌트 초기화
    private void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        inputMagnitude = playerView.Dir.magnitude;                                          //플레이어의 방향값의 크기를 구함
        moveAmount = Mathf.Abs(playerInput.Horizontal) + Mathf.Abs(playerInput.Vertical);   // 플레이어가 이동한 거리의 절대값을 계산함

        if(Input.GetKeyDown(KeyCode.Space))
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
                anim.SetTrigger("roll");
            }

            pressTime = 0;

        }

        MakeGravity();
        SetPlayerAnimationState();
    }

    #region PlayerFunc
    /// <summary>
    /// 애니매이션을 통해 캐릭터를 이동시킴
    /// </summary>
    private void OnAnimatorMove()
    {
        Vector3 vel = anim.deltaPosition;
        vel.y = gravity * Time.deltaTime;
        cc.Move(vel);
    }

    /// <summary>
    /// 플레이어의 애니매이션 상태를 설정한다
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
    /// 플레이어를 목표 각도까지 회전하게 한다
    /// </summary>
    /// <param name="targetRot"></param>
    private void PlayerRotate(Quaternion targetRot)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, spinSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 중력을 만든다
    /// </summary>
    private void MakeGravity()
    {
        if (cc.isGrounded)
        {
            gravity = -2f;
        }
        else
        {
            gravity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }
    #endregion

}
