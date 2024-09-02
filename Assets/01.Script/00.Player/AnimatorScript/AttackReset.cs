using SOUL.Player;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AttackReset : StateMachineBehaviour
{
    [SerializeField] private string triggerName = default;
    [SerializeField] private PlayerController playerController = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.TryGetComponent(out playerController);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);

        playerController.isAction = false;
    }
}
