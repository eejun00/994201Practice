using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController1 : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public GameObject playCharacter;
    private Rigidbody rb;
    private Animator animator;
    public bool isJumping = false;
    private bool isPunching = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = playCharacter.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        // ������ ó��
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        animator.SetFloat("Horizen", horizontalInput);
        animator.SetFloat("Vertical", verticalInput);
        
        if(Input.GetKeyDown(KeyCode.Mouse0) && !isPunching &&!isJumping)
        {
            StartCoroutine(Attack());
        }

        // ���� ó��
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            animator.SetBool("Jump", true);
        }

        float mouseX = Input.GetAxis("Mouse X");
        if (Mathf.Abs(mouseX) > 0.1f)
        {
            // ȸ�� ������ ����Ͽ� ����
            Vector3 newRotation = transform.eulerAngles + new Vector3(0f, mouseX*3, 0f);
            transform.eulerAngles = newRotation;
        }
    }

    private IEnumerator Attack()
    {
        if (isPunching)
        {
            yield break; // �̹� ���� ���̸� ����
        }
        isPunching = true;
        animator.SetTrigger("OnePunch");
        yield return new WaitForSeconds(0.5f);

        animator.SetTrigger("AttackEnd");
        isPunching = false;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Ground")
    //    {
    //        isJumping = false;
    //        animator.SetBool("Jump", isJumping);
    //    }
    //}
}