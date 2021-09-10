using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class BuyItem{
    public int ID = 0;
    public int rare = 0;
    public string name = "test";
    public int cost = 100;
    public string description = "dddd";
    public Sprite sprite = null;
}
public class BuyApp : BasicApp
{
    public List<BuyItem> RawBuyItemList = new List<BuyItem>(); 
    public Text Price = null;
    public Text ItemText = null;
    public Text CountDown = null;
    public Image ItemImage = null;

    float itemCountDown = 0;
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.Instance.EventAddListener(EventCenterType.SpawnItemByID, SpawnItemByID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnItemByID(params object[] data){
        foreach (BuyItem item in RawBuyItemList)
        {
            if(item.ID == (int)data[0]){
                Price.text = item.cost.ToString();
                ItemText.text = item.name;
                return;
            }
        }
    }
}
