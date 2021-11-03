using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;//引入socket命名空间
using System.Threading;
using System.Text;
using System.Collections;
using UnityEngine.SceneManagement;
using ProjectNetWork;
public class HomePageNetTest : MonoBehaviour
{
    public HomePageUIManager homePageUIManager;
    public Queue MessageQueue = new Queue();

    void TryMatch(){
        SceneManager.LoadScene("Scene2In1");
    }

    void TryLogin(){
        if(!LevelManagerNetTest.Instance.HaveLogin){
            NetMessage netMessage = new NetMessage();  
            netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
            netMessage.PlayerName = LevelManagerNetTest.Instance.MyPlayerName;
            netMessage.MessageIndex = NetWorkMessageIndex.ReqPlayerLogin_LoveCmd;
            Send(netMessage);
        }else{
            TryMatch();
        }   
    }

    void TryConnectServer(){
        Debug.Log(123);
        Thread c_thread = new Thread(ConnectServer);
        c_thread.IsBackground = true;
        c_thread.Start();
    }
    
    public static void ConnectServer()
    {
        try
        {
            Debug.Log("try connect");
            IPAddress pAddress = IPAddress.Parse("10.21.248.76");
            IPEndPoint pEndPoint = new IPEndPoint(pAddress, 9000);
            LevelManagerNetTest.Instance.global_socket_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LevelManagerNetTest.Instance.global_socket_client.Connect(pEndPoint);

            Debug.Log("连接成功");
            //创建线程，执行读取服务器消息
            Thread c_thread = new Thread(Received);
            c_thread.IsBackground = true;
            c_thread.Start();
        }
        catch (System.Exception e)
        {

            Debug.Log(e);
        }
    }

    public static void Received()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[2048];
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

    public static void Send( NetMessage msgobj)
    {
       // try
       // {
            byte[] buffer = new byte[2048];
            Debug.Log("Send" + NetWorkUtility.toNetStr(msgobj));
            buffer = Encoding.UTF8.GetBytes(NetWorkUtility.toNetStr(msgobj));
            LevelManagerNetTest.Instance.global_socket_client.Send(buffer);
      //  }
      //  catch (System.Exception e)
      //  {
//
      //      Debug.Log(e);
      //  }
    }

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

    void EnqueueMessage(params object[] data){
        NetMessage netMessage = (NetMessage)data[0];
        MessageQueue.Enqueue(netMessage);
    }

    // Start is called before the first frame update
    void Start()
    {
        homePageUIManager = GetComponent<HomePageUIManager>();

        EventCenter.Instance.EventAddListener(EventCenterType.OnMessageReceive,EnqueueMessage);

        //homePageUIManager.MatchButton.onClick.AddListener(TryMatch);
        homePageUIManager.LoginButton.onClick.AddListener(TryLogin);

        TryConnectServer();

        if(LevelManagerNetTest.Instance.MyPlayerInfo!=null){
            homePageUIManager.MailInputField.text =  LevelManagerNetTest.Instance.MyPlayerMail.ToString();
            homePageUIManager.NameInputField.text =  LevelManagerNetTest.Instance.MyPlayerName.ToString();
        }

    }

    void TryGetScore(){
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqPlayerScoreList_LoveCmd;
        Send(netMessage);
    }

    // Update is called once per frame
    void Update()
    {
        while(MessageQueue.Count>0){
            OnMessageReceive((NetMessage)MessageQueue.Dequeue());
        }

        LevelManagerNetTest.Instance.MyPlayerMail = homePageUIManager.MailInputField.text;
        LevelManagerNetTest.Instance.MyPlayerName = homePageUIManager.NameInputField.text;

        if(LevelManagerNetTest.Instance.AliveSendTime == 0 ) LevelManagerNetTest.Instance.AliveSendTime = Time.timeSinceLevelLoad;
        
        if(Time.timeSinceLevelLoad - LevelManagerNetTest.Instance.AliveSendTime > 10 && LevelManagerNetTest.Instance.global_socket_client!= null){
            //Debug.Log(LevelManagerNetTest.Instance.nowState);
            LevelManagerNetTest.Instance.AliveSendTime = Time.timeSinceLevelLoad;
            SendAlive();
        }
    }

    public void OnMessageReceive(NetMessage netMessage){
        //NetMessage netMessage = (NetMessage)data[0];
        switch (netMessage.MessageIndex)
        {
            case NetWorkMessageIndex.RetPlayerLogin_LoveCmd:
                LevelManagerNetTest.Instance.nowState = LevelMatchState.LoginSuccess;
                homePageUIManager.NowStats.text = string.Format("LoginSuccess!\n{0}\n{1}" , LevelManagerNetTest.Instance.MyPlayerName , LevelManagerNetTest.Instance.MyPlayerMail);
                LevelManagerNetTest.Instance.HaveLogin = true;
                TryMatch();
                break;
            case NetWorkMessageIndex.RetPlayerScoreLiST_loveCmd:
                string t = "";
                int i = 0;
                foreach (SqlUser item in netMessage.PlayerScoreList)
                {
                    i++;
                    t += string.Format("#{0} {1} {2}\n" , i.ToString() , item.Name , item.Score.ToString());
                }

                homePageUIManager.ScoreList.text = t;
                break;
            default:break;
        }
    }

    public void SendAlive(){
        Debug.Log("Send Alive");
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqHeartBag_LoveCmd;
        Send(netMessage);
    }
 
}
