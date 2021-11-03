using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;//引入socket命名空间
using System.Threading;
using System.Text;
using System.Collections;
using UnityEngine.SceneManagement;
using ProjectNetWork;
using System.Collections.Generic;
public enum LevelMatchState{
    Idle,
    LoginSuccess,
    Matching,
    Gaming,
    Error
}

public class LevelManagerNetTest : Single<LevelManagerNetTest>
{
    public LevelMatchState nowState = LevelMatchState.Idle;
    public string MyPlayerMail = "123@garena.cn";
    public string MyPlayerName = "123";
    public int MyPlayerRank;
    public string EnemyPlayerMail;
    public string EnemyPlayerName;
    public int EnemyPlayerRank;
    public int NowItemID;
    public bool IsMyScreenShut = false;
    public bool IsEnemyScreenShut;
    public bool IsAttacking;
    public bool IsEnemyAttacking;
    public Queue DebugQueue = new Queue();
    public Socket global_socket_client;
    public PlayerInfo EnemyPlayerInfo;
    public PlayerInfo MyPlayerInfo;

    public bool ResultWin;

    public float AliveSendTime;

    public int EnemyAttackTime = 1;
    public int MyAttackTime = 1;
    public bool HaveLogin = false;
}

public class net : MonoBehaviour
{


    public UIManagerNetTest UIManager;
    public MiniGameObserver miniGameObserver;

    public Queue MessageQueue = new Queue();

    // Start is called before the first frame update
    void Start()
    {

        UIManager = GetComponent<UIManagerNetTest>();
        miniGameObserver = GetComponent<MiniGameObserver>();

        LevelManagerNetTest.Instance.nowState = LevelMatchState.Idle;
        LevelManagerNetTest.Instance.IsMyScreenShut = false;
        LevelManagerNetTest.Instance.IsAttacking = false;
        LevelManagerNetTest.Instance.IsEnemyAttacking = false;

        LevelManagerNetTest.Instance.AliveSendTime = Time.timeSinceLevelLoad;

        EventCenter.Instance.EventAddListener(EventCenterType.OnMessageReceive,EnqueueMessage);

        //UIManager.PlayerPhone.ButtonBuy.onClick.AddListener(miniGameObserver.StartRandomMiniGame);
        //UIManager.PlayerPhone.ButtonShutScreen.onClick.AddListener(TryShut);

        //UIManager.PlayerAttackButton.ButtonAttack.onClick.AddListener(TryAttack);


        EventCenter.Instance.EventAddListener(EventCenterType.BlockSuccess , TryBlockSuccess);

        TryMatch();

        GameSetting.Instance.shopItems = new List<ShopItem>{
         new ShopItem(1 , 800 , "全称卡拉什尼科夫1947年式自动步枪，从不卡壳！经典之作！高光返场！\n店铺：米莎的最爱" , Resources.Load<Sprite>("6_AK")),
         new ShopItem(2 , 1200 , "全塑料设计，你懂的。此时下单附送300发子弹，暴赚不亏！！\n卖家：马克弟弟西姆" , Resources.Load<Sprite>("10_G18")),
         new ShopItem(3 , 2300 , "想要跑得快吗？买两把吧！风驰般的感觉，追男神利器。\n卖家：凯莉妹妹" , Resources.Load<Sprite>("49_Vector")),
         new ShopItem(4 , 3200 , "尝试一下最新的流血弹科技吗？官方正版，假一罚三。\n卖家：警官柔拉" , Resources.Load<Sprite>("13_VSS")),
         new ShopItem(5 , 3700 , "害怕了吗？出发前多备两个，高低多一条命出来。\n卖家：悍匪之家" , Resources.Load<Sprite>("HEAL")),
         new ShopItem(6 , 4200 , "艺术就是爆炸！艺术就是大星星！！爆炸艺术，暴力美学的艺术。\n卖方：火神殿下" , Resources.Load<Sprite>("gen")),
         new ShopItem(7 , 4700 , "路见不平，拔刀相助！没有武士刀的武士不是好武士。\n卖方：刀阁下" , Resources.Load<Sprite>("Icon_slot_Katana")),
        };
        Debug.Log(GameSetting.Instance.shopItems[5].Description);

        LevelManagerNetTest.Instance.MyAttackTime = 1;
        LevelManagerNetTest.Instance.EnemyAttackTime = 1;

        LevelManagerNetTest.Instance.NowItemID = 0;
        //miniGameObserver.StartRandomMiniGame();


    }
    void CallMiniGame(){

    }

