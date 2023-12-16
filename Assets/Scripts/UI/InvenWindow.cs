using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//가장 상위 인벤창
public class InvenWindow : MonoBehaviour
{
    List<ItemSlot> slotlist = new List<ItemSlot>();//slot 담는 배열
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
         * 인벤토리 채우기 테스트용으로 넣은 함수
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
                slotlist[i].SetItemCount(1);//===========================아이템 개수(임시)
                return;
            }
        }
    }


}
