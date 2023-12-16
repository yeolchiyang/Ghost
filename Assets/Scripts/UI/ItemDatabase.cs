using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase getItemDatabase;
    const int EMPTY_INDEX = 0;
    [SerializeField] List<Item> itemList;//hierachy â���� �����ִ� ������ asset
    Item[] itemArray;//ItemCode ������ ���Ŀ� �迭
    private void Awake()
    {
        if (getItemDatabase == null)
            getItemDatabase = this;
        itemArray = new Item[itemList.Count];
        foreach(Item item in itemList)
        {
            itemArray[item.itemCode] = item;
        }
    }

    public string GetItemName(int ItemCode)//������ �̸� �ִ� �Լ�
    {
        return itemArray[ItemCode].itemName;
    }
    public Sprite GetItemImage(int ItemCode)
    {
        return itemArray[ItemCode].itemImage;
    }
    public GameObject GetItemObject(int ItemCode)//������ �ִ� �Լ�
    {
        return itemArray[ItemCode].itemObject;
    }

    //�� �Ʒ��� ItemTooltipUI�� ���� �� ������Ʈ�� �󼼸� ǥ���ϴ� �Լ�
    [SerializeField]
    ItemTooltipUI itemTooltipUI;

    public void OpenDetail(int ItemCode)//�̰� ��� ����
    {
        if (ItemCode != EMPTY_INDEX)
        {
            itemTooltipUI.SetItem(ItemCode);
            itemTooltipUI.gameObject.SetActive(true);
        }
    }
    public void CloseDetail()//�̰� ��� ����
    {
         itemTooltipUI.gameObject.SetActive(false);
    }



}
