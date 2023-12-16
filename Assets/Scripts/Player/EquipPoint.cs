using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EquipPoint : MonoBehaviour
{
    public static EquipPoint equipPoint;
    Dictionary<int, GameObject> equipedGameObject = new Dictionary<int, GameObject>();//장착한 오브젝트들 개수
    const int ALLOWED_EQUIP_COUNT = 3;//아이템 장착 가능한 개수
    int currentEquipedSlotNum = 0;//현재 슬롯에 띄울 장비 SlotNum(ItemSlot.SlotNum)
    private void Awake()
    {
        if (equipPoint == null)
            equipPoint = this;
    }

    //장착
    public void equipItem(int SlotNum, GameObject itemObject)
    {
        //기존에 들고있던것 비활성화
        foreach(KeyValuePair<int, GameObject> equipedItem in equipedGameObject)
        {
            equipedItem.Value.gameObject.SetActive(false);
        }
        GameObject instantObject = Instantiate(itemObject, transform.position, transform.rotation, transform);
        this.equipedGameObject.Add(SlotNum, instantObject);
        this.currentEquipedSlotNum = SlotNum;
    }
    //장착 해제
    public void unEquipItem(int SlotNum)
    {
        GameObject unEquiptargetItem = null;
        if(this.equipedGameObject.ContainsKey(SlotNum))
        {
            unEquiptargetItem = equipedGameObject[SlotNum];
            this.equipedGameObject.Remove(SlotNum);
            Destroy(unEquiptargetItem);
        }
    }

    public bool isEquipPointHasSpace()
    {
        bool equipPointHasSpace = true;
        if(this.equipedGameObject.Count == ALLOWED_EQUIP_COUNT)
        {
            equipPointHasSpace = false;
        }
        return equipPointHasSpace;
    }

    public void changeEquipObject(int slotNum)
    {
        //기존에 들고있던것 비활성화
        foreach (KeyValuePair<int, GameObject> equipedItem in equipedGameObject)
        {
            if(slotNum == equipedItem.Key) 
            {
                equipedItem.Value.gameObject.SetActive(true);
                this.currentEquipedSlotNum = slotNum;
            }
            else
            {
                equipedItem.Value.gameObject.SetActive(false);
            }
            
        }

    }
}
