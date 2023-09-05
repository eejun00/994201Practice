using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubScript : MonoBehaviour
{
    private PlayerController1 playerController1;
    private Animator animator;
    private void Start()
    {
        playerController1 = GetComponentInParent<PlayerController1>();
        animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            playerController1.isJumping = false;
            animator.SetBool("Jump", playerController1.isJumping);
        }
    }
}
