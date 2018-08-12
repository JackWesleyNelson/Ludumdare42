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
    [SerializeField]
    UpdateUI updateUI;
    [SerializeField]
    PlanetGenerator planetGenerator;

    public int InventoryItemSize { get; private set; } = 0;
    public int InventoryItemSizeMax { get; private set; } = 5;
    public float Fuel { get; private set; }
    public float FuelMax { get; private set; }
    public float FuelConsumption { get; private set; } = 0.0f;
    public int Speed { get; private set; }
    public int ShipItemSize = 0;
    public int ShipItemSizeMax { get; private set; } = 5;
    public float GameTime { get; private set; } = 0;
    public int Credits { get; private set; } = 800;    

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
            ShipItemSize += shipItem.size;
            FuelMax += shipItem.fuelCapacityBonus;
            FuelConsumption += shipItem.fuelConsumptionRate;
            Speed += shipItem.speedBonus;
        }
        Fuel = FuelMax;
        transform.position += new Vector3(orbitRadius, 0, 0);
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0) && !isMoving) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, planetLayerMask);
            if(hit.collider != null) {
                if(newPlanetIndex != int.Parse(hit.collider.gameObject.name)) {
                    newPlanetIndex = int.Parse(hit.collider.gameObject.name);
                    Planet p = planetGenerator.GetPlanet(newPlanetIndex);
                    if (MoveShip(p)) {
                        p.SetVisited();
                        isMoving = true;
                        orbitCenter = hit.transform.position;
                    }
                    else {
                        updateUI.MoveToPlanet(false);
                    }
                }
            }
        }
        if (!planetGenerator.IsSpawning && !updateUI.Paused) {
            if (isMoving) {
                Fuel -= Time.deltaTime / 4;
            }
            else {
                Fuel -= Time.deltaTime / 12;
            }
            if(Fuel < 0) {
                Fuel = 0;
            }
        }
    }

    public void FixedUpdate() {
        
        if (!isMoving) {
            transform.right = (orbitCenter - transform.position) * -1;
            transform.RotateAround(orbitCenter, Vector3.forward, orbitSpeed);
            PlanetIndex = newPlanetIndex;
        }
        else {
            if (Vector3.Distance(transform.position, orbitCenter) < orbitRadius + .001) {
                isMoving = false;
                updateUI.MoveToPlanet(true);
            }
            transform.position = Vector3.MoveTowards(transform.position, orbitCenter + new Vector3(orbitRadius, 0, 0), orbitSpeed *2 * Time.deltaTime);
            transform.up = orbitCenter - transform.position;
        }
    }

    public bool MoveShip(Planet p) {
        Vector2 pos = orbitCenter;
        Vector2 dir = p.Position - pos;
        float dis = Vector2.Distance(pos, p.Position);
        float timeToTravel = dis / Speed;
        float fuelNeeded = FuelConsumption * timeToTravel;
        if(Fuel >= fuelNeeded) {
            Fuel -= fuelNeeded;
            GameTime += timeToTravel;
            return true;
        }
        else {
            return false;
        }
    }

    public bool IsGameOver() {
        return Fuel > 0.0f;
    }

    public void AddCredits(int amount) {
        Credits += amount;
    }

    //Return true if the transaction completed, false otherwise.
    public bool RemoveCredits(int amount) {
        if(Credits >= amount) {
            Credits -= amount;
            return true;
        }
        return false;
    }

    public bool AddInventoryItem(KeyValuePair<InventoryItem, ItemType> item) {
        if (InventoryItems.Count < InventoryItemSizeMax) {
            InventoryItems.Add(item);
            return true;
        }
        return false;
    }

    public bool RemoveInventoryItem(KeyValuePair<InventoryItem, ItemType> item) {
        if (InventoryItems.Contains(item) ) {
            InventoryItems.Remove(item);
            return true;
        }
        return false;
    }



}