    void EnqueueMessage(params object[] data){
        NetMessage netMessage = (NetMessage)data[0];
        MessageQueue.Enqueue(netMessage);
    }
    void TryShut(){
        if(LevelManagerNetTest.Instance.IsEnemyAttacking && LevelManagerNetTest.Instance.IsMyScreenShut)   return;

        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        Debug.Log("Now Scree:" + LevelManagerNetTest.Instance.IsMyScreenShut.ToString());
        netMessage.MessageIndex = LevelManagerNetTest.Instance.IsMyScreenShut?NetWorkMessageIndex.ReqLightScreen_LoveCmd:NetWorkMessageIndex.ReqShutScreen_LoveCmd;

        Send(netMessage);
    }

    void TryAttack(){
        if(!(LevelManagerNetTest.Instance.nowState == LevelMatchState.Gaming))  return;
        if(LevelManagerNetTest.Instance.IsAttacking)    return;
        if(LevelManagerNetTest.Instance.IsEnemyAttacking)   return;
        if(LevelManagerNetTest.Instance.MyAttackTime <= 0){
            return;
        }
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqAttack_LoveCmd;

        Send(netMessage);
    }
    void TryBuyItem(params object[] data){
        if(!(LevelManagerNetTest.Instance.nowState == LevelMatchState.Gaming))  return;
        if(UIManager.ShopCountDownTime >= 0 )    return;
        if(LevelManagerNetTest.Instance.IsEnemyAttacking == true)   return;
        if(LevelManagerNetTest.Instance.NowItemID <= 0) return;
        NetMessage netMessage = new NetMessage();
        netMessage.ItemID = LevelManagerNetTest.Instance.NowItemID;
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqSendTryBuyItem_LoveCmd;
        Send(netMessage);
    }

    void TryBlockSuccess(params object[] data){
        NetMessage netMessage = new NetMessage();
        //netMessage.ItemID = LevelManagerNetTest.Instance.NowItemID;
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqLightScreen_LoveCmd;
        Send(netMessage);
    }
    // Update is called once per frame
    void TryMatch(){
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqSendTryMatch_LoveCmd;
        LevelManagerNetTest.Instance.nowState = LevelMatchState.Matching;
        Send(netMessage);
    }

    static void TryLogin(){
        NetMessage netMessage = new NetMessage();  

        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.PlayerName = LevelManagerNetTest.Instance.MyPlayerName;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqPlayerLogin_LoveCmd;
        Send(netMessage);
    }

