using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceSelectUI : MonoBehaviour
{
    //활성화 시, 7개의 CheckEvidenceToggle 생성
    List<CheckEvidenceToggle> toggleList = new List<CheckEvidenceToggle>();//slot 담는 배열
    [SerializeField] GameObject togglePrefab;

    private void Start()
    {
        GameObject instantToggle = null;

        for (int i = 0; i < 7; ++i)
        {
            instantToggle = Instantiate(togglePrefab, transform);
            CheckEvidenceToggle toggle = instantToggle.GetComponent<CheckEvidenceToggle>();
            toggle.EvidenceNumber = i;
            toggleList.Add(toggle);
        }

    }
}
