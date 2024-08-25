using SOUL.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKSystem : MonoBehaviour
{
    #region IK
    [Space(20f)]
    [Header("Feet Grounder")]
    public bool enableFeetIK = true;
    [Range(0f, 2f)] public float raycastUpDistance = default;
    [Range(0f, 2f)] public float raycastDownDistance = default;
    [Range(0f, 1f)][SerializeField] private float pelvisUpAndDownSpeed = default;
    [Range(0f, 1f)][SerializeField] private float feetToIkPositionSpeed = default;
    [SerializeField] private float pelvisOffset = 0f;
    [SerializeField] private bool useProIkFeature = false;
    [SerializeField] private bool showSolverDebug = true;
    [SerializeField] private LayerMask environmentLayer;

    private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;

    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    private readonly string leftFootAnimVariableName = "LeftFootCurve";
    private readonly string rightFootAnimVariableName = "RightFootCurve";

   [HideInInspector] public float lastDownRayDistance = default;
   [HideInInspector] public float lastUpRayDistance = default;

    private Animator anim = null;
    private NewPlayerController playerController = null;


    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<NewPlayerController>();
    }

    #region FootIK

    private void FixedUpdate()
    {
        if (enableFeetIK == false) { return; }
        if (anim == null) { return; }

        AdjustTarget(ref rightFootPosition, HumanBodyBones.RightFoot); //오른쪽발
        AdjustTarget(ref leftFootPosition, HumanBodyBones.LeftFoot); //왼쪽발

        FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableFeetIK == false) { return; }
        if (anim == null) { return; }

        MovePelvisHeight();

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);

        if (useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
        }
        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);

        if (useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));
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
        //레이가 닫지 않았을때 
        if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = anim.bodyPosition.y;
            return;
        }

        if (playerController.IsGround == false) { return; }
        if (playerController.IsAction) { return; }


        float leftOffsetPosition = leftFootIkPosition.y - transform.position.y; //현재 y값과 왼쪽발의 IK값의 사이의 길이를 구함
        float rightOffsetPosition = rightFootIkPosition.y - transform.position.y; //현재 y값과 오른쪽발의 IK값의 사이의 길이를 구함
                                                                                  //양수이면 y값 보다 높은것임 (경사에서 높은 발의 지점)
                                                                                  //음수이면 y값 보다 낮은것임 (경사에서 낮은 발의 지점)

        float totalOffset = (leftOffsetPosition < rightOffsetPosition) ? leftOffsetPosition : rightOffsetPosition;


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
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + raycastUpDistance), Color.yellow);
        }

        //지정해준 레이어에 닫는다면 실행
        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + raycastUpDistance, environmentLayer))
        {
            //받은 포지션을 레이에 닫은 y값으로 바꾼다
            feetIkPositions = fromSkyPosition;

            feetIkPositions.y = feetOutHit.point.y + pelvisOffset;

            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
            //현재의 값을 곱하면 로컬 좌표기준으로 바뀐다

            return;
        }

        //닫지 않으면 초기화
        feetIkPositions = Vector3.zero;
    }

    //받은 포지션을 뼈의 포지션으로 옮긴다
    private void AdjustTarget(ref Vector3 feetPosition, HumanBodyBones foot)
    {
        feetPosition = anim.GetBoneTransform(foot).position; //받은 포지션을 뼈의 포지션으로 옮긴다
        feetPosition.y = transform.position.y + raycastUpDistance; //뼈로 옮긴 포지션의 y값에 현제 y값에 레이케스트의 길이만큼 더한다 
    }
    #endregion
}
