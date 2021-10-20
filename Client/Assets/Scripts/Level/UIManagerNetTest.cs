using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectNetWork;

[System.Serializable]
public class PhoneInGame{
    public RectTransform RootTransform;
    public Image ImageItemPic;
    public Text TextItemPrice;
    public Text TextItemDesc;
    public Text TextNowMoney;
    public Text TextBuyCountDown;
    public Button ButtonBuy;
    public Button ButtonShutScreen;
    public Image ImageShutMask;
    public Image ImageBlockCountDown;
    public Animator ScreenTransSuccess;
    public Animator ScreenTransFail;
    public Animator ScreenTransAlert;
    public Animator ShutAlert;
    public Animator CountDown;
}

[System.Serializable]
public struct AttackButtonInGame{
    public Button ButtonAttack;
    public Text TextAttackTime;
}
[System.Serializable]
public struct DebugInGame{
    public Text TextNowStat;
}

public class UIManagerNetTest : MonoBehaviour
{
    public PhoneInGame PlayerPhone = new PhoneInGame();
    public PhoneInGame EnemyPhone = new PhoneInGame();
    public AttackButtonInGame PlayerAttackButton;
    public AttackButtonInGame EnemyAttackButton;
    public DebugInGame debugInGame;
    public Image EnemyPic;
    public Image MyPic;
    public Text TextEnemyName;
    public Text TextMyName;
    public Light LightSelf;
    public Light LightEnemy;

    public Animator GlobalMask;
    public Animator EnemyCharacter;
    public Animator SelfCharacter;
    float LightSelfStartIntersity;
    float LightEnemyStartIntersity;

    public Text PagePlayerName;
    public Text PagePlayerMoney;
    public Text PagePlayerMail;
    public Animator PageAnimator;

    Animator EnemyMoneyBeat;
    public Text TextEnemyMoneyBeat;
    Animator MyMoneyBeat;
    public Text TextMyMoneyBeat;
    Animator ItemAnimator; 

    Vector3 PlayerPhoneStartPos;
    Vector3 PlayerPhoneStartRotation;
    float PhoneWingleSpeed = 0.8f;
    float BlockCountDownStartTime;
    float BlockCountDownDuration = 1.5f;
    float ShopCountDownTime;
    bool isBlockCountDown;
    
    public void StartBlockCountDown(){
        Debug.Log("start block");
        PlayerPhone.ShutAlert.SetBool("IsCounting" , true);
        isBlockCountDown = true;
        BlockCountDownStartTime = Time.timeSinceLevelLoad;
        
    }
    public void StopBlockCountDown(){
        PlayerPhone.ShutAlert.SetBool("IsCounting" , false);
        isBlockCountDown = false;
    }
    public void BuyItemSuccess(bool isSuccess){
        if(isSuccess){
            ItemAnimator.SetTrigger("BuySuccess");
            PlayerPhone.ButtonBuy.interactable = false;
            PlayerPhone.ScreenTransSuccess.SetTrigger("Trans");
        }else{
            ItemAnimator.SetTrigger("BuyFail");
            PlayerPhone.ButtonBuy.interactable = false;
            PlayerPhone.ScreenTransFail.SetTrigger("Trans");
        }

        PlayerPhone.TextItemDesc.text = "更多商品，即将到来！";
        PlayerPhone.TextItemPrice.text = "$？？？";
        PlayerPhone.TextBuyCountDown.text = "00:00";

    }
    public void ReSpawnItem(){
        EnemyPhone.TextItemDesc.text = GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Description;
        EnemyPhone.TextItemPrice.text = "$" +GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Price.ToString();
        PlayerPhone.TextItemDesc.text = GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Description;
        PlayerPhone.TextItemPrice.text = "$" +GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Price.ToString();

        ItemAnimator.SetTrigger("ReSpawn");
        PlayerPhone.ScreenTransAlert.SetTrigger("Trans");
        
        StartShopCountDown();
        
    }

    public void StartShopCountDown(){
        ShopCountDownTime = 5f;
        PlayerPhone.CountDown.SetBool("isCounting" , true);
    }

    public void SetPlayerInfo(bool isSelf , int deltaMoney){
        string d = deltaMoney >= 0 ?"+":"-" + deltaMoney.ToString();
        if(isSelf){
            MyMoneyBeat.SetTrigger("beat");
            TextMyMoneyBeat.text = d;
            PlayerPhone.TextNowMoney.text = "$" + LevelManagerNetTest.Instance.MyPlayerInfo.Money.ToString();
        }else{
            EnemyMoneyBeat.SetTrigger("beat");
            TextEnemyMoneyBeat.text = d;
            PagePlayerMoney.text = "她好像还剩……" + LevelManagerNetTest.Instance.EnemyPlayerInfo.Money.ToString();
        }
    }

