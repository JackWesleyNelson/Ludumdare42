using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShipItemList", menuName = "ScriptableObjects/ShipItemList")]
public class ShipItemList: ScriptableObject {
    public List<ShipItem> itemList;
}
