using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UpdateUI : MonoBehaviour {
    public UpdateUI Instance { get; private set; }

    [SerializeField]
    Ship ship;
    [SerializeField]
    private TextMeshProUGUI creditsText;
    [SerializeField]
    private Transform loadingScreen;
    [SerializeField]
    private TextMeshProUGUI loadingText;
    [SerializeField]
    private PlanetGenerator planetGen;
    [SerializeField]
    private TextMeshProUGUI shipPartsText;
    [SerializeField]
    private TextMeshProUGUI shipCargoText;
    [SerializeField]
    private TextMeshProUGUI planetSellingText;
    [SerializeField]
    private TextMeshProUGUI planetBuyingText;


    private Coroutine loadingCo;

	// Use this for initialization
	void Start () {
		if(Instance == null) {
            Instance = this;
        }
        if(this != Instance) {
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (planetGen.IsSpawning) {
            if(loadingCo == null) {
                loadingCo = StartCoroutine(LoadingTextAnimation());
            }
            loadingScreen.gameObject.SetActive(true);
        }
        else {
            loadingScreen.gameObject.SetActive(false);
            StopCoroutine(loadingCo);
        }
        creditsText.text = "Credits: " + ship.Credits;

        string partsText = "Ship Parts " + "(" + ship.ShipItems.Count + "/" + ship.ShipItemSizeMax + ")" + ":\n";
        foreach (ShipItem part in ship.ShipItems) {
            partsText += part.partName + "\n";
        }
        shipPartsText.text = partsText;

        string cargoText = "Ship Cargo " + "(" + ship.InventoryItems.Count + "/" + ship.InventoryItemSizeMax + ")" + ":\n";
        foreach (KeyValuePair<InventoryItem, ItemType> kvp in ship.InventoryItems) {
            cargoText += kvp.Key.itemName + "\n(" + kvp.Value + ", " + kvp.Key.quality + ")\n";
        }
        shipCargoText.text = cargoText;


        Planet p = planetGen.GetPlanet(ship.PlanetIndex);

        string buyText = "Buy Cargo" + "(" + p.InventoryItemsPurchasable.Count + " / " + 5 + ")" + ":\n";
        foreach (KeyValuePair<InventoryItem, ItemType> kvp in p.InventoryItemsPurchasable) {
            buyText += kvp.Key.itemName + "\n(" + kvp.Value + ", " + kvp.Key.quality + ")\n";
        }
        planetBuyingText.text = buyText;

        string sellText = "Sell Cargo" + "(" + p.InventoryItemsSellable.Count + " / " + 5 + ")" + ":\n"; ;
        foreach (KeyValuePair<InventoryItem, ItemType> kvp in p.InventoryItemsSellable) {
            sellText += kvp.Key.itemName + "\n(" + kvp.Value + ", " + kvp.Key.quality + ")\n";
        }
        planetSellingText.text = sellText;
    }

    IEnumerator LoadingTextAnimation() {
        while (true) {
            string s = "Loading ";
            for(int i = 0; i <= 3; i++) {
                string dots = string.Concat(Enumerable.Repeat(".", i));
                loadingText.text = s + dots;
                yield return new WaitForSeconds(.5f);
            }
        }
    }



}
