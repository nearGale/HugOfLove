using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HomePageApp : BasicApp
{
    public InputField nameInputField = null;
    public InputField mailInputField = null;
    public PlayerInfo playerInfo = null;

    bool Transiable = true;

    // Start is called before the first frame update
    void Start()
    {
        AppName = "HomePageApp";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TryStartGame(){
        Debug.Log("Try Start");
        playerInfo = new PlayerInfo();
        Debug.Log(playerInfo);
        playerInfo.name = nameInputField.text;
        playerInfo.mail = mailInputField.text;
        myphone.playerInfo = playerInfo;

        EventCenter.Instance.EventTrigger(EventCenterType.MatchSuccess , new ProjectNetWork.MessageMatchSuccess());
    }

}
