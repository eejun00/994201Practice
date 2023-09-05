using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public Collider punchCollider;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody rb;
    private Animator animator;
    private bool isJumping = false;
    private bool isPunching = false;
    private bool isChanging = false;
    private float pushForce = 5f;
    public LayerMask tranformable;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        punchCollider.enabled = false;
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isPunching && !isJumping)
        {
            StartCoroutine(Attack());
        }

        //if (Input.GetKeyDown(KeyCode.Mouse1))
        //{
        //    Vector3 raycastStartPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(raycastStartPos, transform.forward, out hitInfo, 3f, tranformable))
        //    {
        //        foreach (Transform child in transform)
        //        {
        //            // �ڽ� ������Ʈ�� Parts �±׸� ������ �ִ��� Ȯ��
        //            if (child.CompareTag("Parts"))
        //            {
        //                // Parts �±׸� ���� �ڽ� ������Ʈ�� ��Ȱ��ȭ
        //                child.gameObject.SetActive(false);
        //            }
        //            else
        //            {
        //                Destroy(child.gameObject);
        //            }
        //        }
        //        GameObject hitObject = hitInfo.collider.gameObject;
        //        GameObject changingObj = Instantiate(hitObject);
        //        changingObj.transform.SetParent(transform);
        //        changingObj.transform.position = transform.position;
        //        isChanging = true;
        //        //rb.constraints = RigidbodyConstraints.None;
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Vector3 raycastStartPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            RaycastHit hitInfo;
            if (Physics.Raycast(raycastStartPos, transform.forward, out hitInfo, 3f, tranformable))
            {               
                string objectName = hitInfo.collider.gameObject.name; // 오브젝트 이름 가져오기
                photonView.RPC("ChangeObject", RpcTarget.All, objectName);
                //ChangeObject(objectName);
            }
        }       

        if (isChanging && Input.GetKeyDown(KeyCode.R))
        {
            photonView.RPC("ChangeBack", RpcTarget.All);
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
            Vector3 newRotation = transform.eulerAngles + new Vector3(0f, mouseX * 3, 0f);
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
        if(isChanging)
        {
            punchCollider.enabled = true;
        }
        yield return new WaitForSeconds(0.5f);
        punchCollider.enabled = false;
        animator.SetTrigger("AttackEnd");
        isPunching = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJumping = false;
            animator.SetBool("Jump", isJumping);
        }

        if (collision.collider == punchCollider && collision.gameObject.CompareTag("Punch"))
        {
            Debug.Log("닿았다");
            //Vector3 pushDirection = collision.transform.position - transform.position;
            //pushDirection = -pushDirection.normalized;
            //collision.gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * pushForce*10);
            Vector3 punchDirection = collision.contacts[0].normal;
            rb.AddForce(punchDirection * pushForce, ForceMode.Impulse);
        }
    }

    [PunRPC]
    void ChangeObject(string objectName)
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Parts"))
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                PhotonNetwork.Destroy(child.gameObject);
            }
        }
        if (photonView.IsMine)
        {
            // 리소스 폴더 내의 프리팹을 로드하여 생성
            GameObject newObject = PhotonNetwork.Instantiate(objectName, transform.position, Quaternion.identity);

            Vector3 currentRotation = newObject.transform.localRotation.eulerAngles;

            // X 축을 기준으로 -90도 회전합니다.
            currentRotation.x = -90f;

            // 새로운 로컬 회전 값을 설정합니다.
            newObject.transform.localRotation = Quaternion.Euler(currentRotation);
            newObject.transform.SetParent(transform);
            newObject.transform.position = transform.position;
        }
        isChanging = true;
        //rb.constraints = RigidbodyConstraints.None;
    }

    [PunRPC]
    private void ChangeBack()
    {
        foreach (Transform child in transform)
        {
            // �ڽ� ������Ʈ�� Parts �±׸� ������ �ִ��� Ȯ��
            if (child.CompareTag("Parts"))
            {
                // Parts �±׸� ���� �ڽ� ������Ʈ�� ��Ȱ��ȭ
                child.gameObject.SetActive(true);
            }
            else if(photonView.IsMine)
            {
                PhotonNetwork.Destroy(child.gameObject);
            }
        }
        isChanging = false;
    }
}