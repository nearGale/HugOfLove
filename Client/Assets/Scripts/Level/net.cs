using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;//引入socket命名空间
using System.Threading;
using System.Text;
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

}

public class net : MonoBehaviour
{
    float AliveSendTime = 0;

    public Button SendTryMatch;
    public Button TryConnect;
    public Button Buy;
    public Button Attack;
    public InputField NameInput;
    public InputField MailInput;
    public Text EnemyPlayerName;
    public Text MyPlayerName;

    public Text ItemDesc;
    public Image ItemPic;
    // Start is called before the first frame update
    void Start()
    {
        
        LevelManagerNetTest.Instance.nowState = LevelState.Idle;

        AliveSendTime = Time.timeSinceLevelLoad;

        EventCenter.Instance.EventAddListener(EventCenterType.OnMessageReceive,OnMessageReceive);

        SendTryMatch.onClick.AddListener(TryMatch);
        TryConnect.onClick.AddListener(TryConnectServer);
        Buy.onClick.AddListener(TryBuyItem);
        Attack.onClick.AddListener(TryAttack);
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
    void Update()
    {
        if(Time.timeSinceLevelLoad - AliveSendTime > 3){
            //Debug.Log(LevelManagerNetTest.Instance.nowState);
            AliveSendTime = Time.timeSinceLevelLoad;
            //if(LevelManagerNetTest.Instance.nowState == LevelState.Matching) SendAlive();
        }

        LevelManagerNetTest.Instance.MyPlayerMail = MailInput.text;
        LevelManagerNetTest.Instance.MyPlayerName = NameInput.text;
    }

    public void OnMessageReceive(params object[] data){
        NetMessage netMessage = (NetMessage)data[0];
        Debug.Log(netMessage.MessageIndex);
        switch (netMessage.MessageIndex)
        {
            case NetWorkMessageIndex.RetMessageMatchSuccess_LoveCmd:
                LevelManagerNetTest.Instance.nowState = LevelState.Gaming;
                LevelManagerNetTest.Instance.EnemyPlayerMail = netMessage.EnemyMail;
                LevelManagerNetTest.Instance.EnemyPlayerName = netMessage.EnemyName;
                EnemyPlayerName.text = "ene:"+LevelManagerNetTest.Instance.EnemyPlayerName;
                break;
            case NetWorkMessageIndex.MessageSpawnItem_LoveCmd:
                LevelManagerNetTest.Instance.NowItemID = ((int)netMessage.ItemID);
                ItemDesc.text = "item:"+netMessage.ItemID.ToString();
                break;
            case NetWorkMessageIndex.MessageBuyItemSuccess_LoveCmd:
                if(netMessage.isSuccess){
                    ItemPic.color = Color.blue;
                    Invoke("TT",2f);
                }
                if(netMessage.isEnemySuccess){
                    ItemPic.color = Color.red;
                    Invoke("TT",2f);
                }
                break;
            case NetWorkMessageIndex.MessageBeAttacked_LoveCmd:

            default:break;
        }
    }

    public void TT(){
        ItemPic.color = Color.white;
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

    static void TryLogin(){
        NetMessage netMessage = new NetMessage();  

        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.PlayerName = LevelManagerNetTest.Instance.MyPlayerName;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqPlayerLogintwo_LoveCmd;
        Send(netMessage);
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
            Debug.Log(msgobj.MessageIndex);
            byte[] buffer = new byte[1024];
            Debug.Log(NetWorkUtility.toNetStr(msgobj));
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