using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEngine.UI.Image icon;//슬롯에 출력할 이미지
    [SerializeField] UnityEngine.UI.Text itemNameText;//슬롯에 출력할 아이템 이미지
    [SerializeField] UnityEngine.UI.Text itemCountText; //슬롯에 출력할 아이탬 개수
    [SerializeField] UnityEngine.UI.Text printIsEquipedText;//장착한 아이템인지 출력할 텍스트

    private int currentItemCode;//현재 슬롯에 있는 아이템의 ItemCode
    private int slotNum; //아이템 슬롯 번호
    private int itemCount;//아이템 개수
    private bool isEquiped = false;//장착된 슬롯인지
    private string equipedMark = "E";//장착표시

    public int CurrentItemCode
    {
        get{ return currentItemCode; }
        set{ currentItemCode = value; }
    }
    public int SlotNum
    {
        get{ return slotNum; }
        set{ slotNum = value; }
    }
    public int ItemCount
    {
        get { return itemCount; }
        set { itemCount = value; }
    }
    public UnityEngine.UI.Image getIcon()
    {
        return this.icon;
    }


    //ItemDatabase 클래스에서 아이템 이미지, 이름 가져와 ItemSlot에 이미지, 아이템 개수 출력하는 함수
    public void SetItem(int ItemCode)
    {
        currentItemCode = ItemCode;
        icon.sprite = ItemDatabase.getItemDatabase.GetItemImage(ItemCode);
        itemNameText.text = ItemDatabase.getItemDatabase.GetItemName(ItemCode);
    }

    public void SetItemCount(int count)
    {
        ItemCount = count;
        itemCountText.text = count.ToString();
    }

    //인벤토리 아이템 커서 올릴 시 출력하는 상세
    public void OpenItemDetail()
    {
        ItemDatabase.getItemDatabase.OpenDetail(currentItemCode);
    }

    //인벤토리 아이템 커서 뺄 시 출력하는 상세
    public void CloseItemDetail()
    {
        ItemDatabase.getItemDatabase.CloseDetail();
    }

    [SerializeField] float doubleClickInterval = 0.3f;//더블클릭 간격
    float lastClickedTime = 0f;//마지막 클릭시간 기록용
    public void OnPointerClick(PointerEventData eData)
    {
        if ((Time.time - lastClickedTime) < doubleClickInterval)
        {
            lastClickedTime = 0f;
            //더블 클릭 시 원하는함수 실행
            if(isEquiped == false)
            {
                if (EquipPoint.equipPoint.isEquipPointHasSpace()
                    && EquipedUI.equipedUI.isEquipUIHasSpace())
                {
                    EquipItem();
                }
                else
                {
                    //찼으면 장착 x, 장착 못한다는 메세지 출력
                }
            }
            else
            {
                UnequipItem();
            }
        }
        else
        {
            lastClickedTime = Time.time;
        }
    }

    /*
     * ItemSlot에 E(장착문자) 출력
     * EquipPoint.setEquip 함수로 EquipPoint 자식 Item 오브젝트 생성(EquipPoint 오브젝트 필요)o
     * EquipedUI 하위 오브젝트에 추가
     */
    public void EquipItem()
    {
        GameObject getItemObject = ItemDatabase.getItemDatabase.GetItemObject(currentItemCode);
        EquipPoint.equipPoint.equipItem(SlotNum, getItemObject);
        EquipedUI.equipedUI.equipImage(this);
        isEquiped = true;
        printIsEquipedText.text = equipedMark;
    }
    public void UnequipItem()
    {
        EquipPoint.equipPoint.unEquipItem(SlotNum);
        EquipedUI.equipedUI.unEquipImage(SlotNum);
        isEquiped = false;
        printIsEquipedText.text = "";
    }

}
