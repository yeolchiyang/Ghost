using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEventController : MonoBehaviour
{
    [SerializeField] float dotProjectorTimer = 5f;//��Ʈ �������� �̺�Ʈ ��� ���ð�
    [SerializeField] float dotProjectorEventDelay = 5f;//��Ʈ �������� �̺�Ʈ ��� �ֱ�
    [SerializeField] float dotProjectorEventProbability = 10f;//��Ʈ �������� �̺�Ʈ �߻�Ȯ��(���� : %)
    [SerializeField] float dotProjectorEventTimer = 2f; //��Ʈ �̺�Ʈ �����ð� Count��
    [SerializeField] float dotProjectorEventDuration = 2f; //��Ʈ �̺�Ʈ ���ӽð�
    bool isDotProjectorEventing = false;//�̺�Ʈ ����ų�� �˻�
    bool isDotProjectorEventCoroutineStarted = false;//�̺�Ʈ �Ͼ


    [SerializeField] float ghostWritingTimer = 5f;//��Ʈ ������ �̺�Ʈ ��� ���ð�
    [SerializeField] float ghostWritingEventDelay = 5f;//��Ʈ ������ �̺�Ʈ ��� ���ð�
    [SerializeField] float ghostWritingEventProbability = 10f;//��Ʈ ������ �̺�Ʈ ��� ���ð�
    [SerializeField] float ghostWritingEventTimer = 2f;//��Ʈ ������ �̺�Ʈ ��� ���ð�
    [SerializeField] float ghostWritingEventDuration = 2f;//��Ʈ ������ �̺�Ʈ ��� ���ð�
    bool isGhostWritingEventing = false;
    bool isGhostWritingEventCoroutineStarted = false;


    private void Awake()
    {
        dotProjectorTimer = dotProjectorEventDelay;
        ghostWritingTimer = ghostWritingEventDelay;
    }
    /* �̺�Ʈ ���õ� ������ Ŭ������ �ش� �޼��� �ٿ��� 
     * �۵��ϴ� �������� ¥�� ��
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
    /* ��Ʈ �̺�Ʈ ���� �Լ�
     * 1.���� �ð����� �Ǵ�
     * 2.�̺�Ʈ�� isEventing = true
     * 3.�̺�Ʈ ���ӽð� üũ, �̺�Ʈ���� dotProjectorTimer �ȳ�����
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
            GetComponent<GhostController>().AddToCullingMask();//����ȭ �������
            dotProjectorEventTimer -= Time.deltaTime;
            if (dotProjectorEventTimer <= 0f)
            {
                GetComponent<GhostController>().RemoveToCullingMask();//�ٽ� ����ȭ
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
    /* ��Ʈ ������ ���� �Լ�
     * FoldedNote���� �ֱ������� �Ʒ� �Լ��� �����ϵ��� ��û
     * ��û�� ������ ghostWritingTimer ����
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
            //�������� targetLayer�� ���� ��ȯ
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (Collider2D hitedTarget in colliders)
            {
                FoldedNote foldedNoteComponent = hitedTarget.GetComponent<FoldedNote>();
                // FoldedNote ������Ʈ�� �����ϸ� openedNote �Լ� ȣ��
                // �ڲ� null�ߴ� ����ã�� �ذ��ϱ�
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

    //�̺�Ʈ �߻����� üũ
    public bool checkEventCondition(ref bool isEventing, 
        ref bool isEventCoroutineStarted,  
        ref float Timer, float EventDelay, float EventProbability)
    {
        if (!isEventing)//�̺�Ʈ �߻��� isEventing = true
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0f)
            {
                Timer = EventDelay;
                isEventing = true;
            }
        }
        if (isEventing)//�̺�Ʈ �߻� �� Ȯ��üũ
        {
            if (!isEventCoroutineStarted)//�̺�Ʈ �ڷ�ƾ ����Ǿ� �ִ��� Ȯ��
            {
                //if Ȯ�������� �̺�Ʈ �ڷ�ƾ ����
                if (isEventChance(EventProbability))
                {
                    isEventCoroutineStarted = true;
                }
                else
                {
                    /* else Ȯ������ ���ϸ� �ڷ�ƾ �������� �ʰ�, 
                     * �� ������ �ݺ� 
                     */
                    isEventing = false;
                }
            }
        }
        return isEventCoroutineStarted;
    }

    //�Ҽ��� 6�ڸ����� Ȯ�� üũ
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
