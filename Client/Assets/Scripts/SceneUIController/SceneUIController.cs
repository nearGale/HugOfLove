using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneUIController : MonoBehaviour
{
    public Image ImageActorRightBack;
    public Image ImageActorRightLeft;

    public Text TextReturnTimeLeft;
    public Text TextReturnTimeRight;

    public Button BtnBuyLeft;
    public Button BtnBuyRight;

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

    public Image ImageActorLeftReturn;
    public Image ImageActorRightReturn;
    
    void Start()
    {
        BtnBuyLeft.onClick.AddListener(OnBtnClickBuyLeft);
        BtnBuyRight.onClick.AddListener(OnBtnClickBuyRight);
        BtnReturnLeft.onClick.AddListener(OnBtnClickReturnLeft);
        BtnReturnRight.onClick.AddListener(OnBtnClickReturnRight);
    }

    void Update()
    {
        
    }

    void OnBtnClickBuyLeft()
    {
    }

    void OnBtnClickBuyRight()
    {
    }

    void OnBtnClickReturnLeft()
    {
    }

    void OnBtnClickReturnRight()
    {
    }
}
