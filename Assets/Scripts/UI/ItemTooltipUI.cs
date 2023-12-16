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
     * ItemDatabase Ŭ�������� ������ �̹���, �̸� ������ ItemSlot�� �̹���, ������ ���� ����ϴ� �Լ�
     */
    public void SetItem(int ItemCode)
    {
        //currentItemCode = ItemCode;//���� ��� ����
        Icon.sprite = ItemDatabase.getItemDatabase.GetItemImage(ItemCode);
        ItemDetailText.text = ItemDatabase.getItemDatabase.GetItemName(ItemCode);
    }

}
