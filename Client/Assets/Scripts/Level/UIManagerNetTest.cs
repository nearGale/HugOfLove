using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectNetWork;

[System.Serializable]
public class ProBar{
    public NewProgressBar bar1;
    public NewProgressBar bar2;
    public NewProgressBar bar3;
    public void SetProBar(float f1 , float f2 , float f3){
        //bar1.SetProgressValue(f1);
        bar2.SetProgressValue(f2);
        bar3.SetProgressValue(f3);
        //if(f1 >= .1f)   bar1.GetComponent<Animator>().SetBool("Blink" , false);
        Debug.Log(new Vector3(f1,f2,f3));
    }
}
[System.Serializable]
public class PhoneInGame{
    public RectTransform RootTransform;
    public Image ImageItemPic;
    public Text TextItemPrice;
    public Text TextItemDesc;
    public Text TextNowMoney;
    public Text TextLookBackTime;
    public Image ImageShutMask;
    public Animator ScreenTransSuccess;
    public Animator ScreenTransFail;
    public Animator ScreenTransAlert;

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
    public List<ParticleSystem> par;
    public ProBar BarLeft;
    public ProBar BarRight;
    public List<GameObject> OnMatchHuds;
    
    public AudioSource audioSource;
    public AudioClip AudioCheer;
    public AudioClip AudioFail;
    public AudioClip AudioSpawn;
    public AudioClip AudioAuction;
    public AudioClip AudioError;
    public AudioClip AudioErrorBig;
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
    public Animator MyMoneyBeat;
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
            PlayerPhone.ScreenTransSuccess.SetTrigger("Trans");
            if (audioSource != null && AudioCheer != null)
            {
                audioSource.PlayOneShot(AudioCheer);
            }
            foreach (var item in par)
            {
                item.Play();
                
            }
        }else{
            PlayerPhone.ScreenTransFail.SetTrigger("Trans");
            if(!isAuction)
            {
                if (audioSource != null && AudioFail != null)
                {
                    audioSource.PlayOneShot(AudioFail);
                }
            }else{
                if (audioSource != null && AudioAuction != null)
                {
                    audioSource.PlayOneShot(AudioAuction);
                }
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
        if (audioSource != null && AudioSpawn != null)
        {
            audioSource.PlayOneShot(AudioSpawn);
        }
        StartShopCountDown();

        if(LevelManagerNetTest.Instance.NowItemID > 0 ){
            float a = (LevelManagerNetTest.Instance.MyPlayerInfo.Money - GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Price) / 10000f;
            Debug.Log((LevelManagerNetTest.Instance.MyPlayerInfo.Money));
            Debug.Log(GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Price);
            Debug.Log(LevelManagerNetTest.Instance.NowItemID);
            float b = LevelManagerNetTest.Instance.MyPlayerInfo.Money / 10000f;
            BarRight.SetProBar(0.1f , a , b);

            a = (LevelManagerNetTest.Instance.EnemyPlayerInfo.Money - GameSetting.Instance.GetShopItemByID(LevelManagerNetTest.Instance.NowItemID).Price) / 10000f;
            b = LevelManagerNetTest.Instance.EnemyPlayerInfo.Money / 10000f;
            BarLeft.SetProBar(0.1f , a , b);
        }
        
    }
    public void OnPlayError(params object[] data){
        if (audioSource != null && AudioError != null)
        {
            audioSource.PlayOneShot(AudioError);
        }
        else
        {
            Debug.LogError("audioSource != null :" + (audioSource != null) + "  " + "AudioError != null : " + (AudioError != null));
        }
    }

    public void StartShopCountDown(){
        ShopCountDownTime = 5f;
        AniCountDown.SetBool("isCounting" , true);
    }

    public void SetPlayerInfo(bool isSelf , int deltaMoney){
        if(deltaMoney ==0)  return;
        string d = deltaMoney >= 0 ?"+":"" + deltaMoney.ToString();
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

        if (audioSource != null && AudioCheer != null)
        {
            audioSource.PlayOneShot(AudioCheer);
        }

        EventCenter.Instance.EventAddListener(EventCenterType.PlayerErrorAudio , OnPlayError);

        BarLeft.SetProBar(1,1,1);
        BarRight.SetProBar(1,1,1);

    }

    public void OnBuyError(){
        audioSource.PlayOneShot(AudioErrorBig);
        MyMoneyBeat.SetTrigger("BeatErr");
    }

    IEnumerator TickTick(int frame){
        for (float i = 0; i < frame; i++)
        {
            float k = 1.01f;
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
