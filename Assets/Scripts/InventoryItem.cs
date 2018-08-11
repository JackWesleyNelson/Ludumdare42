using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem  {

    public string itemName = "New Item";
    public Texture2D itemIcon = null;
    public ItemQuality quality = ItemQuality.Low;
    public int size = 1;

}
