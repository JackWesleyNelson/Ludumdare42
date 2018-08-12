using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private List<Button> shipPartsButtons;
    [SerializeField]
    private List<Button> shipCargoButtons;
    [SerializeField]
    private List<Button> planetSellingButtons;
    [SerializeField]
    private List<Button> planetBuyingButtons;
    
    private Planet p;


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

        p = planetGen.GetPlanet(ship.PlanetIndex);
        creditsText.text = "Credits: " + ship.Credits;
        
        for(int i = 0; i < 5; i++) {
            planetBuyingButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            planetSellingButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            shipPartsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            shipCargoButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        shipPartsText.text = "Ship Parts " + "(" + ship.ShipItems.Count + "/" + ship.ShipItemSizeMax + ")" + ":";
        for (int i = 0; i < ship.ShipItems.Count; i++) {
            shipPartsButtons[i].gameObject.SetActive(true);
            shipPartsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ship.ShipItems[i].partName;
        }

        shipCargoText.text = "Ship Cargo " + "(" + ship.InventoryItems.Count + "/" + ship.InventoryItemSizeMax + ")" + ":";
        for (int i = 0; i < ship.InventoryItems.Count; i++) {
            KeyValuePair<InventoryItem, ItemType> kvp = ship.InventoryItems[i];
            shipCargoButtons[i].gameObject.SetActive(true);
            shipCargoButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = kvp.Key.itemName + "\n(" + kvp.Value + ", " + (int)kvp.Key.quality + ")\n";
        }

        planetBuyingText.text = "Buy Cargo" + "(" + p.InventoryItemsPurchasable.Count + " / " + 5 + ")" + ":";
        for (int i = 0; i < p.InventoryItemsPurchasable.Count; i++) {
            KeyValuePair<InventoryItem, ItemType> kvp = p.InventoryItemsPurchasable[i];
            planetBuyingButtons[i].gameObject.SetActive(true);
            planetBuyingButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = kvp.Key.itemName + "\n(" + kvp.Value + ", " + (int)kvp.Key.quality + ")\n";
        }

        planetSellingText.text = "Sell Cargo" + "(" + p.InventoryItemsSellable.Count + " / " + 5 + ")" + ":";
        for (int i = 0; i < p.InventoryItemsSellable.Count; i++) {
            KeyValuePair<InventoryItem, ItemType> kvp = p.InventoryItemsSellable[i];
            planetSellingButtons[i].gameObject.SetActive(true);
            planetSellingButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = kvp.Key.itemName + "\n(" + kvp.Value + ", " + (int)kvp.Key.quality + ")\n";
        }
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

    public void BuyItem(int index) {
        if(index < p.InventoryItemsPurchasable.Count) {
            KeyValuePair<InventoryItem, ItemType> item = p.InventoryItemsPurchasable[index];
            //if (ship.RemoveCredits(item))
        }
    }



}
