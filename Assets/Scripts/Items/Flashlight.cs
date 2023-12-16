using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlight : MonoBehaviour, IUsable
{

    private Vector2 lightingDirection;

    private void Update()
    {
        // 1. ���� ���콺 �ȼ� ��ġ ��������
        Vector3 mousePosition = Input.mousePosition;
        // 2. ScreenToWorldPoint �̿��Ͽ� ���� ��ǥ�� ��ȯ
        Vector3 mousePositionToWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        lightingDirection = mousePositionToWorld - transform.position;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Use();
        }
        setLightingDirection(lightingDirection);
    }

    public void Use()
    {
        // �������� Ų�ٰų� ���� ���� ���� ����
        GetComponent<Light2D>().enabled = !GetComponent<Light2D>().enabled;
        
    }

    public void setLightingDirection(Vector2 lightingDirection)
    {
        Vector2 normalDirection = Vector2.up;
        Quaternion rotationToTarget = Quaternion.FromToRotation(
            normalDirection, lightingDirection);
        // ȸ�� ���� ���ϱ�
        float angle = rotationToTarget.eulerAngles.z;
        //Debug.Log(angle);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