    void Update()
    {
        if(LevelManagerNetTest.Instance.AliveSendTime == 0 ) LevelManagerNetTest.Instance.AliveSendTime = Time.timeSinceLevelLoad;
        if(Time.timeSinceLevelLoad - LevelManagerNetTest.Instance.AliveSendTime > 10&& LevelManagerNetTest.Instance.global_socket_client!= null){
            //Debug.Log(LevelManagerNetTest.Instance.nowState);
            LevelManagerNetTest.Instance.AliveSendTime = Time.timeSinceLevelLoad;
            SendAlive();
        }

        //UIManager.PlayerPhone.ImageShutMask.gameObject.SetActive(LevelManagerNetTest.Instance.IsMyScreenShut);
        //UIManager.EnemyPhone.ImageShutMask.gameObject.SetActive(LevelManagerNetTest.Instance.IsEnemyScreenShut);



        //UIManager.PlayerAttackButton.ButtonAttack.GetComponentInChildren<Text>().text = LevelManagerNetTest.Instance.IsAttacking ? "取消" : "攻击";

        //UIManager.EnemyPic.color = LevelManagerNetTest.Instance.IsEnemyAttacking ? Color.red : Color.white;
        //UIManager.MyPic.color = LevelManagerNetTest.Instance.IsAttacking ? Color.red : Color.white;

        //UIManager.TextMyName.text = LevelManagerNetTest.Instance.MyPlayerName;
        //UIManager.TextEnemyName.text = LevelManagerNetTest.Instance.EnemyPlayerName;

        //UIManager.debugInGame.TextNowStat.text = LevelManagerNetTest.Instance.nowState.ToString();

        //UIManager.PlayerAttackButton.ButtonAttack.GetComponentInChildren<Text>().text = LevelManagerNetTest.Instance.IsAttacking ? "取消攻击" : "攻击";

        if(LevelManagerNetTest.Instance.MyPlayerInfo != null)   {
            UIManager.PlayerPhone.TextNowMoney.text = "$" + LevelManagerNetTest.Instance.MyPlayerInfo.Money.ToString();
            UIManager.PlayerPhone.TextLookBackTime.text = LevelManagerNetTest.Instance.MyAttackTime.ToString();
        }
          
        if(LevelManagerNetTest.Instance.EnemyPlayerInfo != null)   {
            UIManager.EnemyPhone.TextNowMoney.text = LevelManagerNetTest.Instance.EnemyPlayerInfo.Money.ToString();
            UIManager.EnemyPhone.TextLookBackTime.text = LevelManagerNetTest.Instance.EnemyAttackTime.ToString();
        }

        while(MessageQueue.Count>0){
            OnMessageReceive((NetMessage)MessageQueue.Dequeue());
        }

    }

    private void LateUpdate() {
        if(Input.GetKeyUp(KeyCode.Return)){
            TryAttack();
        }
        if(Input.GetKeyUp(KeyCode.Space)){
            TryBuyItem();
        }
    }
    public void SetPlayerInfo(PlayerInfo playerInfo){
        int dMoney;
        Debug.Log(playerInfo);
        if(playerInfo == null)  return;
        if(playerInfo.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
            if(LevelManagerNetTest.Instance.MyPlayerInfo!= null)    {
                dMoney = playerInfo.Money - LevelManagerNetTest.Instance.MyPlayerInfo.Money;
            }else{
                dMoney = playerInfo.Money;
            }
            LevelManagerNetTest.Instance.MyPlayerInfo = playerInfo;

            UIManager.SetPlayerInfo(true , dMoney);

            if(playerInfo.Money <= 1000){
                LevelManagerNetTest.Instance.ResultWin = true;
                Invoke("GotoResult" , 2f);
            }
        }else if(playerInfo.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
            if(LevelManagerNetTest.Instance.EnemyPlayerInfo!= null)    {
                dMoney = playerInfo.Money - LevelManagerNetTest.Instance.EnemyPlayerInfo.Money;
            }else{
                dMoney = playerInfo.Money;
            }
            LevelManagerNetTest.Instance.EnemyPlayerInfo = playerInfo;

            UIManager.SetPlayerInfo(false , dMoney);

            if(playerInfo.Money <= 1000){
                LevelManagerNetTest.Instance.ResultWin = false;
                Invoke("GotoResult" , 2f);
            }
        }

        
    }

    public void GotoResult(){
        SceneManager.LoadScene("Result");
    }

    public void OnEnemyAttack(){
        if(LevelManagerNetTest.Instance.IsEnemyAttacking == true)   return;
        UIManager.OnAttack(false);
        LevelManagerNetTest.Instance.IsEnemyAttacking = true;
        miniGameObserver.StartRandomMiniGame();
    }



