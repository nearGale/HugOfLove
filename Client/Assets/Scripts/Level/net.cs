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
    Matching,
    Gaming
}

public class LevelPlayerInfo{
    public int Money;
    public float AttackTime;
}

public class LevelManagerNetTest : Single<LevelManagerNetTest>
{
    public LevelMatchState nowState = LevelMatchState.Idle;

    public string MyPlayerMail = "123@garena.cn";
    public string MyPlayerName = "123";
    public string EnemyPlayerMail;
    public string EnemyPlayerName;
    public int NowItemID;
    public bool IsMyScreenShut;
    public bool IsEnemyScreenShut;
    public bool IsAttacking;
    public bool IsEnemyAttacking;
    public Queue DebugQueue = new Queue();

}

public class net : MonoBehaviour
{
    float AliveSendTime = 0;

    public UIManagerNetTest UIManager;

    public Queue MessageQueue = new Queue();

    // Start is called before the first frame update
    void Start()
    {

        UIManager = GetComponent<UIManagerNetTest>();

        LevelManagerNetTest.Instance.nowState = LevelMatchState.Idle;
        LevelManagerNetTest.Instance.IsMyScreenShut = false;
        LevelManagerNetTest.Instance.IsAttacking = false;
        LevelManagerNetTest.Instance.IsEnemyAttacking = false;

        AliveSendTime = Time.timeSinceLevelLoad;

        EventCenter.Instance.EventAddListener(EventCenterType.OnMessageReceive,EnqueueMessage);

        UIManager.debugInGame.ButtonTryMatch.onClick.AddListener(TryMatch);
        UIManager.debugInGame.ButtonTryConnect.onClick.AddListener(TryConnectServer);

        UIManager.PlayerPhone.ButtonBuy.onClick.AddListener(TryBuyItem);
        UIManager.PlayerPhone.ButtonShutScreen.onClick.AddListener(TryShut);

        UIManager.PlayerAttackButton.ButtonAttack.onClick.AddListener(TryAttack);

    }

    void EnqueueMessage(params object[] data){
        NetMessage netMessage = (NetMessage)data[0];
        MessageQueue.Enqueue(netMessage);
    }
    void TryShut(){
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = LevelManagerNetTest.Instance.IsMyScreenShut?NetWorkMessageIndex.ReqLightScreen_LoveCmd:NetWorkMessageIndex.ReqShutScreen_LoveCmd;

        Send(netMessage);
    }
    void TryConnectServer(){
        Thread c_thread = new Thread(ConnectServer);
        c_thread.IsBackground = true;
        c_thread.Start();
    }

