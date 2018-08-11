using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
    public List<ShipItem> ShipItems { get; private set; }
    public List<ShipItem> ShipItemsCatalogue { get; private set; }
    [SerializeField]
    ShipItemList ShipItemsCatalogueSerializable;
    public int ShipItemSize { get; private set; } = 4;
    public int InventoryItemSize { get; private set; } = 4;


    public void Start() {
        ShipItems = new List<ShipItem>();
        ShipItemsCatalogue = new List<ShipItem>(ShipItemsCatalogueSerializable.shipItemList);
        //Add all the zero cost default items from the catalogue to the ship, and add them to the ship
        for(int i = ShipItemsCatalogue.Count - 1; i >= 0; i--) {
            ShipItem shipItem = ShipItemsCatalogue[i];
            if (shipItem.price == 0) {
                ShipItems.Add(shipItem);
                ShipItemsCatalogue.RemoveAt(i);
            }
        }
    }

    public void MoveShip(Planet p) {

    }

}
