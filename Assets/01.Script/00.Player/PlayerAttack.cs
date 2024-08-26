namespace SOUL.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerAttack : MonoBehaviour
    {
        Animator animator;
        int hashAttackCount = Animator.StringToHash("AttackCount");

        private void Start()
        {
            TryGetComponent(out  animator);
        }

        //프로퍼티
        public int AttackCount { get => animator.GetInteger(hashAttackCount); set => animator.SetInteger(hashAttackCount, value); }
    }
}
