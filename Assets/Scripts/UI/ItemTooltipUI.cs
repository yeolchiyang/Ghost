using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Image Icon;

    [SerializeField]
    UnityEngine.UI.Text ItemDetailText;


    /*
     * ItemDatabase 클래스에서 아이템 이미지, 이름 가져와 ItemSlot에 이미지, 아이템 개수 출력하는 함수
     */
    public void SetItem(int ItemCode)
    {
        //currentItemCode = ItemCode;//현재 사용 안함
        Icon.sprite = ItemDatabase.getItemDatabase.GetItemImage(ItemCode);
        ItemDetailText.text = ItemDatabase.getItemDatabase.GetItemName(ItemCode);
    }

}
