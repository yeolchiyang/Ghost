using System;
using UnityEngine;
using UnityEngine.UI;

public class CheckEvidenceToggle : MonoBehaviour
{
    /* üũ �� ���Ÿ� GameManager�� ����
     * GameManager�� üũ �̺�Ʈ �Ͼ ������, 
     * GameManager�� ���ſ� ���õ� �Լ� ����
     * 
     * ���Ű� üũ�� ������, EscUI -> GhostSelectUI �ȿ� �ִ�
     * CheckEvidenceToggle��
     * ���� ������ �ͽ� ���� �پ��
     * 
     */

    //EvidenceSelectUI�� �ڵ����� ���� �� �Ҵ�
    [SerializeField] Toggle toggle;//���� ���
    [SerializeField] int evidenceNumber;
    [SerializeField] Text evidenceText;

    public int EvidenceNumber
    {
        get { return evidenceNumber; }
        set { evidenceNumber = value; }
    }

    private void Start()
    {
        
        toggle.onValueChanged.AddListener(OnToggleValueChangedEvent);//�̺�Ʈ�� �Լ� �߰�
        string evidenceString =
            GameManager.gameManager.Ghost.EvidenceString[evidenceNumber];
        evidenceText.text = evidenceString;
        //OnToggleValueChangedEvent
    }

    public void OnToggleValueChangedEvent(bool isOn)
    {
        Debug.Log($"is {isOn}");
        if (isOn)
        {
            /* GameManager ���� Evidences �迭 ��� �߰�
             * üũ ���� �� ���� o
             * ���Ű� üũ�� ������, EscUI -> GhostSelectUI �ȿ� �ִ�
             * CheckEvidenceToggle��
             * ���� ������ �ͽ� ���� �پ��
             * 
             */
            if (GameManager.gameManager.Evidences.Count > 0)
            {
                Debug.Log(GameManager.gameManager.Evidences.Find(x => x == evidenceNumber));
            }
            else
            {
                GameManager.gameManager.Evidences.Add(evidenceNumber);
            }

        }
        else
        {
            /* GameManager ���� Evidences �迭 ��� �ִ��� üũ �Ŀ� ����
             * 
             */
            for (int index = 0; index < GameManager.gameManager.Evidences.Count; index++)
            {
                if (evidenceNumber == GameManager.gameManager.Evidences[index])
                {
                    GameManager.gameManager.Evidences.RemoveAt(index);   
                }
            }
        }
    }


}