    void TryAttack(){
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = LevelManagerNetTest.Instance.IsAttacking ? NetWorkMessageIndex.ReqCancelAttack_LoveCmd : NetWorkMessageIndex.ReqAttack_LoveCmd;

        Send(netMessage);
    }
    void TryBuyItem(){
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
        if(Time.timeSinceLevelLoad - AliveSendTime > 3){
            //Debug.Log(LevelManagerNetTest.Instance.nowState);
            AliveSendTime = Time.timeSinceLevelLoad;
            //if(LevelManagerNetTest.Instance.nowState == LevelMatchState.Matching) SendAlive();
        }

        UIManager.PlayerPhone.ImageShutMask.gameObject.SetActive(LevelManagerNetTest.Instance.IsMyScreenShut);
        UIManager.EnemyPhone.ImageShutMask.gameObject.SetActive(LevelManagerNetTest.Instance.IsEnemyScreenShut);

        UIManager.PlayerAttackButton.ButtonAttack.GetComponentInChildren<Text>().text = LevelManagerNetTest.Instance.IsAttacking ? "取消" : "攻击";

        UIManager.EnemyPic.color = LevelManagerNetTest.Instance.IsEnemyAttacking ? Color.red : Color.white;
        UIManager.MyPic.color = LevelManagerNetTest.Instance.IsAttacking ? Color.red : Color.white;

        LevelManagerNetTest.Instance.MyPlayerMail = UIManager.debugInGame.InputFieldPlayerMail.text;
        LevelManagerNetTest.Instance.MyPlayerName = UIManager.debugInGame.InputFieldPlayerName.text;
        UIManager.TextMyName.text = LevelManagerNetTest.Instance.MyPlayerName;
        UIManager.TextEnemyName.text = LevelManagerNetTest.Instance.EnemyPlayerName;

        UIManager.debugInGame.TextNowStat.text = LevelManagerNetTest.Instance.nowState.ToString();

        UIManager.PlayerAttackButton.ButtonAttack.GetComponentInChildren<Text>().text = LevelManagerNetTest.Instance.IsAttacking ? "取消攻击" : "攻击";

        while(MessageQueue.Count>0){
            OnMessageReceive((NetMessage)MessageQueue.Dequeue());
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
                UIManager.TextEnemyName.text = LevelManagerNetTest.Instance.EnemyPlayerName;
                break;
            case NetWorkMessageIndex.RetMessageSpawnItem_LoveCmd:
                LevelManagerNetTest.Instance.NowItemID = ((int)netMessage.ItemID);
                UIManager.EnemyPhone.TextItemDesc.text = "item:"+netMessage.ItemID.ToString();
                break;
            case NetWorkMessageIndex.RetMessageBuyItemSuccess_LoveCmd:
                Debug.Log(string.Format("ME:{0},{1}" , netMessage.IsSuccess.GetType() , netMessage.IsSuccess));
                Debug.Log(string.Format("ENEMY:{0},{1}" , netMessage.IsEnemySuccess.GetType() , netMessage.IsEnemySuccess));
                if(netMessage.IsSuccess){
                    Debug.Log("I success");
                    UIManager.EnemyPhone.ImageItemPic.color = Color.blue;
                    UIManager.PlayerPhone.ImageItemPic.color = Color.blue;
                }
                if(netMessage.IsEnemySuccess){
                    Debug.Log("Enemy Success");
                    UIManager.EnemyPhone.ImageItemPic.color = Color.red;
                    UIManager.PlayerPhone.ImageItemPic.color = Color.red;
                }
                break;
            case NetWorkMessageIndex.MessageBeAttacked_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    UIManager.EnemyPic.color = Color.green;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    UIManager.MyPic.color = Color.green;
                }
                break;
            case NetWorkMessageIndex.RetCancelAttackReceived_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsAttacking = false;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyAttacking = false;
                }
                break;
            case NetWorkMessageIndex.RetAttackReceived:
                Debug.Log(string.Format("receive Attack!enemy:{0}   me:{1}" , LevelManagerNetTest.Instance.EnemyPlayerMail , LevelManagerNetTest.Instance.MyPlayerMail));
                Debug.Log("Receive ene:" + netMessage.EnemyMail);
                Debug.Log("Receive me:" + netMessage.PlayerMail);
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsAttacking = true;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyAttacking = true;
                }
                break;
            case NetWorkMessageIndex.RetLightScreenReceived:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsMyScreenShut = false;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyScreenShut = false;
                }
                break;
            case NetWorkMessageIndex.RetShutScreenReceived:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsMyScreenShut = true;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    LevelManagerNetTest.Instance.IsEnemyScreenShut = true;
                }
                break;
            default:break;
        }
    }
    public void SendAlive(){
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        Send(netMessage);
    }

 　　/// <summary>
    /// 连接服务器
    /// </summary>
    static Socket socket_client;
    public static void ConnectServer()
    {
        try
        {
            Debug.Log("try connect");
            IPAddress pAddress = IPAddress.Parse("10.21.248.76");
            IPEndPoint pEndPoint = new IPEndPoint(pAddress, 9000);
            socket_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket_client.Connect(pEndPoint);
            Debug.Log("连接成功");
            //创建线程，执行读取服务器消息
            Thread c_thread = new Thread(Received);
            c_thread.IsBackground = true;
            c_thread.Start();

            TryLogin();
        }
        catch (System.Exception e)
        {

            Debug.Log(e);
        }
    }

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
                int len = socket_client.Receive(buffer);
                if (len == 0) break;
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("客户端打印服务器返回消息：" + socket_client.RemoteEndPoint + ":" + str);
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
       // try
       // {
            byte[] buffer = new byte[1024];
            Debug.Log("Send" + NetWorkUtility.toNetStr(msgobj));
            buffer = Encoding.UTF8.GetBytes(NetWorkUtility.toNetStr(msgobj));
            socket_client.Send(buffer);
      //  }
      //  catch (System.Exception e)
      //  {
//
      //      Debug.Log(e);
      //  }
    }
　　 /// <summary>
    /// 关闭连接
    /// </summary>
    public static void close()
    {
        try
        {
            socket_client.Close();
            Debug.Log("关闭客户端连接");
            SceneManager.LoadScene("control");
        }
        catch (System.Exception)
        {
            Debug.Log("未连接");
        }
    }
}