    public void ShowPlayerInfo(){
        UIManager.PlayerPhone.Info.text = string.Format("玩家名：{0}\n排名：#{1}",LevelManagerNetTest.Instance.MyPlayerName , LevelManagerNetTest.Instance.MyPlayerRank);
        UIManager.EnemyPhone.Info.text = string.Format("玩家名：{0}\n排名：#{1}",LevelManagerNetTest.Instance.EnemyPlayerName , LevelManagerNetTest.Instance.EnemyPlayerRank);


        UIManager.PlayerPhone.InfoRoot.SetActive(true);
        UIManager.EnemyPhone.InfoRoot.SetActive(true);
    }

    public void OnMessageReceive(NetMessage netMessage){
        //NetMessage netMessage = (NetMessage)data[0];
        switch (netMessage.MessageIndex)
        {
            case NetWorkMessageIndex.RetMessageMatchSuccess_LoveCmd:
                LevelManagerNetTest.Instance.nowState = LevelMatchState.Gaming;
                LevelManagerNetTest.Instance.EnemyPlayerMail = netMessage.EnemyMail;
                LevelManagerNetTest.Instance.EnemyPlayerName = netMessage.EnemyName;

                LevelManagerNetTest.Instance.EnemyPlayerRank = netMessage.EnemyRank;
                LevelManagerNetTest.Instance.MyPlayerRank = netMessage.PlayerRank;

                UIManager.OnMatchSuccess();


                UIManager.PlayerPhone.InfoMe.SetTrigger("BLINK");

                Invoke("ShowPlayerInfo"  , 2f);

                //UIManager.TextEnemyName.text = LevelManagerNetTest.Instance.EnemyPlayerName;
                break;
            case NetWorkMessageIndex.RetMessageSpawnItem_LoveCmd:
                LevelManagerNetTest.Instance.NowItemID = ((int)netMessage.ItemID);
                UIManager.ReSpawnItem();
                Debug.Log("SpawnItem:" + LevelManagerNetTest.Instance.NowItemID.ToString());
                UIManager.PlayerPhone.ImageItemPic.color = new Color(1,1,1,1);
                break;
            case NetWorkMessageIndex.RetMessageBuyItemSuccess_LoveCmd:
                if(netMessage.IsSuccess){
                    LevelManagerNetTest.Instance.NowItemID = 0;
                    Debug.Log("I success");
                    UIManager.BuyItemSuccess(true);

                    LevelManagerNetTest.Instance.EnemyAttackTime++;

                    UIManager.PlayerPhone.ImageItemPic.color = new Color(1,1,1,0);
                    break;
                }
                if(netMessage.IsEnemySuccess){
                    LevelManagerNetTest.Instance.NowItemID = 0;
                    Debug.Log("Enemy Success");
                    UIManager.BuyItemSuccess(false);

                    LevelManagerNetTest.Instance.MyAttackTime++;
                    UIManager.PlayerPhone.ImageItemPic.color = new Color(1,1,1,0);
                    break;
                }
                break;
            //case NetWorkMessageIndex.RetLookBackSuccess_LoveCmd:
            //    if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
            //        UIManager.ShowKiss();
            //        UIManager.StopBlockCountDown();
            //    }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
            //        UIManager.ShowAttackSuccess();
            //    }
            //    break;
            //case NetWorkMessageIndex.RetCancelAttackReceived_LoveCmd:
            //    if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
            //        LevelManagerNetTest.Instance.IsAttacking = false;
            //        UIManager.OnCancelAttack(true);
            //    }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
            //        LevelManagerNetTest.Instance.IsEnemyAttacking = false;
            //        UIManager.OnCancelAttack(false);
            //    }
            //    break;
            case NetWorkMessageIndex.RetAttackReceived_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsAttacking = true;
                    UIManager.OnAttack(true);
                    LevelManagerNetTest.Instance.IsEnemyScreenShut = true;
                    UIManager.EnemyPhoneAnimator.SetBool("Light" , false);

                    LevelManagerNetTest.Instance.MyAttackTime--;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    OnEnemyAttack();
                    LevelManagerNetTest.Instance.EnemyAttackTime--;
                    //if(!LevelManagerNetTest.Instance.IsMyScreenShut) UIManager.StartBlockCountDown();
                }
                break;
            case NetWorkMessageIndex.RetLightScreenReceived_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsMyScreenShut = false;

                    UIManager.AniBoth.SetBool("active" , true);
                    UIManager.AniMe.SetBool("active" , false);
                    UIManager.AniEne.SetBool("active" , false);

                    LevelManagerNetTest.Instance.IsEnemyAttacking = false;

                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyScreenShut = false;
                    UIManager.EnemyPhoneAnimator.SetBool("Light" , true);
                    LevelManagerNetTest.Instance.IsAttacking = false;

                    UIManager.OnMatchSuccess();
                }
                break;
            case NetWorkMessageIndex.RetAuction_LoveCmd:
                if(LevelManagerNetTest.Instance.NowItemID == 0) break;
                LevelManagerNetTest.Instance.NowItemID = 0;
                Debug.Log("Auction");
                UIManager.BuyItemSuccess(false , true);

