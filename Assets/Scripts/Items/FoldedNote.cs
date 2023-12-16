using System.Collections;
using UnityEditor;
using UnityEngine;

public class FoldedNote : MonoBehaviour
{
    [SerializeField] LayerMask TargetLayer;
    [SerializeField] float foldedNoteRadius = 2f;
    [SerializeField] bool isDebug = false;
    [SerializeField] Sprite eventSprite;//��Ʈ ����Ʈ �̺�Ʈ�� ��ȯ �� sprite
    SpriteRenderer eventRenderer;//Sprite ������ ���� �ʿ��� Ŭ����
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

    //��Ʈ ������ �̺�Ʈ �߻� �� ����� �Լ�
    //�ѹ��̶� ����Ǹ�, �����·� ���ƿ��� ����
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
            // Sprite ����
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

    /* Ÿ�ٰ��� ��-> 
     * Update������ GhostEventController�� �̺�Ʈ �߻� �Լ� ����ȣ�� ��
     * GhostEventController
     */

    public bool isTargetInRange()
    {
        bool isTargetInRange = false;
        Vector2 originPosition = (Vector2)this.transform.position;
        //�������� targetLayer�� ���� ��ȯ
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
        //����ġ->Player�� ���� ���� ���Ⱒ�� ���ϱ�
        Vector2 targetPos = hitedTarget.transform.position;
        Vector2 directionVector = this.transform.up;
        Vector2 toPlayerDirectionVector = targetPos - originPosition;
        //���� ���� ���Դ��� üũ
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
    //     * DrawWireArc �Լ��� ����Ͽ� ȣ �׸���
    //     * Handles.DrawWireArc(center, normal, from, angle, radius)
    //     * center : ȣ�� ������ ���� �߾���ǥ(���� �߾��� �� ��ġ transform.position �� ���մϴ�)
    //     * normal : ȣ�� �׸� ������ ���� rotation ���Դϴ�. ���� ���� �ִµ�, new Vector3(0, 0, 1)��� Z���� ��������� ȸ����ų �������� �׸��ڴٴ� ���Դϴ�.
    //             * new Vector3(0, 0, 1)�̵� new Vector3(0, 0, 180)�̵� ���� ũ��� ��������ϴ�. �׳� ȣ�� �׸� ������ �����մϴ�.
    //     * from : ȣ�� �׸��� ������ ��ǥ�� ����(���� ������ ��ġ�� radius�� ���� ������, ȣ�� ù��° ���� ���� ���⸸ �������ִ� ��)
    //     * angle : angle��ŭ + �� rotation���� ���� �����
    //     * (ȣ�� �׸��� ������ normal�� ���� �����˴ϴ�.angle�� �׳� normal�� ������ �� ������ ���� ����� �� ����, angle�� ���̳ʽ��� �ݴ������ �˴ϴ�)
    //     */
    //    Handles.color = Color.red;
    //    Handles.DrawWireArc(transform.position, new Vector3(0, 0, 1), new Vector3(0, 1, 0), 360f, foldedNoteRadius);
    //    //���� ����
    //    //Handles.DrawWireArc(transform.position, Vector3.forward, leftRotatedDirection, -sightAngle, sightRadius);
    //}

}
