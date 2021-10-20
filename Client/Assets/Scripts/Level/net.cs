using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;//引入socket命名空间
using System.Threading;
using System.Text;
using System.Collections;
using UnityEngine.SceneManagement;
using ProjectNetWork;

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
    public string EnemyPlayerMail;
    public string EnemyPlayerName;
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

        UIManager.PlayerPhone.ButtonBuy.onClick.AddListener(miniGameObserver.StartRandomMiniGame);
        UIManager.PlayerPhone.ButtonShutScreen.onClick.AddListener(TryShut);

        UIManager.PlayerAttackButton.ButtonAttack.onClick.AddListener(TryAttack);


        EventCenter.Instance.EventAddListener(EventCenterType.TryBuyItem , TryBuyItem);

        TryMatch();

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
        if(LevelManagerNetTest.Instance.IsAttacking)    return;
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqAttack_LoveCmd;

        Send(netMessage);
    }
    void TryBuyItem(params object[] data){
        NetMessage netMessage = new NetMessage();
        netMessage.ItemID = LevelManagerNetTest.Instance.NowItemID;
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqSendTryBuyItem_LoveCmd;

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

        UIManager.PlayerPhone.ImageShutMask.gameObject.SetActive(LevelManagerNetTest.Instance.IsMyScreenShut);
        UIManager.EnemyPhone.ImageShutMask.gameObject.SetActive(LevelManagerNetTest.Instance.IsEnemyScreenShut);
        UIManager.LightSelf.gameObject.SetActive(!LevelManagerNetTest.Instance.IsMyScreenShut);
        UIManager.LightEnemy.gameObject.SetActive(!LevelManagerNetTest.Instance.IsEnemyScreenShut);


        //UIManager.PlayerAttackButton.ButtonAttack.GetComponentInChildren<Text>().text = LevelManagerNetTest.Instance.IsAttacking ? "取消" : "攻击";

        //UIManager.EnemyPic.color = LevelManagerNetTest.Instance.IsEnemyAttacking ? Color.red : Color.white;
        //UIManager.MyPic.color = LevelManagerNetTest.Instance.IsAttacking ? Color.red : Color.white;

        //UIManager.TextMyName.text = LevelManagerNetTest.Instance.MyPlayerName;
        //UIManager.TextEnemyName.text = LevelManagerNetTest.Instance.EnemyPlayerName;

        UIManager.debugInGame.TextNowStat.text = LevelManagerNetTest.Instance.nowState.ToString();

        //UIManager.PlayerAttackButton.ButtonAttack.GetComponentInChildren<Text>().text = LevelManagerNetTest.Instance.IsAttacking ? "取消攻击" : "攻击";

        if(LevelManagerNetTest.Instance.MyPlayerInfo != null)   {
            UIManager.PlayerPhone.TextNowMoney.text = "$" + LevelManagerNetTest.Instance.MyPlayerInfo.Money.ToString();
        }
          
        if(LevelManagerNetTest.Instance.EnemyPlayerInfo != null)   {
            UIManager.EnemyPhone.TextNowMoney.text = "$" + LevelManagerNetTest.Instance.EnemyPlayerInfo.Money.ToString();
        }

        while(MessageQueue.Count>0){
            OnMessageReceive((NetMessage)MessageQueue.Dequeue());
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

            if(playerInfo.Money <= 0){
                LevelManagerNetTest.Instance.ResultWin = true;
                SceneManager.LoadScene("Result");
            }
        }else if(playerInfo.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
            if(LevelManagerNetTest.Instance.EnemyPlayerInfo!= null)    {
                dMoney = playerInfo.Money - LevelManagerNetTest.Instance.EnemyPlayerInfo.Money;
            }else{
                dMoney = playerInfo.Money;
            }
            LevelManagerNetTest.Instance.EnemyPlayerInfo = playerInfo;

            UIManager.SetPlayerInfo(false , dMoney);

            if(playerInfo.Money <= 0){
                LevelManagerNetTest.Instance.ResultWin = false;
                SceneManager.LoadScene("Result");
            }
        }

        
    }

    public void OnMessageReceive(NetMessage netMessage){
        //NetMessage netMessage = (NetMessage)data[0];
        switch (netMessage.MessageIndex)
        {
            case NetWorkMessageIndex.RetMessageMatchSuccess_LoveCmd:
                LevelManagerNetTest.Instance.nowState = LevelMatchState.Gaming;
                LevelManagerNetTest.Instance.EnemyPlayerMail = netMessage.EnemyMail;
                LevelManagerNetTest.Instance.EnemyPlayerName = netMessage.EnemyName;
                UIManager.OnMatchSuccess();
                //UIManager.TextEnemyName.text = LevelManagerNetTest.Instance.EnemyPlayerName;
                break;
            case NetWorkMessageIndex.RetMessageSpawnItem_LoveCmd:
                LevelManagerNetTest.Instance.NowItemID = ((int)netMessage.ItemID);
                UIManager.ReSpawnItem();
                Debug.Log("SpawnItem:" + LevelManagerNetTest.Instance.NowItemID.ToString());
                break;
            case NetWorkMessageIndex.RetMessageBuyItemSuccess_LoveCmd:
                LevelManagerNetTest.Instance.NowItemID = 0;
                if(netMessage.IsSuccess){
                    Debug.Log("I success");
                    UIManager.BuyItemSuccess(true);
                    break;
                }
                if(netMessage.IsEnemySuccess){
                    Debug.Log("Enemy Success");
                    UIManager.BuyItemSuccess(false);
                    break;
                }
                break;
            case NetWorkMessageIndex.RetLookBackSuccess_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    UIManager.ShowKiss();
                    UIManager.StopBlockCountDown();
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    UIManager.ShowAttackSuccess();
                }
                break;
            case NetWorkMessageIndex.RetCancelAttackReceived_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsAttacking = false;
                    UIManager.OnCancelAttack(true);
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyAttacking = false;
                    UIManager.OnCancelAttack(false);
                }
                break;
            case NetWorkMessageIndex.RetAttackReceived_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsAttacking = true;
                    UIManager.OnAttack(true);
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyAttacking = true;
                    if(!LevelManagerNetTest.Instance.IsMyScreenShut) UIManager.StartBlockCountDown();
                    UIManager.OnAttack(false);
                }
                break;
            case NetWorkMessageIndex.RetLightScreenReceived_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsMyScreenShut = false;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyScreenShut = false;
                }
                break;
            case NetWorkMessageIndex.RetShutScreenReceived_LoveCmd :
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsMyScreenShut = true;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyScreenShut = true;
                }
                break;
            case NetWorkMessageIndex.RetSetPlayerInfo_LoveCmd:
                SetPlayerInfo(netMessage.PlayerOne);
                SetPlayerInfo(netMessage.PlayerTwo);
                break;
            case NetWorkMessageIndex.RetBlockSuccess_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    UIManager.ShowBlock();
                    UIManager.StopBlockCountDown();
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    UIManager.ShowAttackFail();
                }
                break;
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