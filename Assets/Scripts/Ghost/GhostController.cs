using RSG;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    //디버깅용 변수
    [SerializeField] bool debugMode = false;
    GameObject[] interactiveObjects = new GameObject[5];//귀신 시야넣을 임시변수
    GameObject ghostSight;//디버깅용 불빛시야
    private IState rootState;//상태
    Ghost ghost;//귀신 스탯 보유한 클래스
    private bool isColliding = false;
    private Player player;

    private void Awake()
    {
        //=============시야관련 코드, 필요없으면 지울 것
        if (debugMode)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                interactiveObjects[i] = transform.GetChild(i).gameObject;
                if (interactiveObjects[i].CompareTag("Sight"))
                {
                    ghostSight = interactiveObjects[i];
                }
            }
        }
        //=============시야관련 코드, 필요없으면 지울 것
    }

    void Start()
    {
        ghost = GetComponent<Ghost>();
        ghost.CurrentHuntingGraceTime = ghost.MaxHuntingGraceTime;//헌팅 유예시간 초기화
        ghost.setGhostEvents(ghost.EvidenceAmount);//증거 초기설정
        GameManager.gameManager.VideoHandler.SetActive(false);//gameOver 비활성화
        player = GameManager.gameManager.Player;
        //================상태정의
        rootState = new StateMachineBuilder()
            .State<HuntingState>("Peaceful")//
                .Enter(state =>
                {
                    Debug.Log("Entering Peaceful state");
                    RemoveToCullingMask();//카메라에 비치지 않도록
                    Vector2 randomPosition = new Vector3(2.79f, -4.27f, 0f) +
                        Random.insideUnitSphere * ghost.MoveRadius;
                    ghost.TargetPosition = randomPosition;
                    ghost.CatchingDistance = 0.1f;
                })
                .Update((state, deltaTime) =>
                {
                    MoveToDiretion(ghost.TargetPosition - transform.position,
                        deltaTime);
                })
                .Condition(() =>
                {
                    //부딪히거나, 일정거리 접근 시 true
                    return isCollidedOrAccess();
                },
                state =>
                {
                    state.Enter();
                })
                .Condition(() =>
                {
                    //상태전환 조건 추가
                    
                    return isHuntingTime();
                },
                state =>
                {
                    state.Parent.ChangeState("Hunting");
                })
                .Event("Targeting", state =>
                {
                    //state.PushState("Retreat");
                })
                .Exit(state =>
                {
                    AddToCullingMask();
                    Debug.Log("Exiting Peaceful state");
                })
                .End()
            .State<HuntingState>("Hunting")//======Hunting 이곳부터
                .Enter(state =>
                {
                    Debug.Log("Entering Hunting state");
                    Vector2 randomPosition = new Vector3(2.79f, -4.27f, 0f) +
                        Random.insideUnitSphere * ghost.MoveRadius;
                    ghost.TargetPosition = randomPosition;
                    ghost.CatchingDistance = 0.1f;
                    ghost.VisibleStateCount = 0f;
                    ghost.CurrentHuntingDuration = ghost.MaxHuntingDuration;
                })
                .Update((state, deltaTime) =>
                {
                    ChangeTransparency(deltaTime);
                    MoveToDiretion(ghost.TargetPosition - transform.position,
                        deltaTime);
                })
                .Condition(() =>
                {
                    return isPlayerInSight();
                },
                state =>
                {
                    state.ChangeState("Chasing");
                })
                .Condition(() =>
                {
                    //부딪히거나, 일정거리 접근 시 true
                    return isCollidedOrAccess();
                },
                state =>
                {
                    state.Enter();
                })
                .Exit(state =>
                {
                    Debug.Log("Exiting Hunting state");
                })
                .State<HuntingState>("Chasing")
                    .Enter(state =>
                    {
                        Debug.Log("Entering Chasing state");
                        ghost.CatchingDistance = 1f;
                    })
                    .Update((state, deltaTime) =>
                    {
                        ChangeTransparency(deltaTime);
                        MoveToDiretion(ghost.TargetPosition - transform.position,
                                deltaTime);
                    })
                    .Condition(() =>
                    {
                        return isCollidedOrAccess();
                    },
                    state =>
                    {
                        //일정거리 접근 시 Player 사망하는 것 구현
                        //현재 여기서 다른상태 전환 못하고 있음 나중에 고치기
                        GameManager.gameManager.VideoHandler.SetActive(true);
                        StartCoroutine(GameManager.gameManager.GetComponentInChildren<VideoHandler>().PrepareVideo());
                        state.Parent.ChangeState("Peaceful");
                    })
                    .Condition(() =>
                    {
                        bool isChasingOver = !isPlayerInSight() &&
                        !isCollidedOrAccess() && isHuntingTimeOver();
                        return isChasingOver;
                    },
                    state =>
                    {
                        state.Parent.PopState();
                    })
                    .Exit(state =>
                    {
                        Debug.Log("Exiting Chasing state");
                    })
                    .End()//StateBuilder().parentBuilder 반환(State<MovingState>("Approach") 에서 생성된 StateBuilder)
                .End()//StateBuilder().parentBuilder 반환(제일 상위인 StateMachineBuilder)
            .Build();//rootState 반환
        ////================
        StartCoroutine(StartMoveCoroutine());
    }

    //ghost 오브젝트 출력 전환
    public void AddToCullingMask()
    {
        Camera.main.cullingMask = -1;
    }
    public void RemoveToCullingMask()
    {
        Camera.main.cullingMask = ~(1 << gameObject.layer);
    }
    /* 헌팅 지속시간 체크
     */
    public bool isHuntingTimeOver()
    {
        bool isHuntingTimeOver = false;
        ghost.CurrentHuntingDuration -= Time.deltaTime;
        if (ghost.CurrentHuntingDuration <= 0f)
        {
            ghost.CurrentHuntingDuration = ghost.MaxHuntingDuration;
            isHuntingTimeOver = true;
        }
        return isHuntingTimeOver;
    }
    /* 헌팅타임 체크
     * 조건1. 현재 정신력
     * 조건2. 정신력 일정수치 소모 이후 유예시간 카운트
     */
    public bool isHuntingTime()
    {
        int aMinute = 60;
        bool isHuntingTime = false;
        if(player.CurrentMentalityStat >= 0f)
        {
            player.CurrentMentalityStat -= Time.deltaTime * player.MentalityStatLossPerMinute / aMinute;
        }
        if (player.CurrentMentalityStat <= ghost.HuntingMentalityStat)
        {
            //유예시간 체크 및 유예시간 감소
            if(checkGraceTime())
            {
                player.CurrentMentalityStat = player.MaxMentalityStat;
                isHuntingTime = true;
            }
        }
        return isHuntingTime;
    }
    //헌팅 유예시간 체크
    public bool checkGraceTime()
    {
        bool checkGraceTime = false;
        ghost.CurrentHuntingGraceTime -= Time.deltaTime;
        if (ghost.CurrentHuntingGraceTime <= 0f)
        {
            checkGraceTime = true;
            ghost.CurrentHuntingGraceTime = ghost.MaxHuntingGraceTime;
        }
        return checkGraceTime;
    }

    //투명화 조절하는 함수
    public void ChangeTransparency(float deltaTime)
    {
        if (ghost.IsVisible)//실체화일때
        {
            ghost.VisibleStateCount += deltaTime;
            if (ghost.VisibleStateCount >= ghost.VisibleTime)
            {
                ghost.VisibleStateCount = 0f;
                Color currentColor = ghost.GetComponent<SpriteRenderer>().color;
                currentColor.a = 0f;
                ghost.GetComponent<SpriteRenderer>().color = currentColor;
                ghost.IsVisible = !ghost.IsVisible;
            }
        }
        else
        {
            ghost.VisibleStateCount += deltaTime;
            if (ghost.VisibleStateCount >= ghost.InvisibleTime)
            {
                ghost.VisibleStateCount = 0f;
                Color currentColor = ghost.GetComponent<SpriteRenderer>().color;
                currentColor.a = 1f;
                ghost.GetComponent<SpriteRenderer>().color = currentColor;
                ghost.IsVisible = !ghost.IsVisible;
            }
        }
    }



    public bool isCollidedOrAccess()
    {
        bool isCollidedOrAccessCheck = false;
        bool AccessCheck = (Vector2.Distance(transform.position,
            ghost.TargetPosition) < ghost.CatchingDistance);      
        if (this.isColliding
            || AccessCheck)
        {
            isCollidedOrAccessCheck = true;
            this.isColliding = false;
        }
        return isCollidedOrAccessCheck;
    }

    public IEnumerator StartMoveCoroutine()
    {
        rootState.ChangeState("Peaceful");
        while (true)
        {
            rootState.Update(Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    public void MoveToDiretion(Vector2 Direction, float DeltaTime)
    {
        Direction.Normalize();
        transform.Translate(Direction * DeltaTime * ghost.MoveSpeed);
    }


    /* 플레이어의 시야각을 θ라고 하면, Forward 단위벡터와 
     * 타겟과 플레이어의 거리 차이로 나오는 단위벡터A 간의 내적이 cos(θ/2)보다 커야 시야 내에 존재한다.
     */
    public bool isPlayerInSight()
    {
        bool isPlayerInSight = false;
        Vector2 originPosition = (Vector2)this.transform.position;
        //범위안의 targetLayer들 정보 반환
        Collider2D[] hitedTargets = Physics2D.OverlapCircleAll(originPosition, ghost.SightRadius, ghost.TargetLayer);
        foreach (Collider2D hitedTarget in hitedTargets)
        {
            if(isTargetInLayer(hitedTarget, originPosition))
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
        Vector2 directionVector = (Vector2)ghost.TargetPosition - originPosition;
        Vector2 toPlayerDirectionVector = targetPos - originPosition;
        //각도 내에 들어왔는지 체크
        float angle = Vector2.Angle(directionVector, toPlayerDirectionVector);
        if (angle <= ghost.SightAngle * 0.5f)
        {
            RaycastHit2D rayHitedTarget =
                Physics2D.Raycast(originPosition, toPlayerDirectionVector,
                                ghost.SightRadius, ghost.TargetLayer);
            if (rayHitedTarget)
            {
                ghost.TargetPosition = targetPos;
                isTargetInLayer = true;
            }
        }
        return isTargetInLayer;
    }


    //특정 layer 부딪히면 MoveTo 중지
    private void OnTriggerExit2D(Collider2D collision)
    {
        this.isColliding = collidingCheck(collision);
    }

    private bool collidingCheck(Collider2D collision)
    {
        bool isColliding = false;
        int collidedLayer = collision.gameObject.layer;//부딪힌 Object의 layer를 반환
        if (((1 << collidedLayer) & ghost.CollisionLayer) != 0)
        {
            Vector2 pushDirection = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
            transform.Translate(pushDirection * 0.2f, Space.World);
            isColliding = true;
        }
        return isColliding;
    }

    ////디버깅용
    //void OnDrawGizmos()//유니티 지원 함수
    //{
    //    if (debugMode && ghost != null)
    //    {
    //        // 부채꼴 그리기
    //        DrawFieldOfView();
    //        attachLightToDirection(ghost.TargetPosition - transform.position);
    //    }
    //}

    //=============================귀신 불빛시야 관련 함수, 필요 없을 시 주석처리 또는 삭제
    //void attachLightToDirection(Vector2 directionVector)
    //{
    //    /* 2D 화면에서 rotation (0, 0, 0) 기준 시야방향을 지정합니다.
    //     * 기본 시야방향이 위를 보고 있기 때문에 위 방향을 나타내는 벡터로 지정합니다.
    //     */
    //    Vector2 sightDirection = new Vector2(0, 1);
    //    /* Quaternion.FromToRotation(Vector3 FromDirection, Vector3 ToDirection);
    //     * FromDirection이 ToDirection와 같은 방향을 보기 위한 회전값(쿼터니언)을 반환합니다.
    //     */
    //    Quaternion rotationToTarget = Quaternion.FromToRotation(
    //        sightDirection, directionVector);
    //    // 회전 각도 구하기
    //    float angle = rotationToTarget.eulerAngles.z;
    //    if(ghostSight != null)
    //       ghostSight.transform.rotation = Quaternion.Euler(0, 0, angle);//==========
    //}
    ////=============================귀신 불빛시야 관련 함수, 필요 없을 시 주석처리 또는 삭제


    //void DrawFieldOfView()
    //{
    //    /*
    //     * 1.목표좌표 - 내 좌표를 뺀 값을 구하고 directionVector로 정의
    //     * 내 좌표 -> 목표좌표로 이동하는 방향벡터는, (0, 0, 0)에서, directrionVector로 이동하는 방향과 같음 
    //     */
    //    Ghost ghost = transform.GetComponent<Ghost>();//play모드 아닐 때, 할당되지 않아 여기서 따로 선언
    //    Vector2 directionVector = ghost.TargetPosition - transform.position;
    //    /*
    //     * 2. z축 왼쪽 22.5, 오른쪽 22.5도 회전한 오일러각도를 쿼터니언 값으로 변환
    //     * rotation 값이 플러스 값이면 왼쪽으로 회전, 마이너스 값이면 오른쪽으로 회전 입니다.
    //     */
    //    Quaternion leftSightRotation = Quaternion.Euler(0, 0, ghost.SightAngle * 0.5f);
    //    Quaternion rightSightRotation = Quaternion.Euler(0, 0, -ghost.SightAngle * 0.5f);
    //    /*
    //     * 3.(0, 0, 0) 기준 directionVector까지의 선을 Z축 22.5도 회전한 선의 끝 좌표를 구하고 
    //     * leftRotatedDirection로 정의, transform.position(내 좌표) + leftRotatedDirection을 더하면,
    //     * 왼쪽으로 22.5도만큼 회전한 진짜 좌표를 구할 수 있음
    //     * 오른쪽도 위와 동일하게 하기
    //     */
    //    Vector2 rightRotatedDirection = rightSightRotation * directionVector;
    //    Vector2 leftRotatedDirection = leftSightRotation * directionVector;
    //    /*
    //     * RotatedDirection들을 길이가 1인 벡터로 바꾸고, 내 좌표에 반지름만큼 곱한 값을 더함
    //     */
    //    rightRotatedDirection.Normalize();
    //    leftRotatedDirection.Normalize();
    //    Vector2 rightSightDirection = (Vector2)transform.position + rightRotatedDirection * ghost.SightRadius;
    //    Vector2 leftSightDirection = (Vector2)transform.position + leftRotatedDirection * ghost.SightRadius;

    //    Handles.color = Color.red;
    //    Handles.DrawLine(transform.position, ghost.TargetPosition);//이동예정좌표 표시용
    //    Handles.DrawLine(transform.position, leftSightDirection);
    //    Handles.DrawLine(transform.position, rightSightDirection);
    //    //호를 그릴 중심 좌표
    //    /*
    //     * DrawWireArc 함수를 사용하여 호 그리기
    //     * Handles.DrawWireArc(center, normal, from, angle, radius)
    //     * center : 호를 포함한 원의 중앙좌표(걍 내 위치)
    //     * normal : 호를 그릴 방향이 담긴 rotation 값입니다..
    //     * from : 호를 그리기 시작할 좌표의 방향(실제 찍히는 위치는 radius에 의해 결정됨, 
    //     * 호의 첫번째 점의 방향만 지정해주는 것)
    //     * angle : angle만큼 + 한 rotation까지 점을 찍어줌
    //     * (호를 그리는 방향은 normal에 의해 결정됩니다.angle은 그냥 normal의 방향대로 몇 도까지 점을 찍어줄 지 결정)
    //     */
    //    Handles.DrawWireArc(transform.position, new Vector3(0, 0, 1), leftRotatedDirection, -ghost.SightAngle, ghost.SightRadius);
    //}

}
