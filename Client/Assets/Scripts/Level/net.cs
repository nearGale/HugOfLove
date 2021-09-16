using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;//引入socket命名空间
using System.Threading;
using System.Text;
using System.Collections;
using UnityEngine.SceneManagement;
using ProjectNetWork;

public enum LevelState{
    Idle,
    Matching,
    Gaming
}

public class LevelManagerNetTest : Single<LevelManagerNetTest>
{
    public LevelState nowState = LevelState.Idle;

    public string MyPlayerMail = "123@garena.cn";
    public string MyPlayerName = "123";
    public string EnemyPlayerMail;
    public string EnemyPlayerName;
    public int NowItemID;
    public bool IsMyScreenShut;
    public bool IsAttacking;

    public Queue DebugQueue = new Queue();

}

public static class netDebug{
    public struct _Log{
        public string message;
        public string stackTrace;
        public LogType type;
    }
    public static void Log(string m , string s , LogType t){
        LevelManagerNetTest.Instance.DebugQueue.Enqueue(new _Log{message = m , stackTrace = s , type = t});
    }
}

public class net : MonoBehaviour
{
    float AliveSendTime = 0;

    public Queue MessageQueue = new Queue();
    public Button SendTryMatch;
    public Button TryConnect;
    public Button Buy;
    public Button Attack;
    public Button Shut;
    public InputField NameInput;
    public InputField MailInput;
    public Text EnemyPlayerName;
    public Text MyPlayerName;
    public Text NowStat;


    public Image MyPlayerPic;
    public Image EnemyPlayerPic;
    public Image ShutPic;


    public Text ItemDesc;
    public Image ItemPic;

    public Text NetDebugBox;

    // Start is called before the first frame update
    void Start()
    {

        LevelManagerNetTest.Instance.nowState = LevelState.Idle;
        LevelManagerNetTest.Instance.IsMyScreenShut = false;
        LevelManagerNetTest.Instance.IsAttacking = false;

        AliveSendTime = Time.timeSinceLevelLoad;

        EventCenter.Instance.EventAddListener(EventCenterType.OnMessageReceive,EnqueueMessage);

        SendTryMatch.onClick.AddListener(TryMatch);
        TryConnect.onClick.AddListener(TryConnectServer);
        Buy.onClick.AddListener(TryBuyItem);
        Attack.onClick.AddListener(TryAttack);
        Shut.onClick.AddListener(TryShut);

        //Application.logMessageReceived += netDebug.Log;

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
        netMessage.MessageIndex = NetWorkMessageIndex.ReqAttack_LoveCmd;

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
        LevelManagerNetTest.Instance.nowState = LevelState.Matching;
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
            //if(LevelManagerNetTest.Instance.nowState == LevelState.Matching) SendAlive();
        }

        ShutPic.enabled = LevelManagerNetTest.Instance.IsMyScreenShut;

        LevelManagerNetTest.Instance.MyPlayerMail = MailInput.text;
        LevelManagerNetTest.Instance.MyPlayerName = NameInput.text;

        MyPlayerName.text = LevelManagerNetTest.Instance.MyPlayerName;

        NowStat.text = LevelManagerNetTest.Instance.nowState.ToString();

        Attack.GetComponentInChildren<Text>().text = LevelManagerNetTest.Instance.IsAttacking ? "取消攻击" : "攻击";

        while(MessageQueue.Count>0){
            OnMessageReceive((NetMessage)MessageQueue.Dequeue());
        }

        while(LevelManagerNetTest.Instance.DebugQueue.Count >0){
            NetDebugBox.text = ((netDebug._Log)(LevelManagerNetTest.Instance.DebugQueue.Peek())).message.ToString() + "\n" + NetDebugBox.text;
            Application.logMessageReceived -= netDebug.Log;
            //Debug.Log(((netDebug._Log)(LevelManagerNetTest.Instance.DebugQueue.Dequeue())).message.ToString());
            Application.logMessageReceived += netDebug.Log;
        }
    }

    public void OnMessageReceive(NetMessage netMessage){
        //NetMessage netMessage = (NetMessage)data[0];
        switch (netMessage.MessageIndex)
        {
            case NetWorkMessageIndex.RetMessageMatchSuccess_LoveCmd:
                LevelManagerNetTest.Instance.nowState = LevelState.Gaming;
                LevelManagerNetTest.Instance.EnemyPlayerMail = netMessage.EnemyMail;
                LevelManagerNetTest.Instance.EnemyPlayerName = netMessage.EnemyName;
                EnemyPlayerName.text = "ene:"+LevelManagerNetTest.Instance.EnemyPlayerName;
                break;
            case NetWorkMessageIndex.RetMessageSpawnItem_LoveCmd:
                LevelManagerNetTest.Instance.NowItemID = ((int)netMessage.ItemID);
                ItemDesc.text = "item:"+netMessage.ItemID.ToString();
                break;
            case NetWorkMessageIndex.RetMessageBuyItemSuccess_LoveCmd:
                Debug.Log(string.Format("ME:{0},{1}" , netMessage.IsSuccess.GetType() , netMessage.IsSuccess));
                Debug.Log(string.Format("ENEMY:{0},{1}" , netMessage.IsEnemySuccess.GetType() , netMessage.IsEnemySuccess));
                if(netMessage.IsSuccess){
                    Debug.Log("I success");
                    ItemPic.color = Color.blue;
                    Invoke("TT",2f);
                }
                if(netMessage.IsEnemySuccess){
                    Debug.Log("Enemy Success");
                    ItemPic.color = Color.red;
                    Invoke("TT",2f);
                }
                break;
            case NetWorkMessageIndex.MessageBeAttacked_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.EnemyPlayerMail){
                    EnemyPlayerPic.color = Color.green;
                }else if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    MyPlayerPic.color = Color.green;
                    LevelManagerNetTest.Instance.IsAttacking = true;
                }
                Invoke("TTT",2f);
                break;
            case NetWorkMessageIndex.ReqCancelAttack_LoveCmd:
                if(netMessage.PlayerMail == LevelManagerNetTest.Instance.MyPlayerMail){
                    LevelManagerNetTest.Instance.IsAttacking = false;
                }
                TTT();
                break;
            

            default:break;
        }
    }

    public void TT(){
        ItemPic.color = Color.white;
    }
    public void TTT(){
        EnemyPlayerPic.color = Color.white;
        MyPlayerPic.color = Color.white;
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