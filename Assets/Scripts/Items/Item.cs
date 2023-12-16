using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/Item",
    order = int.MaxValue)]
public class Item : ScriptableObject
{
    public int itemCode;//Àý´ë °ãÄ¡¸é ¾ÈµÊ
    public string itemName;
    public Sprite itemImage;
    public GameObject itemObject;

}
