using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItemList", menuName = "ScriptableObjects/InventoryItemList")]
public class InventoryItemList : ScriptableObject {
    public List<InventoryItem> itemList;
}
