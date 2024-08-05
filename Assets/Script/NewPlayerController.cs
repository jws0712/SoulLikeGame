using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    private Animator anim;
    private CharacterController cc;

    Vector2 input;
    Vector3 rootMotion;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
    }
}
