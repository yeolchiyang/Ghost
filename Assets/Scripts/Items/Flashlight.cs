using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlight : MonoBehaviour, IUsable
{

    private Vector2 lightingDirection;

    private void Update()
    {
        // 1. 현재 마우스 픽셀 위치 가져오기
        Vector3 mousePosition = Input.mousePosition;
        // 2. ScreenToWorldPoint 이용하여 월드 좌표로 변환
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
        // 손전등을 킨다거나 끈다 등의 동작 수행
        GetComponent<Light2D>().enabled = !GetComponent<Light2D>().enabled;
        
    }

    public void setLightingDirection(Vector2 lightingDirection)
    {
        Vector2 normalDirection = Vector2.up;
        Quaternion rotationToTarget = Quaternion.FromToRotation(
            normalDirection, lightingDirection);
        // 회전 각도 구하기
        float angle = rotationToTarget.eulerAngles.z;
        //Debug.Log(angle);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
