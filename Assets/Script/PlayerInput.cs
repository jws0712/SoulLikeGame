namespace SOUL.Player
{

    //System
    using System.Collections;
    using System.Collections.Generic;

    //UnityEngine
    using UnityEngine;

    public class PlayerInput : MonoBehaviour
    {
        private float horizontal = default;
        private float vertical = default;

        //프로퍼티
        public float Horizontal => horizontal;
        public float Vertical => vertical;

        private void Update()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
    }
}

