using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopItem{
    public int ID;
    public int Price;
    public string Description;
    public Sprite Icon;

    public ShopItem(int id = 0 , int price = 0 , string Description = "test" , Sprite Icon = null){

    }
}


public class GameSetting : Single<GameSetting>
{
    ShopItem[] shopItems = {
        new ShopItem()
    };
    public ShopItem GetShopItemByID(int ID){
        return new ShopItem();
    }
}
