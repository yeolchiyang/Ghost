using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEngine.UI.Image icon;//���Կ� ����� �̹���
    [SerializeField] UnityEngine.UI.Text itemNameText;//���Կ� ����� ������ �̹���
    [SerializeField] UnityEngine.UI.Text itemCountText; //���Կ� ����� ������ ����
    [SerializeField] UnityEngine.UI.Text printIsEquipedText;//������ ���������� ����� �ؽ�Ʈ

    private int currentItemCode;//���� ���Կ� �ִ� �������� ItemCode
    private int slotNum; //������ ���� ��ȣ
    private int itemCount;//������ ����
    private bool isEquiped = false;//������ ��������
    private string equipedMark = "E";//����ǥ��

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


    //ItemDatabase Ŭ�������� ������ �̹���, �̸� ������ ItemSlot�� �̹���, ������ ���� ����ϴ� �Լ�
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

    //�κ��丮 ������ Ŀ�� �ø� �� ����ϴ� ��
    public void OpenItemDetail()
    {
        ItemDatabase.getItemDatabase.OpenDetail(currentItemCode);
    }

    //�κ��丮 ������ Ŀ�� �� �� ����ϴ� ��
    public void CloseItemDetail()
    {
        ItemDatabase.getItemDatabase.CloseDetail();
    }

    [SerializeField] float doubleClickInterval = 0.3f;//����Ŭ�� ����
    float lastClickedTime = 0f;//������ Ŭ���ð� ��Ͽ�
    public void OnPointerClick(PointerEventData eData)
    {
        if ((Time.time - lastClickedTime) < doubleClickInterval)
        {
            lastClickedTime = 0f;
            //���� Ŭ�� �� ���ϴ��Լ� ����
            if(isEquiped == false)
            {
                if (EquipPoint.equipPoint.isEquipPointHasSpace()
                    && EquipedUI.equipedUI.isEquipUIHasSpace())
                {
                    EquipItem();
                }
                else
                {
                    //á���� ���� x, ���� ���Ѵٴ� �޼��� ���
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
     * ItemSlot�� E(��������) ���
     * EquipPoint.setEquip �Լ��� EquipPoint �ڽ� Item ������Ʈ ����(EquipPoint ������Ʈ �ʿ�)o
     * EquipedUI ���� ������Ʈ�� �߰�
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
