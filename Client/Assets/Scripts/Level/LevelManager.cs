using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class PlayerInfo{
    public string name = "玩家名";
    public string mail = "乱七八糟@garena.cn";
    public int HP = 100;
    public int AttackTimes = 2;

}

public class LevelManager : MonoBehaviour
{
    public Phone phone = null;
    public ThatGirl thatGirl = null;
    public Text playerInfoText = null;
    public Text enemyInfoText = null;
    public Text attackInfoText = null;

    public PlayerInfo playerInfo = new PlayerInfo();
    public PlayerInfo enemyInfo = new PlayerInfo();
    // Start is called before the first frame update
    void Start()
    {
        phone.levelManager = this;
        thatGirl.levelManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        playerInfoText.text = string.Format("name:{0}\nmail:{1}\nHP:{2}",playerInfo.name,playerInfo.mail,playerInfo.HP);
        enemyInfoText.text = string.Format("name:{0}\nmail:{1}\nHP:{2}",enemyInfo.name,enemyInfo.mail,enemyInfo.HP);
        attackInfoText.text = string.Format("回头次数:{0}",playerInfo.AttackTimes);

        if(Input.GetKeyDown(KeyCode.Q)){
            if(playerInfo.AttackTimes > 0){
                playerInfo.AttackTimes--;
                thatGirl.GetComponent<SpriteRenderer>().color = Color.blue;
                enemyInfo.HP -= 10;
                Invoke("AA",.3f);
            }
        }
    }

    void AA(){
        thatGirl.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
