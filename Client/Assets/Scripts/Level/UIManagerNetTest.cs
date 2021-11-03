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
    public Text TextLookBackTime;
    public Button ButtonBuy;
    public Button ButtonShutScreen;
    public Image ImageShutMask;
    public Image ImageBlockCountDown;
    public Animator ScreenTransSuccess;
    public Animator ScreenTransFail;
    public Animator ScreenTransAlert;
    public Animator ShutAlert;
    public Animator CountDown;

    public Animator InfoMe;
    public Text Info;
    public GameObject InfoRoot;
}

[System.Serializable]
public class PhoneInGameEnemy{
    public RectTransform RootTransform;
    public Animator NowScreenState;
    public Text Money;
    public Text AttackTime;
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
    public List<GameObject> OnMatchHuds;
    
    public AudioSource audioSource;
    public AudioClip AudioCheer;
    public AudioClip AudioFail;
    public AudioClip AudioSpawn;
    public AudioClip AudioAuction;
    public AudioClip AudioError;
    float LastTickTime = 0;
    public PhoneInGame PlayerPhone = new PhoneInGame();
    public PhoneInGame EnemyPhone = new PhoneInGame();
    public Animator EnemyPhoneAnimator;

    public Animator AniMe;
    public Animator AniEne;
    public Animator AniBoth;

    public Text TextEnemyName;
    public Text TextMyName;
    public Text IsMeAttacking;
    public Animator GlobalMask;
    //public Animator EnemyCharacter;
    //public Animator SelfCharacter;

   // public Text PagePlayerName;
   // public Text PagePlayerMoney;
   // public Text PagePlayerMail;

    Animator EnemyMoneyBeat;
    public Text TextEnemyMoneyBeat;
    public Text TextEnemyMoney;
    public Text TextCountDown;
    public Animator AniCountDown;
    Animator MyMoneyBeat;
    public Text TextMyMoneyBeat;
    public Text TextMyMoney;

    Vector3 PlayerPhoneStartPos;
    Vector3 PlayerPhoneStartRotation;
    float PhoneWingleSpeed = 0.8f;
    float BlockCountDownStartTime;
    float BlockCountDownDuration = 1.5f;
    public float ShopCountDownTime;
    bool isBlockCountDown;

    public List<RectTransform> BasicBeatList = new List<RectTransform>();
    
    public void BuyItemSuccess(bool isSuccess , bool isAuction = false){
        if(isSuccess){
            PlayerPhone.ButtonBuy.interactable = false;
            PlayerPhone.ScreenTransSuccess.SetTrigger("Trans");
            audioSource.PlayOneShot(AudioCheer);
        }else{
            PlayerPhone.ButtonBuy.interactable = false;
            PlayerPhone.ScreenTransFail.SetTrigger("Trans");
            if(!isAuction) {
                audioSource.PlayOneShot(AudioFail);
            }else{
                audioSource.PlayOneShot(AudioAuction);
            }
        }

        PlayerPhone.TextItemDesc.text = "更多商品，即将到来！";
        PlayerPhone.TextItemPrice.text = "$？？？";

    }
    public void ReSpawnItem(){
        //EnemyPhone.TextItemDesc.text = GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Description;
        //EnemyPhone.TextItemPrice.text = "$" +GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Price.ToString();
        PlayerPhone.TextItemDesc.text = GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Description;
        PlayerPhone.TextItemPrice.text = "$" +GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Price.ToString();
        PlayerPhone.ImageItemPic.sprite = GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Icon;
        PlayerPhone.ScreenTransAlert.SetTrigger("Trans");
        audioSource.PlayOneShot(AudioSpawn);
        StartShopCountDown();
        
    }
    public void OnPlayError(params object[] data){
        audioSource.PlayOneShot(AudioError);
    }

    public void StartShopCountDown(){
        ShopCountDownTime = 5f;
        AniCountDown.SetBool("isCounting" , true);
    }

    public void SetPlayerInfo(bool isSelf , int deltaMoney){
        if(deltaMoney ==0)  return;
        string d = deltaMoney >= 0 ?"+":"-" + deltaMoney.ToString();
        if(isSelf){
            MyMoneyBeat.SetTrigger("Beat");
            TextMyMoneyBeat.text = d;
            PlayerPhone.TextNowMoney.text = "$" + LevelManagerNetTest.Instance.MyPlayerInfo.Money.ToString();
        }else{
            EnemyMoneyBeat.SetTrigger("Beat");
            TextEnemyMoneyBeat.text = d;
            //PagePlayerMoney.text = "她好像还剩……" + LevelManagerNetTest.Instance.EnemyPlayerInfo.Money.ToString();
        }
    }

