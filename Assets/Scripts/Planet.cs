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

    private void StockItemsPurchasable() {
        StockList(InventoryItemsPurchasable);
    }

    private  void StockItemsSellable() {
        StockList(InventoryItemsSellable);
    }

    public void SetVisited() {
        IsVisited = true;
    }

    private void StockList(List<KeyValuePair<InventoryItem, ItemType>> list) {
        list.Clear();
        list.AddRange(inventoryItemsDistributor.GenerateItemList());
    }

    public void RemoveAndReplaceItem(int index, List<KeyValuePair<InventoryItem, ItemType>> list) {
        list.RemoveAt(index);
        list.Add(inventoryItemsDistributor.GenerateItem());
    }

}