                //LevelManagerNetTest.Instance.MyAttackTime++;
                //LevelManagerNetTest.Instance.EnemyAttackTime++;
                break;
            //case NetWorkMessageIndex.RetShutScreenReceived_LoveCmd :
            //    if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
            //        LevelManagerNetTest.Instance.IsMyScreenShut = true;
            //    }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
            //        LevelManagerNetTest.Instance.IsEnemyScreenShut = true;
            //    }
            //    break;
            case NetWorkMessageIndex.RetSetPlayerInfo_LoveCmd:
                SetPlayerInfo(netMessage.PlayerOne);
                SetPlayerInfo(netMessage.PlayerTwo);
                break;
            //case NetWorkMessageIndex.RetBlockSuccess_LoveCmd:
            //    if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
            //        UIManager.ShowBlock();
            //        UIManager.StopBlockCountDown();
            //    }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
            //        UIManager.ShowAttackFail();
            //    }
            //    break;
            default:break;
        }
    }
    public void OnAttack(bool IsEnemy){

    }
    public void SendAlive(){
        Debug.Log("Send Alive");
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqHeartBag_LoveCmd;
        Send(netMessage);
    }

 　　/// <summary>
    /// 连接服务器
    /// </summary>
    static Socket socket_client;

　　 /// <summary>
    /// 读取服务器消息
    /// </summary>
    public static void Received()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int len = LevelManagerNetTest.Instance.global_socket_client.Receive(buffer);
                if (len == 0) break;
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("客户端打印服务器返回消息：" + LevelManagerNetTest.Instance.global_socket_client.RemoteEndPoint + ":" + str);
                NetMessage Ret = JsonUtility.FromJson<NetMessage>(str);
                EventCenter.Instance.EventTrigger(EventCenterType.OnMessageReceive , Ret);
            }
            catch (System.Exception)
            {

                throw;
            }

        }
    }
 　　/// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg"></param>
    public static void Send( NetMessage msgobj)
    {
        try
        {
          byte[] buffer = new byte[1024];
          Debug.Log("Send" + NetWorkUtility.toNetStr(msgobj));
          buffer = Encoding.UTF8.GetBytes(NetWorkUtility.toNetStr(msgobj));
          LevelManagerNetTest.Instance.global_socket_client.Send(buffer);
        }
        catch (System.Exception e)
        {
            LevelManagerNetTest.Instance.nowState = LevelMatchState.Error;

            Debug.Log(e);
        }
    }
　　 /// <summary>
    /// 关闭连接
    /// </summary>
    public static void close()
    {
        try
        {
            LevelManagerNetTest.Instance.global_socket_client.Close();
            Debug.Log("关闭客户端连接");
            SceneManager.LoadScene("control");
        }
        catch (System.Exception)
        {
            Debug.Log("未连接");
        }
    }
}