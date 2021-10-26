using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopItem{
    public int ID;
    public int Price;
    public string Description;
    public Sprite Icon;

    public ShopItem(int id = 0 , int price = 0 , string description = "test" , Sprite icon = null){
        ID = id;
        Price = price;
        Description = description;
        Icon = icon;
    }
}


public class GameSetting : Single<GameSetting>
{
    public List<ShopItem> shopItems;
    public ShopItem GetShopItemByID(int ID){
        foreach (var item in shopItems)
        {
            if(item.ID == ID)   return item;
        }
        return new ShopItem();
    }
}
