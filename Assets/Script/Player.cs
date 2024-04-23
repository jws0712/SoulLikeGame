using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Player : MonoBehaviour
{
    [SerializeField] float WalkSpeed;
    [SerializeField] float sprintSpeed;
    private float moveSpeed;
    private Rigidbody rb;
    private Animator anim;
    private Vector3 dir;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        moveSpeed = WalkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = sprintSpeed;
            Debug.Log("¶Ü");
        }
        else
        {
            moveSpeed = WalkSpeed;
            Debug.Log("°É¾î");
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        dir = (Vector3.forward * vertical + Vector3.right * horizontal).normalized;

        transform.LookAt(transform.position + dir);

        if(dir != Vector3.zero)
        {
            anim.SetBool("IsMove", true);
        }
        else
        {
            anim.SetBool("IsMove", false);
        }

        anim.SetFloat("MoveSpeed", moveSpeed);
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }
}