    public void OnMatchSuccess(){
        PageAnimator.SetTrigger("MatchSuccess");
        PagePlayerMail.text = "她的邮箱是……\n" + LevelManagerNetTest.Instance.EnemyPlayerMail.ToString();
        PagePlayerName.text = "她好像叫……" + LevelManagerNetTest.Instance.EnemyPlayerName.ToString();
//        PagePlayerMoney.text = "她好像还剩……" + LevelManagerNetTest.Instance.EnemyPlayerInfo.Money.ToString();
    }

    public void ShopCountDownTick(){
        
        if(ShopCountDownTime >=0){
            ShopCountDownTime -= Time.deltaTime;
            PlayerPhone.TextBuyCountDown.text = string.Format("00:{0:00}" , Mathf.Ceil(ShopCountDownTime));
            if(ShopCountDownTime < 0 ){
                PlayerPhone.ButtonBuy.interactable = true;
                PlayerPhone.CountDown.SetBool("isCounting" , false);
                PlayerPhone.TextBuyCountDown.text = "现在抢购！";
            }
        }  
        if(LevelManagerNetTest.Instance.NowItemID <= 0) PlayerPhone.TextBuyCountDown.text = "敬请期待";
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartBlockCountDown();
        EnemyMoneyBeat = TextEnemyMoneyBeat.GetComponent<Animator>();
        MyMoneyBeat = TextMyMoneyBeat.GetComponent<Animator>();

        ItemAnimator = PlayerPhone.ImageItemPic.GetComponent<Animator>();

        PlayerPhoneStartPos = PlayerPhone.RootTransform.position;
        PlayerPhoneStartRotation  = PlayerPhone.RootTransform.rotation.eulerAngles;

        LightEnemyStartIntersity = LightEnemy.intensity;
        LightSelfStartIntersity = LightSelf.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPhone.ImageBlockCountDown.gameObject.SetActive(isBlockCountDown);
        //if(isBlockCountDown){
        //    var a = .1f;
        //    a = (Time.timeSinceLevelLoad - BlockCountDownStartTime) > BlockCountDownDuration ? 0 : 1 - ( (Time.timeSinceLevelLoad - BlockCountDownStartTime) / BlockCountDownDuration);
        //    PlayerPhone.ImageBlockCountDown.rectTransform.localScale = new Vector3(a , a , a);
        //    if(a==0) isBlockCountDown = false;
        //}

        PlayerPhone.RootTransform.position = new Vector3(PlayerPhoneStartPos.x + PhoneWingleSpeed * Mathf.Sin(0.8f * Time.timeSinceLevelLoad) , PlayerPhoneStartPos.y - PhoneWingleSpeed * Mathf.Sin(0.42f * Time.timeSinceLevelLoad) , 0);
        PlayerPhone.RootTransform.rotation = Quaternion.Euler(PlayerPhoneStartRotation.x , PlayerPhoneStartRotation.y , PlayerPhoneStartRotation.z + 5.4f* Mathf.Sin(0.45f * Time.timeSinceLevelLoad));
    
        LightEnemy.intensity = LightEnemyStartIntersity +  Mathf.Abs( 15 * Mathf.Sin(1.84f * Time.timeSinceLevelLoad) + 7 );
        LightSelf.intensity = LightSelfStartIntersity +  Mathf.Abs( 45 * Mathf.Sin(1.96f * Time.timeSinceLevelLoad + 2.53f) + 18 );

        ShopCountDownTick();
    }

    public void ShowKiss(){
        GlobalMask.SetTrigger("Kiss");
        EnemyCharacter.SetTrigger("AttackSuccess");
    }

    public void ShowBlock(){
        GlobalMask.SetTrigger("Block");
    }

    public void ShowAttackSuccess(){
        GlobalMask.SetTrigger("AttackSuccess");
    }

    public void ShowAttackFail(){
        GlobalMask.SetTrigger("AttackFail");
    }

    public void OnAttack(bool isSelf){
        if(isSelf){
            SelfCharacter.SetTrigger("BeAttacked");
        }else{
            EnemyCharacter.SetBool("IsAttacking" , true);
        }
    }
    public void OnCancelAttack(bool isSelf){
        if(isSelf){
            SelfCharacter.SetTrigger("AttackEnd");
            
        }else{
            EnemyCharacter.SetBool("IsAttacking" , false);
        }
    }

}
