using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneUIController : MonoBehaviour
{
    public Image ImageActorLeftBack;
    public Image ImageActorRightBack;

    public Text TextReturnTimeLeft;
    public Text TextReturnTimeRight;

    public Button BtnBuyLeft;
    public Button BtnBuyRight;

    public Button BtnHomeLeft;
    public Button BtnHomeRight;

    public Button BtnReturnLeft;
    public Button BtnReturnRight;

    public Text TextMoneyLeft;
    public Text TextMoneyRight;

    public Image ImageItemLeft;
    public Image ImageItemRight;

    public Text TextItemLeft;
    public Text TextItemRight;

    public Text TextItemPriceLeft;
    public Text TextItemPriceRight;

    public Text TextItemCountDownLeft;
    public Text TextItemCountDownRight;

    public Image ImageMaskLeft;
    public Image ImageMaskRight;

    public Image ImageScreenLockLeft;
    public Image ImageScreenLockRight;

    public Image ImageActorLeftReturn;
    public Image ImageActorRightReturn;

    private bool m_IsReturning;
    private float m_ReturnDuration;
    private float m_ReturnLeftTime;
    private bool m_ReturnChecked;
    private PlayerId m_ReturnPlayerId;

    public float ScreenDelayToLock;

    void Start()
    {
        Screen.SetResolution(1920, 1080, false);

        BtnBuyLeft.onClick.AddListener(OnBtnClickBuyLeft);
        BtnBuyRight.onClick.AddListener(OnBtnClickBuyRight);
        BtnReturnLeft.onClick.AddListener(OnBtnClickReturnLeft);
        BtnReturnRight.onClick.AddListener(OnBtnClickReturnRight);
        BtnHomeLeft.onClick.AddListener(OnBtnClickHomeLeft);
        BtnHomeRight.onClick.AddListener(OnBtnClickHomeRight);

        ImageActorLeftReturn.gameObject.SetActive(false);
        ImageActorRightReturn.gameObject.SetActive(false);

        ImageMaskLeft.gameObject.SetActive(false);
        ImageMaskRight.gameObject.SetActive(false);

        ImageScreenLockLeft.gameObject.SetActive(false);
        ImageScreenLockRight.gameObject.SetActive(false);

        EventCenter.Instance.EventAddListener(EventCenterType.RefreshItem, RefreshShopItem);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnBtnClickBuyLeft();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnBtnClickReturnLeft();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnBtnClickHomeLeft();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            OnBtnClickBuyRight();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            OnBtnClickReturnRight();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnBtnClickHomeRight();
        }

        // refresh return duration
        TextReturnTimeLeft.text = $"{DataManager.Instance.GetReturnDuration(PlayerId.Left)}s";
        TextReturnTimeRight.text = $"{DataManager.Instance.GetReturnDuration(PlayerId.Right)}s";

        // refresh money
        TextMoneyLeft.text = $"{DataManager.Instance.GetPlayerMoney(PlayerId.Left)}s";
        TextMoneyRight.text = $"{DataManager.Instance.GetPlayerMoney(PlayerId.Right)}s";

        TextItemCountDownLeft.text = $"{ShopItemController.Instance.GetCurrentShopItemLeftTime().ToString("0.000")}s";
        TextItemCountDownRight.text = $"{ShopItemController.Instance.GetCurrentShopItemLeftTime().ToString("0.000")}s";

        if (m_IsReturning)
        {
            m_ReturnLeftTime -= Time.deltaTime;
            if(m_ReturnDuration - m_ReturnLeftTime > ScreenDelayToLock && !m_ReturnChecked)
            {
                m_ReturnChecked = true;
                if (m_ReturnPlayerId == PlayerId.Left)
                {
                    if (!ImageScreenLockRight.gameObject.activeInHierarchy)
                    {
                        ImageMaskRight.gameObject.SetActive(true);
                    }
                }
                else if (m_ReturnPlayerId == PlayerId.Right)
                {
                    if (!ImageScreenLockLeft.gameObject.activeInHierarchy)
                    {
                        ImageMaskLeft.gameObject.SetActive(true);
                    }
                }
            }
            if(m_ReturnLeftTime <= 0)
            {
                m_IsReturning = false;
                ResetReturnState();
            }
        }
    }

    private void OnBtnClickBuyLeft()
    {
        bool buySuccess = ShopItemController.Instance.TryBuy();
        if (buySuccess)
        {
            DataManager.Instance.DecreaseMoney(PlayerId.Left, ShopItemController.Instance.GetCurrentShopItemPrice());
            DataManager.Instance.IncreaseReturnDuration(PlayerId.Right, 1);
        }
    }

    private void OnBtnClickBuyRight()
    {
        bool buySuccess = ShopItemController.Instance.TryBuy();
        if (buySuccess)
        {
            DataManager.Instance.DecreaseMoney(PlayerId.Right, ShopItemController.Instance.GetCurrentShopItemPrice());
            DataManager.Instance.IncreaseReturnDuration(PlayerId.Left, 1);
        }
    }

    private void OnBtnClickReturnLeft()
    {
        if (m_IsReturning)
        {
            return;
        }

        float duration = DataManager.Instance.GetReturnDuration(PlayerId.Left);
        if (duration > 0)
        {
            ImageActorLeftBack.gameObject.SetActive(false);
            ImageActorLeftReturn.gameObject.SetActive(true);

            m_IsReturning = true;
            m_ReturnDuration = duration;
            m_ReturnLeftTime = duration;
            m_ReturnPlayerId = PlayerId.Left;
            m_ReturnChecked = false;

            DataManager.Instance.ClearReturnDuration(PlayerId.Left);
        }
    }

    private void OnBtnClickReturnRight()
    {
        if (m_IsReturning)
        {
            return;
        }

        float duration = DataManager.Instance.GetReturnDuration(PlayerId.Right);
        if (duration > 0)
        {
            ImageActorRightBack.gameObject.SetActive(false);
            ImageActorRightReturn.gameObject.SetActive(true);

            m_IsReturning = true;
            m_ReturnDuration = duration;
            m_ReturnLeftTime = duration;
            m_ReturnPlayerId = PlayerId.Right;
            m_ReturnChecked = false;

            DataManager.Instance.ClearReturnDuration(PlayerId.Right);
        }
    }

    private void OnBtnClickHomeLeft()
    {
        ImageScreenLockLeft.gameObject.SetActive(!ImageScreenLockLeft.gameObject.activeInHierarchy);
    }

    private void OnBtnClickHomeRight()
    {
        ImageScreenLockRight.gameObject.SetActive(!ImageScreenLockRight.gameObject.activeInHierarchy);
    }

    private void ResetReturnState()
    {
        ImageActorLeftBack.gameObject.SetActive(true);
        ImageActorLeftReturn.gameObject.SetActive(false);

        ImageActorRightBack.gameObject.SetActive(true);
        ImageActorRightReturn.gameObject.SetActive(false);
    }

    private void RefreshShopItem(params object[] data)
    {
        ImageItemLeft.sprite = ShopItemController.Instance.GetCurrentShopItemSprite();
        ImageItemRight.sprite = ShopItemController.Instance.GetCurrentShopItemSprite();

        TextItemLeft.text = ShopItemController.Instance.GetCurrentShopItemDesc();
        TextItemRight.text = ShopItemController.Instance.GetCurrentShopItemDesc();

        TextItemPriceLeft.text = ShopItemController.Instance.GetCurrentShopItemPrice().ToString();
        TextItemPriceRight.text = ShopItemController.Instance.GetCurrentShopItemPrice().ToString();

        TextItemCountDownLeft.text = $"{ShopItemController.Instance.GetCurrentShopItemLeftTime().ToString("0.000")}s";
        TextItemCountDownRight.text = $"{ShopItemController.Instance.GetCurrentShopItemLeftTime().ToString("0.000")}s";
    }
}
