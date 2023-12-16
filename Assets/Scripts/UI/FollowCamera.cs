using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    Transform cameraTransform;
    Vector3 offset;//Start ��, Camera Object�� player Object ������ ���� ������ ����.

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        offset = playerTransform.position - cameraTransform.position;
    }

    private void LateUpdate()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position,
                                            playerTransform.position - offset,
                                            Time.deltaTime);
    }

}
