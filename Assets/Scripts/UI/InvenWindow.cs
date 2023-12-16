using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//���� ���� �κ�â
public class InvenWindow : MonoBehaviour
{
    List<ItemSlot> slotlist = new List<ItemSlot>();//slot ��� �迭
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform SlotWindow;
    const int EMPTY_SLOT_INDEX = 0;

    private void Start()
    {
        GameObject instantSlot = null;

        for(int i = 0; i < 12; ++i)
        {
            instantSlot = Instantiate(slotPrefab, SlotWindow);
            ItemSlot slot = instantSlot.GetComponent<ItemSlot>();
            slot.SlotNum = i;
            slotlist.Add(slot);
        }

        /*
         * �κ��丮 ä��� �׽�Ʈ������ ���� �Լ�
         */
        InsertItem(1);
        InsertItem(2);
        InsertItem(3);
    }
    void InsertItem(int ItemCode)
    {
        for (int i = 0; i < slotlist.Count; ++i)
        {
            if (slotlist[i].CurrentItemCode == EMPTY_SLOT_INDEX)
            {
                slotlist[i].SetItem(ItemCode);
                slotlist[i].SetItemCount(1);//===========================������ ����(�ӽ�)
                return;
            }
        }
    }


}
