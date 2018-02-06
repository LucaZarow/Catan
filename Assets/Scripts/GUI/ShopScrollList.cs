using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item {
    public string itemName;
    public Sprite icon;
    public float price = 1f;
}

public class ShopScrollList : MonoBehaviour {

    public List<Item> itemList;
    public Transform contentPanel;
    public ShopScrollList otherShop;
    public Text myGoldDisplay;
    public SimpleObjectPool buttonObjectPool;
    public float gold = 20f;

	// Use this for initialization
	void Start () {
        RefreshDisplay();
	}

    private void RefreshDisplay() {
        AddButtons();
    }

    private void AddButtons() {

        for (int i = 0; i < itemList.Count; i++) {
            Item item = itemList[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            SampleScriptButton sampleButton = newButton.GetComponent<SampleScriptButton>();
            sampleButton.Setup(item, this);
        }
    }
}
