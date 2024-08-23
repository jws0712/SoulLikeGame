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
    [SerializeField] private float gravityMultiplier = default;
    [SerializeField] private PlayerView playerView = null;
    [Space(20f)]
    [Header("Physics")]
    [SerializeField] private float groundedGravity = default;

    #region IK
    private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    [Space(20f)]
    [Header("Feet Grounder")]
    public bool enableFeetIK = true;
    [Range(0f, 2f)][SerializeField] private float heightFromGroundRaycast = 1.4f;
    [Range(0f, 2f)][SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [Range(0f, 1f)][SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0f, 1f)][SerializeField] private float feetToIkPositionSpeed = 0.5f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    public bool useProIkFeature = false;
    public bool showSolverDebug = true;
    #endregion

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



    //컴포넌트 초기화
    private void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        enableFeetIK = !isAction;
        inputMagnitude = playerView.Dir.magnitude;                                          //플레이어의 방향값의 크기를 구함
        moveAmount = Mathf.Abs(playerInput.Horizontal) + Mathf.Abs(playerInput.Vertical);   // 플레이어가 이동한 거리의 절대값을 계산함

        PlayerAction();
        SetPlayerAnimationState();
        AirMove();
    }

    #region PlayerFunc
    /// <summary>
    /// 애니매이션을 통해 캐릭터를 이동시킴
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
    /// 플레이어가 공중에 있을때 움직임
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
        anim.SetBool("isAction", isAction);

        if (!isGround)
        {
            anim.SetFloat("VelY", gravity);
        }
        
    }

    /// <summary>
    /// 플레이어를 목표 각도까지 회전하게 한다
    /// </summary>
    /// <param name="targetRot"></param>
    private void PlayerRotate(Quaternion targetRot)
    {
        if (!isAction)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, spinSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 플레이어 점프
    /// </summary>
    private void Jump()
    {


        if (cc.isGrounded)
        {
            anim.SetBool("isGround", true);
            isGround = true;

            anim.SetBool("isJump", false);
            isJump = false;

            anim.SetBool("isFall", false);


            gravity = -groundedGravity;


            if (!isAction && Input.GetKeyDown(KeyCode.F))
            {
                isGround = false;
                gravity = jumpPower;
                anim.SetBool("isJump", true);
                isJump = true;
            }
        }
        else
        {
            if (isGround)
            {
                gravity = 0;
                isGround = false;
                anim.SetBool("isFall", true);
            }

            anim.SetBool("isGround", false);
            gravity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;


            

            if((isJump && gravity < 0) || gravity < -groundedGravity)
            {
                anim.SetBool("isFall", true);
            }

        }

        Debug.Log(cc.isGrounded);
    }

    /// <summary>
    /// 플레이어 구르기
    /// </summary>
    private void Roll()
    {
        anim.SetTrigger("roll");
    }

    /// <summary>
    /// 플레이어의 행동을 관리한다
    /// </summary>
    private void PlayerAction()
    {
        Jump();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPress = true;
        }

        if (isPress && Input.GetKey(KeyCode.Space))
        {
            pressTime += Time.deltaTime;

            if (pressTime > 0.5f)
            {
                inputMagnitude *= 2;
            }
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


    public void StartAction()
    {
        isAction = true;
    }

    public void EndAction()
    {
        isAction = false;
    }

    #endregion

    #region FootIK

    private void FixedUpdate()
    {
        if (!isGround) { return; }
        if (enableFeetIK == false) { return; }
        if(anim == null) { return; }

        AdjustTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!isGround) { return; }
        if (enableFeetIK == false) { return; }
        if (anim == null) { return; }

        MovePelvisHeight();

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);

        if(useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
            Debug.Log("Left_Pro");
        }
        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);


        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);

        if (useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));
            Debug.Log("Left_Pro");
        }
        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
    }

    void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
    {
        Vector3 targetIkPosition = anim.GetIKPosition(foot);

        if (positionIkHolder != Vector3.zero)
        {
            targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIkPositionSpeed);
            targetIkPosition.y += yVariable;

            lastFootPositionY = yVariable;

            targetIkPosition = transform.TransformPoint(targetIkPosition);

            anim.SetIKRotation(foot, rotationIkHolder);
        }

        anim.SetIKPosition(foot, targetIkPosition);
    }

    private void MovePelvisHeight()
    {
        if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = anim.bodyPosition.y;
            return;
        }

        float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
        float rOffsetPosition = rightFootIkPosition.y - transform.position.y;

        float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

        Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

        anim.bodyPosition = newPelvisPosition;
        lastPelvisPositionY = anim.bodyPosition.y;
    }

    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
    {
        RaycastHit feetOutHit;

        if (showSolverDebug)
        {
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);
        }

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, environmentLayer))
        {
            feetIkPositions = fromSkyPosition;
            feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }

        feetIkPositions = Vector3.zero;
    }

    private void AdjustTarget(ref Vector3 feelPositions, HumanBodyBones foot)
    {
        feelPositions = anim.GetBoneTransform(foot).position;
        feelPositions.y = transform.position.y + heightFromGroundRaycast;
    }


    #endregion

}
