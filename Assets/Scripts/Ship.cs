using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
    public Ship Instance { get; private set; }

    public int PlanetIndex { get; private set; } = 0;
    private int newPlanetIndex = 0;

    private Vector3 orbitCenter;
    private float orbitRadius = .5f;
    private float orbitSpeed = 0.5f;

    private bool isMoving = false;

    public List<ShipItem> ShipItems { get; private set; }
    public List<ShipItem> ShipItemsCatalogue { get; private set; }
    public List<KeyValuePair<InventoryItem, ItemType>> InventoryItems { get; private set; }

    [SerializeField]
    ShipItemList ShipItemsCatalogueSerializable;
    [SerializeField]
    LayerMask planetLayerMask;
    public int InventoryItemSize { get; private set; } = 0;
    public int InventoryItemSizeMax { get; private set; } = 4;
    public float Fuel { get; private set; }
    public float FuelMax { get; private set; }
    public float FuelConsumption { get; private set; } = 0.0f;
    public int Speed { get; private set; }
    public int ShipItemSize = 0;
    public int ShipItemSizeMax { get; private set; }
    public float GameTime { get; private set; } = 0;
    public int Credits { get; private set; } = 500;    

    public void Start() {
        if(Instance == null) {
            Instance = this;
        }
        if(this != Instance) {
            Destroy(this.gameObject);
        }
        orbitCenter = new Vector3(0, 0, 0);

        ShipItems = new List<ShipItem>();
        ShipItemsCatalogue = new List<ShipItem>(ShipItemsCatalogueSerializable.shipItemList);
        InventoryItems = new List<KeyValuePair<InventoryItem, ItemType>>();
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
        transform.position += new Vector3(orbitRadius, 0, 0);
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0) && !isMoving) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, planetLayerMask);
            if(hit.collider != null) {
                isMoving = true;
                orbitCenter = hit.transform.position;
                newPlanetIndex = int.Parse(hit.collider.gameObject.name);
            }
        }
    }

    public void FixedUpdate() {
        
        if (!isMoving) {
            transform.RotateAround(orbitCenter, Vector3.forward, orbitSpeed);
            PlanetIndex = newPlanetIndex;
        }
        else {
            if (Vector3.Distance(transform.position, orbitCenter) < orbitRadius + .001) {
                isMoving = false;
                Quaternion rot = transform.rotation;
                rot.z = 0;
                transform.rotation = rot;
            }
            transform.position = Vector3.MoveTowards(transform.position, orbitCenter + new Vector3(orbitRadius, 0, 0), orbitSpeed *2 * Time.deltaTime);
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
            GameTime += timeToTravel;
            transform.position = new Vector3(p.Position.x, p.Position.y, transform.position.z);
        }
        else {
            float percentDistanceAchieved = Fuel / fuelNeeded;
            Fuel = 0;
            GameTime += timeToTravel * percentDistanceAchieved;
            transform.position += new Vector3(dir.x, dir.y, 0) * percentDistanceAchieved;
        }
    }

    public bool IsGameOver() {
        return Fuel > 0.0f;
    }

    public void AddCurrency(int amount) {
        Credits += amount;
    }

    //Return true if the transaction completed, false otherwise.
    public bool RemoveCurrency(int amount) {
        if(Credits >= amount) {
            Credits -= amount;
            return true;
        }
        return false;
    }

}
