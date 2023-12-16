using System;
using UnityEngine;
using UnityEngine.UI;

public class CheckEvidenceToggle : MonoBehaviour
{
    /* 체크 한 증거를 GameManager에 전달
     * GameManager가 체크 이벤트 일어날 때마다, 
     * GameManager의 증거와 관련된 함수 실행
     * 
     * 증거가 체크될 때마다, EscUI -> GhostSelectUI 안에 있는
     * CheckEvidenceToggle의
     * 선택 가능한 귀신 개수 줄어듬
     * 
     */

    //EvidenceSelectUI가 자동생성 해줄 때 할당
    [SerializeField] Toggle toggle;//지금 토글
    [SerializeField] int evidenceNumber;
    [SerializeField] Text evidenceText;

    public int EvidenceNumber
    {
        get { return evidenceNumber; }
        set { evidenceNumber = value; }
    }

    private void Start()
    {
        
        toggle.onValueChanged.AddListener(OnToggleValueChangedEvent);//이벤트에 함수 추가
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
            /* GameManager 안의 Evidences 배열 요소 추가
             * 체크 해제 시 제거 o
             * 증거가 체크될 때마다, EscUI -> GhostSelectUI 안에 있는
             * CheckEvidenceToggle의
             * 선택 가능한 귀신 개수 줄어듬
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
            /* GameManager 안의 Evidences 배열 요소 있는지 체크 후에 제거
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
