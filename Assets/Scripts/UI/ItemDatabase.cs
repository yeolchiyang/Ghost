using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase getItemDatabase;
    const int EMPTY_INDEX = 0;
    [SerializeField] List<Item> itemList;//hierachy 창에서 직접넣는 아이템 asset
    Item[] itemArray;//ItemCode 순으로 정렬용 배열
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

    public string GetItemName(int ItemCode)//아이템 이름 넣는 함수
    {
        return itemArray[ItemCode].itemName;
    }
    public Sprite GetItemImage(int ItemCode)
    {
        return itemArray[ItemCode].itemImage;
    }
    public GameObject GetItemObject(int ItemCode)//아이템 넣는 함수
    {
        return itemArray[ItemCode].itemObject;
    }

    //이 아래는 ItemTooltipUI로 선택 된 오브젝트에 상세를 표시하는 함수
    [SerializeField]
    ItemTooltipUI itemTooltipUI;

    public void OpenDetail(int ItemCode)//이건 잠시 무시
    {
        if (ItemCode != EMPTY_INDEX)
        {
            itemTooltipUI.SetItem(ItemCode);
            itemTooltipUI.gameObject.SetActive(true);
        }
    }
    public void CloseDetail()//이건 잠시 무시
    {
         itemTooltipUI.gameObject.SetActive(false);
    }



}
