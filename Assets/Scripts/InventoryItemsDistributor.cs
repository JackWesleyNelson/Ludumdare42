using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemsDistributor : MonoBehaviour {

    public InventoryItemsDistributor Instance { get; private set; }
    [SerializeField]
    List<InventoryItemList> inventoryCollections;
    [SerializeField]
    List<ItemType> inventoryCollectionTypes;
    

    public void Start() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
        if(inventoryCollectionTypes.Count != inventoryCollections.Count) {
            Debug.LogError("InventoryItemsDistributor item count mismatch between inventoryCollections/inventoryCollectionTypes");
        }

    }

    public List<KeyValuePair<InventoryItem, ItemType>> GenerateItemList(int size = 5){
        List<KeyValuePair<InventoryItem, ItemType>> items = new List<KeyValuePair<InventoryItem, ItemType>>();
        for (int i = 0; i < size; i++) {
            //Get a random item collection type index
            int collectionIndex = Random.Range(0, inventoryCollections.Count);
            //Get a random item index from the collection
            int itemIndex = Random.Range(0,inventoryCollections[collectionIndex].itemList.Count);
            //Get the random item from the itemIndex from the collection at collectionIndex.
            //Add this item to the returning list.
            KeyValuePair<InventoryItem, ItemType> item = new KeyValuePair<InventoryItem, ItemType>(inventoryCollections[collectionIndex].itemList[itemIndex], inventoryCollectionTypes[collectionIndex]);
            items.Add(item);
        }
        return items;
    }

    public KeyValuePair<InventoryItem, ItemType> GenerateItem() {
        //Get a random item collection type index
        int collectionIndex = Random.Range(0, inventoryCollections.Count);
        //Get a random item index from the collection
        int itemIndex = Random.Range(0, inventoryCollections[collectionIndex].itemList.Count);
        //Get the random item from the itemIndex from the collection at collectionIndex.
        //Add this item to the returning list.
        KeyValuePair<InventoryItem, ItemType> item = new KeyValuePair<InventoryItem, ItemType>(inventoryCollections[collectionIndex].itemList[itemIndex], inventoryCollectionTypes[collectionIndex]);
        return item;
    }

}
