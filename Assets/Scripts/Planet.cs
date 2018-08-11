using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public Vector2 Position { get; private set; }
    public bool IsVisited { get; private set; } = false;
    public List<InventoryItem> InventoryItemsPurchasable { get; private set; }
    public List<InventoryItem> InventoryItemsSellable { get; private set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



}
