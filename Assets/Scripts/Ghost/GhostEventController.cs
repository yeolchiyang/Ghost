using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEventController : MonoBehaviour
{
    [SerializeField] float dotProjectorTimer = 5f;//도트 프로젝터 이벤트 출력 대기시간
    [SerializeField] float dotProjectorEventDelay = 5f;//도트 프로젝터 이벤트 출력 주기
    [SerializeField] float dotProjectorEventProbability = 10f;//도트 프로젝터 이벤트 발생확률(단위 : %)
    [SerializeField] float dotProjectorEventTimer = 2f; //도트 이벤트 남은시간 Count용
    [SerializeField] float dotProjectorEventDuration = 2f; //도트 이벤트 지속시간
    bool isDotProjectorEventing = false;//이벤트 일으킬지 검사
    bool isDotProjectorEventCoroutineStarted = false;//이벤트 일어남


    [SerializeField] float ghostWritingTimer = 5f;//고스트 라이팅 이벤트 출력 대기시간
    [SerializeField] float ghostWritingEventDelay = 5f;//고스트 라이팅 이벤트 출력 대기시간
    [SerializeField] float ghostWritingEventProbability = 10f;//고스트 라이팅 이벤트 출력 대기시간
    [SerializeField] float ghostWritingEventTimer = 2f;//고스트 라이팅 이벤트 출력 대기시간
    [SerializeField] float ghostWritingEventDuration = 2f;//고스트 라이팅 이벤트 출력 대기시간
    bool isGhostWritingEventing = false;
    bool isGhostWritingEventCoroutineStarted = false;


    private void Awake()
    {
        dotProjectorTimer = dotProjectorEventDelay;
        ghostWritingTimer = ghostWritingEventDelay;
    }
    /* 이벤트 관련된 아이템 클래스에 해당 메서드 붙여서 
     * 작동하는 로직으로 짜야 함
     */
    public void executeEvent(Ghost.GhostEvidences evidenceEnum)
    {
        Ghost.EventCondition condition;
        if (GetComponent<Ghost>().ghostEvents.TryGetValue(evidenceEnum, out condition))
        {
            Debug.Log("executeEvent : " + evidenceEnum);
            if (condition.Predicate())
            {
                condition.Action();
            }
        }
    }
    //==================================================
    /* 도트 이벤트 관련 함수
     * 1.일정 시간마다 판단
     * 2.이벤트시 isEventing = true
     * 3.이벤트 지속시간 체크, 이벤트동안 dotProjectorTimer 안내려감
     */
    public void dotProjector()
    {
        Debug.Log("dotProjector");
        if(checkEventCondition(ref isDotProjectorEventing,
            ref isDotProjectorEventCoroutineStarted,
            ref dotProjectorTimer, 
            dotProjectorEventDelay, dotProjectorEventProbability))
        {
            StartCoroutine(dotProjectorEventCount());
        }
    }
    IEnumerator dotProjectorEventCount()
    {
        while (true)
        {
            GetComponent<GhostController>().AddToCullingMask();//투명화 잠시해제
            dotProjectorEventTimer -= Time.deltaTime;
            if (dotProjectorEventTimer <= 0f)
            {
                GetComponent<GhostController>().RemoveToCullingMask();//다시 투명화
                dotProjectorEventTimer = dotProjectorEventDuration;
                isDotProjectorEventing = false;
                isDotProjectorEventCoroutineStarted = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    //==================================================

    public void EMFLevel5()
    {
        Debug.Log("EMFLevel5");
    }

    public void FreezingTemperatures()
    {
        Debug.Log("FreezingTemperatures");
    }
    public void GhostOrb()
    {
        Debug.Log("GhostOrb");
    }
    //============================================================
    /* 고스트 라이팅 관련 함수
     * FoldedNote에서 주기적으로 아래 함수들 실행하도록 요청
     * 요청할 때마다 ghostWritingTimer 증가
     */
    private float detectionRadius = 5f;
    public void GhostWriting()
    {
        Debug.Log("GhostWriting");
        if (checkEventCondition(ref isGhostWritingEventing,
            ref isGhostWritingEventCoroutineStarted,
            ref ghostWritingTimer,
            ghostWritingEventDelay, ghostWritingEventProbability))
        {
            Vector2 originPosition = (Vector2)this.transform.position;
            //범위안의 targetLayer들 정보 반환
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (Collider2D hitedTarget in colliders)
            {
                FoldedNote foldedNoteComponent = hitedTarget.GetComponent<FoldedNote>();
                // FoldedNote 컴포넌트가 존재하면 openedNote 함수 호출
                // 자꾸 null뜨는 원인찾아 해결하기
                if (foldedNoteComponent != null)
                {
                    StartCoroutine(GhostWritingEventCount(foldedNoteComponent));
                }
            }
        }
    }
    IEnumerator GhostWritingEventCount(FoldedNote foldedNoteComponent)
    {
        while (true)
        {
            ghostWritingEventTimer -= Time.deltaTime;
            if (ghostWritingEventTimer <= 0f)
            {
                StartCoroutine(foldedNoteComponent.changeIsEvent());
                ghostWritingEventTimer = ghostWritingEventDuration;
                isGhostWritingEventing = false;
                isGhostWritingEventCoroutineStarted = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    //===============================================================

    public void SpiritBox()
    {
        Debug.Log("SpiritBox");
    }
    public void Ultraviolet()
    {
        Debug.Log("Ultraviolet");
    }

    //이벤트 발생조건 체크
    public bool checkEventCondition(ref bool isEventing, 
        ref bool isEventCoroutineStarted,  
        ref float Timer, float EventDelay, float EventProbability)
    {
        if (!isEventing)//이벤트 발생시 isEventing = true
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0f)
            {
                Timer = EventDelay;
                isEventing = true;
            }
        }
        if (isEventing)//이벤트 발생 시 확률체크
        {
            if (!isEventCoroutineStarted)//이벤트 코루틴 실행되어 있는지 확인
            {
                //if 확률만족시 이벤트 코루틴 실행
                if (isEventChance(EventProbability))
                {
                    isEventCoroutineStarted = true;
                }
                else
                {
                    /* else 확률만족 못하면 코루틴 실행하지 않고, 
                     * 맨 위부터 반복 
                     */
                    isEventing = false;
                }
            }
        }
        return isEventCoroutineStarted;
    }

    //소숫점 6자리까지 확률 체크
    bool isEventChance(float probability)
    {
        bool isEventChance = false;
        float decimalPlacesVariable = 100f;
        float randomFloat = Random.value;
        float multipleRandom = randomFloat * decimalPlacesVariable;
        if (multipleRandom < probability)
        {
            isEventChance = true;
        }
        return isEventChance;
    }

}
