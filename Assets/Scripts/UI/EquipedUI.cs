using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipedUI : MonoBehaviour
{
    public static EquipedUI equipedUI;
    [SerializeField] UnityEngine.UI.Image IconImage;//슬롯에 출력할 이미지
    ItemSlot[] Equipslot; //장착한 이미지들 모음(ItemSlot 클래스에서 넣어줌)
    const int ALLOWED_EQUIP_COUNT = 3;//아이템 장착 가능한 개수
    int currentIconImageSlotNum;//현재 슬롯에 띄울 장비 index
    int currentEquipslotIndex = 0;//현재 표시중인 Equipslot index

    private void Awake()
    {
        if (equipedUI == null)
        {
            equipedUI = this;
        }
        this.Equipslot = new ItemSlot[ALLOWED_EQUIP_COUNT];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //changeEquipImage 실행 후 currentIconImageSlotNum이 바뀌기에, 실행 
            changeEquipImage();
            EquipPoint.equipPoint.changeEquipObject(currentIconImageSlotNum);
        }
    }

    public void equipImage(ItemSlot itemSlot)
    {
        for (int i = 0; i < ALLOWED_EQUIP_COUNT; i++)
        {
            if(this.Equipslot[i] == null)
            {
                this.Equipslot[i] = itemSlot;
                this.IconImage.sprite = this.Equipslot[i].getIcon().sprite;
                this.currentIconImageSlotNum = itemSlot.SlotNum;
                this.currentEquipslotIndex = i;
                break;
            }
        }
    }
    public void unEquipImage(int SlotNum)
    {
        for (int i = 0; i < ALLOWED_EQUIP_COUNT; i++)
        {
            if (this.Equipslot[i] != null)
            {
                if (SlotNum == this.Equipslot[i].SlotNum)
                {
                    int deleteTargetNum = this.Equipslot[i].SlotNum;
                    if(this.currentIconImageSlotNum == deleteTargetNum)
                    {
                        this.IconImage.sprite = null;
                    }
                    this.Equipslot[i] = null;
                }
            }
        }
    }
    public bool isEquipUIHasSpace()
    {
        bool hasSpace = false;
        foreach(ItemSlot itemSlot in this.Equipslot)
        {
            if(itemSlot == null)
            {
                hasSpace = true;
                break;
            }
        }
        return hasSpace;
    }

    public void changeEquipImage()//현재 UIIndex 변경, 이미지 변경
    {
        for (int i = 0; i < Equipslot.Length; i++)
        {
            if (i == currentEquipslotIndex)
            {
                if ((i + 1) == Equipslot.Length)
                {
                    currentEquipslotIndex = 0;
                }
                else
                {
                    currentEquipslotIndex = i + 1;
                }
                break;
            }
        }
        IconImage.sprite = Equipslot[currentEquipslotIndex].getIcon().sprite;
        currentIconImageSlotNum = Equipslot[currentEquipslotIndex].SlotNum;
    }
}
