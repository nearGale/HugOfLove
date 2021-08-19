using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public ItemDisplayBox itemDisplayBox = null;
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
                itemDisplayBox.ShowRawItem(item);
                return;
            }
        }
    }
}
