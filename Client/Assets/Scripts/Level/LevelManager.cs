using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;



[Serializable]
public class PlayerInfo{
    public string name = "玩家名";
    public string mail = "乱七八糟@garena.cn";
    public int HP = 10000;
    public float AttackTimes = .2f;

}

[Serializable]
public class LevelPrefabList{
    public GameObject ThatGirl = null;
    public GameObject Phone = null;
    public GameObject UIAttackButton = null;
}

[Serializable]
public class LevelObjects{
    public Camera MainCamera = null;
    public RawImage ContentImage = null;
    public Canvas UICanvas = null;
}

public enum GameState{
    Idle,
    Matching,
    InGame,
    Result
}

public class LevelManager : MonoBehaviour
{
    public Phone phone = null;
    public ThatGirl thatGirl = null;
    public Text playerInfoText = null;
    public Text enemyInfoText = null;

    public PlayerInfo playerInfo = new PlayerInfo();
    public PlayerInfo enemyInfo = new PlayerInfo();

    public LevelPrefabList levelPrefabList = new LevelPrefabList();

    public LevelObjects levelObjects = new LevelObjects();
    public AttackButton attackButton = null;
    GameState nowState = GameState.Idle;

    float DebugSpawnItemTime = 0;
    

    // Start is called before the first frame update
    public void Start()
    {
        LoadLevelObjects();
    }

    void LoadLevelObjects(){
        thatGirl = Instantiate(levelPrefabList.ThatGirl).GetComponent<ThatGirl>();

        phone = Instantiate(levelPrefabList.Phone).GetComponent<Phone>();
        phone.levelManager = this;
        phone.Content = levelObjects.ContentImage;
        phone.MainCamera = levelObjects.MainCamera;
        phone.UICanvas = levelObjects.UICanvas;

        attackButton = Instantiate(levelPrefabList.UIAttackButton , parent : levelObjects.UICanvas.transform).GetComponent<AttackButton>();
        attackButton.levelManager = this;
    }

    public void OnMatchSuccess(ProjectNetWork.MessageMatchSuccess messageMatchSuccess){
        nowState = GameState.InGame;
        phone.StartNewGame();
    }

    public void OnSpawnItem(ProjectNetWork.MessageSpawnItem messageSpawnItem){
        EventCenter.Instance.EventTrigger(EventCenterType.SpawnItemByID , messageSpawnItem.ItemID);
    }




    // Update is called once per frame
    void Update()
    {
        playerInfoText.text = string.Format("name:{0}\nmail:{1}\nMoney:{2}",playerInfo.name,playerInfo.mail,playerInfo.HP);
        enemyInfoText.text = string.Format("name:{0}\nmail:{1}\nMoney:{2}",enemyInfo.name,enemyInfo.mail,enemyInfo.HP);

        attackButton.AttackTimeText.text = string.Format("T：{0}s"  , playerInfo.AttackTimes);

        if(Input.GetKeyDown(KeyCode.Q)){
            if(playerInfo.AttackTimes > 0){
                playerInfo.AttackTimes--;
                thatGirl.GetComponent<SpriteRenderer>().color = Color.blue;
                enemyInfo.HP -= 10;
                Invoke("AA",.3f);
            }
        }

        if(Time.realtimeSinceStartup - DebugSpawnItemTime > 2){
            ProjectNetWork.MessageSpawnItem m = new ProjectNetWork.MessageSpawnItem();
            m.ItemID = (int)( UnityEngine.Random.Range(0,3.9f) );
            OnSpawnItem( m );

            DebugSpawnItemTime = Time.realtimeSinceStartup;
        }
    }

    public void Attack(){

    }
}
