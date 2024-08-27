using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlyerAttackReset : StateMachineBehaviour
{
    [SerializeField] private string triggerName = default;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
    }
}