    public void OnMatchSuccess(){

        EnemyPhoneAnimator.SetBool("Light" , true);

       // PagePlayerMail.text = "她的邮箱是……\n" + LevelManagerNetTest.Instance.EnemyPlayerMail.ToString();
       // PagePlayerName.text = "她好像叫……" + LevelManagerNetTest.Instance.EnemyPlayerName.ToString();
//
        AniBoth.SetBool("active" , true);
        AniMe.SetBool("active" , false);
        AniEne.SetBool("active" , false);

    


//        PagePlayerMoney.text = "她好像还剩……" + LevelManagerNetTest.Instance.EnemyPlayerInfo.Money.ToString();
    }

    public void ShopCountDownTick(){
        
        if(ShopCountDownTime >=0){
            ShopCountDownTime -= Time.deltaTime;
            TextCountDown.text = string.Format("{0:00}" , Mathf.Ceil(ShopCountDownTime));
            if(ShopCountDownTime < 0 ){
                //PlayerPhone.ButtonBuy.interactable = true;
                AniCountDown.SetBool("isCounting" , false);
                TextCountDown.text = "Now";
            }
        }  
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartBlockCountDown();
        EnemyMoneyBeat = TextEnemyMoney.GetComponent<Animator>();
        MyMoneyBeat = TextMyMoney.GetComponent<Animator>();

        PlayerPhoneStartPos = PlayerPhone.RootTransform.position;
        PlayerPhoneStartRotation  = PlayerPhone.RootTransform.rotation.eulerAngles;

        LastTickTime = Time.timeSinceLevelLoad;

        audioSource.PlayOneShot(AudioCheer);

        EventCenter.Instance.EventAddListener(EventCenterType.PlayerErrorAudio , OnPlayError);
    }

    IEnumerator TickTick(int frame){
        for (float i = 0; i < frame; i++)
        {
            float k = 1.07f;
            float z;
            if( i < frame /2){
                z = 1 + (k - 1) * ( i / (frame / 2));
            }else{
                z = k - (k - 1) * ( (i - frame / 2) / (frame / 2));
            }
            //Debug.Log(z);
            PlayerPhone.RootTransform.localScale = new Vector3( z ,  z , 1 );
            EnemyPhone.RootTransform.localScale = new Vector3( z ,  z , 1 );
            foreach (var item in BasicBeatList)
            {
                item.localScale = new Vector3( z ,  z , 1 );
            }
            yield return null;//表示下一帧再继续执行后面的代码
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeSinceLevelLoad - LastTickTime >= .5){
            LastTickTime = Time.timeSinceLevelLoad;
            StartCoroutine(TickTick(40));
        }
        
        PlayerPhone.ImageBlockCountDown.gameObject.SetActive(isBlockCountDown);
        //if(isBlockCountDown){
        //    var a = .1f;
        //    a = (Time.timeSinceLevelLoad - BlockCountDownStartTime) > BlockCountDownDuration ? 0 : 1 - ( (Time.timeSinceLevelLoad - BlockCountDownStartTime) / BlockCountDownDuration);
        //    PlayerPhone.ImageBlockCountDown.rectTransform.localScale = new Vector3(a , a , a);
        //    if(a==0) isBlockCountDown = false;
        //}

        //PlayerPhone.RootTransform.position = new Vector3(PlayerPhoneStartPos.x + PhoneWingleSpeed * Mathf.Sin(0.8f * Time.timeSinceLevelLoad) , PlayerPhoneStartPos.y - PhoneWingleSpeed * Mathf.Sin(0.42f * Time.timeSinceLevelLoad) , 0);
        PlayerPhone.RootTransform.rotation = Quaternion.Euler(PlayerPhoneStartRotation.x , PlayerPhoneStartRotation.y , PlayerPhoneStartRotation.z + 5.4f* Mathf.Sin(0.9f * Time.timeSinceLevelLoad + 1.2F));
    
        EnemyPhone.RootTransform.rotation = Quaternion.Euler(PlayerPhoneStartRotation.x , PlayerPhoneStartRotation.y , PlayerPhoneStartRotation.z + 5.4f* Mathf.Sin(0.72f * Time.timeSinceLevelLoad));
    

        IsMeAttacking.gameObject.SetActive( LevelManagerNetTest.Instance.IsAttacking );

        foreach (var item in OnMatchHuds)
        {
            item.SetActive(LevelManagerNetTest.Instance.nowState == LevelMatchState.Matching);
        }

        ShopCountDownTick();
    }

    public void ShowKiss(){
        GlobalMask.SetTrigger("Kiss");
        //EnemyCharacter.SetTrigger("AttackSuccess");
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
            //SelfCharacter.SetTrigger("BeAttacked");

            AniBoth.SetBool("active" , false);
            AniMe.SetBool("active" , true);
            AniEne.SetBool("active" , false);
        }else{
            //EnemyCharacter.SetBool("IsAttacking" , true);

            AniBoth.SetBool("active" , false);
            AniMe.SetBool("active" , false);
            AniEne.SetBool("active" , true);
        }
    }
}
