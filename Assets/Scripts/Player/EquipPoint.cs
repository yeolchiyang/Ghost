using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EquipPoint : MonoBehaviour
{
    public static EquipPoint equipPoint;
    Dictionary<int, GameObject> equipedGameObject = new Dictionary<int, GameObject>();//������ ������Ʈ�� ����
    const int ALLOWED_EQUIP_COUNT = 3;//������ ���� ������ ����
    int currentEquipedSlotNum = 0;//���� ���Կ� ��� ��� SlotNum(ItemSlot.SlotNum)
    private void Awake()
    {
        if (equipPoint == null)
            equipPoint = this;
    }

    //����
    public void equipItem(int SlotNum, GameObject itemObject)
    {
        //������ ����ִ��� ��Ȱ��ȭ
        foreach(KeyValuePair<int, GameObject> equipedItem in equipedGameObject)
        {
            equipedItem.Value.gameObject.SetActive(false);
        }
        GameObject instantObject = Instantiate(itemObject, transform.position, transform.rotation, transform);
        this.equipedGameObject.Add(SlotNum, instantObject);
        this.currentEquipedSlotNum = SlotNum;
    }
    //���� ����
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
        //������ ����ִ��� ��Ȱ��ȭ
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
