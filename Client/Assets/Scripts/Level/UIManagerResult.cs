using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;//引入socket命名空间
using System.Threading;
using System.Text;
using System.Collections;
using UnityEngine.SceneManagement;
using ProjectNetWork;
public class UIManagerResult : MonoBehaviour
{
    public Text r;
    // Start is called before the first frame update
    void Start()
    {
        r.text = LevelManagerNetTest.Instance.ResultWin ? "胜利" : "失败" ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        EventCenter.Instance.Clear();
        SceneManager.LoadScene("HomePage");
    }

    private void OnApplicationQuit() {
        NetMessage netMessage = new NetMessage();
        netMessage.PlayerMail = LevelManagerNetTest.Instance.MyPlayerMail;
        netMessage.MessageIndex = NetWorkMessageIndex.ReqPlayerLogout_LoveCmd;
        Send(netMessage);
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
}
