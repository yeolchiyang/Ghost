using System.Collections;
using UnityEditor;
using UnityEngine;

public class FoldedNote : MonoBehaviour
{
    [SerializeField] LayerMask TargetLayer;
    [SerializeField] float foldedNoteRadius = 2f;
    [SerializeField] bool isDebug = false;
    [SerializeField] Sprite eventSprite;//고스트 라이트 이벤트시 전환 할 sprite
    SpriteRenderer eventRenderer;//Sprite 변경을 위해 필요한 클래스
    GhostEventController ghostEvents;
    [SerializeField] GameObject[] linePrefab;
    bool isEvented = false;

    public bool IsEvented
    {
        get { return isEvented; }
        set { isEvented = value; }
    }

    private void Awake()
    {
        eventRenderer = GetComponent<SpriteRenderer>();
        ghostEvents = GameObject.FindFirstObjectByType<GhostEventController>()
            .GetComponent<GhostEventController>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (isTargetInRange())
        {
            ghostEvents.executeEvent(Ghost.GhostEvidences.GhostWriting);
        }
    }

    //고스트 라이팅 이벤트 발생 시 실행될 함수
    //한번이라도 실행되면, 원상태로 돌아오지 않음
    float isEventedTimer = 0f;
    public IEnumerator changeIsEvent()
    {
        changeSprite();
        while (!isEvented)
        {
            openedNote();
            isEventedTimer += Time.deltaTime;
            if(isEventedTimer >= 1f)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                isEvented = true;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        
    }

    public void changeSprite()
    {
        if (eventSprite != null && eventRenderer != null)
        {
            // Sprite 변경
            eventRenderer.sprite = eventSprite;
        }
    }


    float eventTimer = 0f;
    bool isRightRotate = true;
    int linePrefabIndex = 0;
    public void openedNote()
    {
        eventTimer += Time.deltaTime;
        if(eventTimer > 0.1f)
        {
            isRightRotate = shake(isRightRotate);
            if(linePrefabIndex < linePrefab.Length)
            {
                GameObject line = Instantiate(linePrefab[linePrefabIndex], transform);
                linePrefabIndex++;
            }
        }
        Debug.Log("openedNote");
    }
    bool shake(bool shake)
    {
        if (shake)
        {
            transform.rotation = Quaternion.Euler(0, 0, -15);
            shake = !shake;
            eventTimer = 0f;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 15);
            shake = !shake;
            eventTimer = 0f;
        }
        return shake;
    }

    /* 타겟감지 시-> 
     * Update문에서 GhostEventController의 이벤트 발생 함수 지속호출 중
     * GhostEventController
     */

    public bool isTargetInRange()
    {
        bool isTargetInRange = false;
        Vector2 originPosition = (Vector2)this.transform.position;
        //범위안의 targetLayer들 정보 반환
        Collider2D[] hitedTargets = Physics2D.OverlapCircleAll(originPosition, foldedNoteRadius, TargetLayer);
        foreach (Collider2D hitedTarget in hitedTargets)
        {
            if(isTargetInLayer(hitedTarget, originPosition))
            {
                isTargetInRange = true;
            }            
        }
        return isTargetInRange;
    }
    public bool isTargetInLayer(Collider2D hitedTarget, Vector2 originPosition)
    {
        bool isTargetInLayer = false;
        //내위치->Player로 가는 광선 방향각도 구하기
        Vector2 targetPos = hitedTarget.transform.position;
        Vector2 directionVector = this.transform.up;
        Vector2 toPlayerDirectionVector = targetPos - originPosition;
        //각도 내에 들어왔는지 체크
        RaycastHit2D rayHitedTarget =
            Physics2D.Raycast(originPosition, toPlayerDirectionVector,
                            foldedNoteRadius, this.TargetLayer);
        if (rayHitedTarget)
        {
            isTargetInLayer = true;
        }
        return isTargetInLayer;
    }



    //void OnDrawGizmos()
    //{
    //    if (isDebug)
    //    {
    //        DrawFieldOfView();
    //    }
        
    //}

    //void DrawFieldOfView()
    //{
    //    /*
    //     * DrawWireArc 함수를 사용하여 호 그리기
    //     * Handles.DrawWireArc(center, normal, from, angle, radius)
    //     * center : 호를 포함한 원의 중앙좌표(원의 중앙은 내 위치 transform.position 을 뜻합니다)
    //     * normal : 호를 그릴 방향을 담은 rotation 값입니다. 벡터 값을 넣는데, new Vector3(0, 0, 1)라면 Z축을 양수각도로 회전시킬 방향으로 그리겠다는 뜻입니다.
    //             * new Vector3(0, 0, 1)이든 new Vector3(0, 0, 180)이든 값의 크기와 상관없습니다. 그냥 호를 그릴 방향을 지정합니다.
    //     * from : 호를 그리기 시작할 좌표의 방향(실제 찍히는 위치는 radius에 의해 결정됨, 호의 첫번째 점이 찍힐 방향만 지정해주는 것)
    //     * angle : angle만큼 + 한 rotation까지 점을 찍어줌
    //     * (호를 그리는 방향은 normal에 의해 결정됩니다.angle은 그냥 normal의 방향대로 몇 도까지 점을 찍어줄 지 결정, angle이 마이너스면 반대방향이 됩니다)
    //     */
    //    Handles.color = Color.red;
    //    Handles.DrawWireArc(transform.position, new Vector3(0, 0, 1), new Vector3(0, 1, 0), 360f, foldedNoteRadius);
    //    //위와 동일
    //    //Handles.DrawWireArc(transform.position, Vector3.forward, leftRotatedDirection, -sightAngle, sightRadius);
    //}

}
