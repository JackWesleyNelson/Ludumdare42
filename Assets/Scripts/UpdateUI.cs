using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour {
    public UpdateUI Instance { get; private set; }

    public bool Paused { get; private set; } = true;

    [SerializeField]
    Ship ship;
    [SerializeField]
    private PlanetGenerator planetGen;
    [SerializeField]
    private Transform loadingScreen;
    [SerializeField]
    private Transform gameOverScreen;
    [SerializeField]
    private Transform startScreen;
    [SerializeField]
    private TextMeshProUGUI startScreenButtonText;
    [SerializeField]
    private TextMeshProUGUI gameOverScoreText;
    [SerializeField]
    private TextMeshProUGUI creditsText;
    [SerializeField]
    private TextMeshProUGUI fuelText;
    [SerializeField]
    private TextMeshProUGUI consoleText;
    [SerializeField]
    private TextMeshProUGUI loadingText;
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

    private float sellMult = 2.0f;

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
        if (Input.GetKeyDown(KeyCode.Escape) && startScreenButtonText.text == "Resume") {
            startScreen.gameObject.SetActive(!startScreen.gameObject.activeSelf);
            Paused = !Paused;
        }

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

        if(ship.Fuel <= 0) {
            gameOverScoreText.text = creditsText.text;
            gameOverScreen.gameObject.SetActive(true);
        }

        p = planetGen.GetPlanet(ship.PlanetIndex);
        creditsText.text = "Credits: " + ship.Credits;
        fuelText.text = "Fuel: " + "(" + ship.Fuel.ToString("0.00") + "/" + ship.FuelMax + ")";

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
            planetSellingButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = kvp.Key.itemName + "\n(" + kvp.Value + ", " + (int)((int)kvp.Key.quality * sellMult) + ")\n";
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
            int credits = (int)item.Key.quality;
            if (ship.RemoveCredits(credits)) {
                if (ship.AddInventoryItem(item)) {
                    consoleText.text = "You purchased (" + item.Key.itemName + ") for " + credits + "!";
                    p.RemoveAndReplaceItem(index, p.InventoryItemsPurchasable);
                }
                else {
                    ship.AddCredits(credits);
                    consoleText.text = "Could not purchase, not enough space in cargo!";
                }
            }
            else {
                consoleText.text = "Could not purchase, not enough credits!";
            }
        }
    }

    public void SellItem(int index) {
        if(index < p.InventoryItemsSellable.Count) {
            KeyValuePair<InventoryItem, ItemType> item = p.InventoryItemsSellable[index];
            int credits = (int)((int)item.Key.quality * sellMult);
            if (ship.RemoveInventoryItem(item)) {
                ship.AddCredits(credits);
                consoleText.text = "You purchased (" + item.Key.itemName + ") for " + credits + "!";
                p.RemoveAndReplaceItem(index, p.InventoryItemsSellable);
            }
            else {
                consoleText.text = "Could not sell item, item not in cargo!";
            }
        }
    }

    public void MoveToPlanet(bool planetReached) {
        if (!planetReached) {
            consoleText.text = "You don't have enough fuel to reach that planet!";
        }
        else {
            consoleText.text = "You've arrived at a new planet!";
        }
    }

    public void StartGame() {
        startScreen.gameObject.SetActive(false);
        startScreenButtonText.text = "Resume";
        Paused = !Paused;
    }

}
