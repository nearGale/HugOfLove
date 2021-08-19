using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public Sprite Icon;
    public string Description;
    public int Price;
}


public class ShopItemController : MonoSingleton<ShopItemController>
{
    [SerializeField]
    private List<ShopItem> m_ShopItems;
    [SerializeField]
    private float m_Duration;

    private int m_CurrentIndex;
    private float m_LeftTime;
    private bool m_Bought;

    void Start()
    {
        m_CurrentIndex = -1;
        PickNextItem();
    }

    void Update()
    {
        m_LeftTime -= Time.deltaTime;
        if(m_LeftTime <= -2f)
        {
            PickNextItem();
        }
    }

    void PickNextItem()
    {
        m_CurrentIndex++;
        if (m_CurrentIndex >= m_ShopItems.Count)
        {
            m_CurrentIndex = 0;
        }

        m_LeftTime = m_Duration;
        EventCenter.Instance.EventTrigger(EventCenterType.RefreshItem);

        m_Bought = false;
    }

    public bool TryBuy()
    {
        if(m_LeftTime > 0)
        {
            return false;
        }

        if (m_Bought)
        {
            return false;
        }

        m_Bought = true;
        return true;
    }

    #region Get Func
    public float GetCurrentShopItemLeftTime()
    {
        if(m_LeftTime >= 0)
        {
            return m_LeftTime;
        }
        return 0;
    }

    public Sprite GetCurrentShopItemSprite()
    {
        ShopItem shopItem = GetCurrentShopItem();
        if(shopItem == null)
        {
            return null;
        }

        return shopItem.Icon;
    }

    public int GetCurrentShopItemPrice()
    {
        ShopItem shopItem = GetCurrentShopItem();
        if (shopItem == null)
        {
            return 0;
        }

        return shopItem.Price;
    }

    public string GetCurrentShopItemDesc()
    {
        ShopItem shopItem = GetCurrentShopItem();
        if (shopItem == null)
        {
            return string.Empty;
        }

        return shopItem.Description;
    }

    private ShopItem GetCurrentShopItem()
    {
        if (m_CurrentIndex >= m_ShopItems.Count)
        {
            return null;
        }

        return m_ShopItems[m_CurrentIndex];
    }
    #endregion
}
