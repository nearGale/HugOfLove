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
        new ShopItem(1 , 100 , "描述1描述1描述1描述1描述1描述1描述1描述1" , null),
        new ShopItem(2 , 100 , "描述2描述2描述2描述2描述2描述2描述2描述2" , null),
        new ShopItem(3 , 100 , "描述3描述3描述3描述3描述3描述3描述3描述3" , null),
        new ShopItem(4 , 100 , "描述4描述4描述4描述4描述4描述4描述4描述4" , null),
        new ShopItem(5 , 100 , "描述5描述5描述5描述5描述5描述5描述5描述5" , null),
        new ShopItem(6 , 100 , "描述6描述6描述6描述6描述6描述6描述6描述6" , null),
        new ShopItem(7 , 100 , "描述7描述7描述7描述7描述7描述7描述7描述7" , null),

    };
    public ShopItem GetShopItemByID(int ID){
        foreach (var item in shopItems)
        {
            if(item.ID == ID)   return item;
        }
        return new ShopItem();
    }
}
