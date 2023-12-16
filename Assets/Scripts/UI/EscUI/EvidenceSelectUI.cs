using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceSelectUI : MonoBehaviour
{
    //Ȱ��ȭ ��, 7���� CheckEvidenceToggle ����
    List<CheckEvidenceToggle> toggleList = new List<CheckEvidenceToggle>();//slot ��� �迭
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
