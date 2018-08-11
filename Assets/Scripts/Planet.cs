using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Planet {

    public Vector2 Position { get; private set; }
    public bool IsVisited { get; private set; } = false;
    public List<KeyValuePair<InventoryItem, ItemType>> InventoryItemsPurchasable { get; private set; }
    public List<KeyValuePair<InventoryItem, ItemType>> InventoryItemsSellable { get; private set; }
    InventoryItemsDistributor inventoryItemsDistributor;

    public Planet(Vector2 position, bool startingLocation, InventoryItemsDistributor distributor) {
        Position = position;
        IsVisited = startingLocation;
        inventoryItemsDistributor = distributor;

        InventoryItemsPurchasable = new List<KeyValuePair<InventoryItem, ItemType>>();
        InventoryItemsSellable = new List<KeyValuePair<InventoryItem, ItemType>>();
        StockItemsPurchasable();
        StockItemsSellable();
    }

    public KeyValuePair<InventoryItem, ItemType> PurchaseItem(int index) {
        
        KeyValuePair<InventoryItem, ItemType> item = InventoryItemsPurchasable[index];
        InventoryItemsPurchasable.RemoveAt(index);
        return item;
    }

    public void StockItemsPurchasable() {
        StockList(InventoryItemsPurchasable);
    }

    public void StockItemsSellable() {
        StockList(InventoryItemsSellable);
    }

    public void SetVisited() {
        IsVisited = true;
    }

    public void StockList(List<KeyValuePair<InventoryItem, ItemType>> list) {
        list.Clear();
        list.AddRange(inventoryItemsDistributor.GenerateItemList());
    }
}
