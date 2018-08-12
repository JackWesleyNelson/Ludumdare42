using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
    public Ship Instance { get; private set; }
    public List<ShipItem> ShipItems { get; private set; }
    public List<ShipItem> ShipItemsCatalogue { get; private set; }
    [SerializeField]
    ShipItemList ShipItemsCatalogueSerializable;
    public int InventoryItemSize { get; private set; } = 0;
    public int InventoryItemSizeMax { get; private set; } = 4;
    public float Fuel { get; private set; }
    public float FuelMax { get; private set; }
    public float FuelConsumption { get; private set; } = 0.0f;
    public int Speed { get; private set; }
    public int ShipItemSize = 0;
    public int ShipItemSizeMax { get; private set; }
    public float Time { get; private set; } = 0;
    public int Currency { get; private set; } = 500;

    public void Start() {
        if(Instance == null) {
            Instance = this;
        }
        if(this != Instance) {
            Destroy(this.gameObject);
        }

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
        //Initialize the stats of the ship based on the assigned parts.
        foreach(ShipItem shipItem in ShipItems) {
            ShipItemSizeMax += shipItem.size;
            FuelMax += shipItem.fuelCapacityBonus;
            FuelConsumption += shipItem.fuelConsumptionRate;
            Speed += shipItem.speedBonus;
        }
        ShipItemSize = ShipItemSizeMax;
        Fuel = FuelMax;
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            
        }
    }

    public void MoveShip(Planet p) {
        Vector2 pos = transform.position;
        Vector2 dir = p.Position - pos;
        float dis = Vector2.Distance(pos, p.Position);
        float timeToTravel = dis / Speed;
        float fuelNeeded = FuelConsumption * timeToTravel;
        if(Fuel >= fuelNeeded) {
            Fuel -= fuelNeeded;
            Time += timeToTravel;
            transform.position = new Vector3(p.Position.x, p.Position.y, transform.position.z);
        }
        else {
            float percentDistanceAchieved = Fuel / fuelNeeded;
            Fuel = 0;
            Time += timeToTravel * percentDistanceAchieved;
            transform.position += new Vector3(dir.x, dir.y, 0) * percentDistanceAchieved;
        }
    }

    public bool IsGameOver() {
        return Fuel > 0.0f;
    }

    public void AddCurrency(int amount) {
        Currency += amount;
    }

    //Return true if the transaction completed, false otherwise.
    public bool RemoveCurrency(int amount) {
        if(Currency >= amount) {
            Currency -= amount;
            return true;
        }
        return false;
    }

}
