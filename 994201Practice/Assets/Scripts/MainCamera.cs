using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;  // �÷��̾� ĳ������ Transform
    public float distance = 6.0f;  // ī�޶�� ĳ���� ������ �Ÿ�
    public float height = 1.0f;  // ī�޶��� ����
    public float smoothSpeed = 30.0f;  // ī�޶� �̵� ������ ��wa

    private Vector3 offset;  // �ʱ� ī�޶�� ĳ������ ������

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        offset = new Vector3(0f, height, -distance);
    }

    private void LateUpdate()
    {
        // ī�޶� ��ġ ������Ʈ
        Vector3 targetPosition = target.position + target.TransformDirection(offset);
        targetPosition = new Vector3(targetPosition.x, targetPosition.y + height, targetPosition.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // ī�޶� ĳ���͸� �ٶ󺸵��� ȸ��
        transform.LookAt(target);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            transform.position = targetPosition;
            transform.LookAt(target);
        }
    }
}
