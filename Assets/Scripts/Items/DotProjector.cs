using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DotProjector : MonoBehaviour
{
    [SerializeField] LayerMask TargetLayer;
    Vector2 lightingDirection;
    float dotAngle;
    float dotRadius;
    bool isEquiped = true;
    GhostEventController ghostEvents;

    private void Awake()
    {
        dotAngle = GetComponent<Light2D>().pointLightOuterAngle;
        dotRadius = GetComponent<Light2D>().pointLightOuterRadius;
        ghostEvents = GameObject.FindFirstObjectByType<GhostEventController>()
            .GetComponent<GhostEventController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Use();
        }
        //장착중 && F키 누름
        if (isEquiped && GetComponent<Light2D>().enabled)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mousePositionToWorld = Camera.main.ScreenToWorldPoint(mousePosition);
            lightingDirection = mousePositionToWorld - transform.position;

            setLightingDirection(lightingDirection);
            if (isTargetInSight())//시야에 들어오면 eventCount 시작
            {
                ghostEvents.executeEvent(Ghost.GhostEvidences.dotprojector);
            }
        }
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

    public bool isTargetInSight()
    {
        bool isPlayerInSight = false;
        Vector2 originPosition = (Vector2)this.transform.position;
        //범위안의 targetLayer들 정보 반환
        Collider2D[] hitedTargets = Physics2D.OverlapCircleAll(originPosition, GetComponent<Light2D>().pointLightOuterRadius, TargetLayer);
        foreach (Collider2D hitedTarget in hitedTargets)
        {
            if (isTargetInLayer(hitedTarget, originPosition))
            {
                isPlayerInSight = true;
                break;
            }
        }
        return isPlayerInSight;
    }

    public bool isTargetInLayer(Collider2D hitedTarget, Vector2 originPosition)
    {
        bool isTargetInLayer = false;
        //내위치->Player로 가는 광선 방향각도 구하기
        Vector2 targetPos = hitedTarget.transform.position;
        Vector2 directionVector = this.transform.up;
        Vector2 toPlayerDirectionVector = targetPos - originPosition;
        //각도 내에 들어왔는지 체크
        float angle = Vector2.Angle(directionVector, toPlayerDirectionVector);
        if (angle <= this.dotAngle * 0.5f)
        {
            RaycastHit2D rayHitedTarget =
                Physics2D.Raycast(originPosition, toPlayerDirectionVector,
                                dotRadius, this.TargetLayer);
            if (rayHitedTarget)
            {
                isTargetInLayer = true;
                Debug.DrawLine(originPosition, targetPos, Color.yellow);
            }
        }
        return isTargetInLayer;
    }

}
