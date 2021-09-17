using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PhoneInGame{
    public Image ImageItemPic;
    public Text TextItemPrice;
    public Text TextItemDesc;
    public Text TextNowMoney;
    public Text TextBuyCountDown;
    public Button ButtonBuy;
    public Button ButtonShutScreen;
    public Image ImageShutMask;
}

[System.Serializable]
public struct AttackButtonInGame{
    public Button ButtonAttack;
    public Text TextAttackTime;
}
[System.Serializable]
public struct DebugInGame{
    public InputField InputFieldPlayerName;
    public InputField InputFieldPlayerMail;
    public Text TextNowStat;
    public Text TextDebugInfo;
    public Button ButtonTryMatch;
    public Button ButtonTryConnect;
}

public class UIManagerNetTest : MonoBehaviour
{
    public PhoneInGame PlayerPhone = new PhoneInGame();
    public PhoneInGame EnemyPhone = new PhoneInGame();
    public AttackButtonInGame PlayerAttackButton;
    public AttackButtonInGame EnemyAttackButton;
    public DebugInGame debugInGame;
    public Image EnemyPic;
    public Image MyPic;
    public Text TextEnemyName;
    public Text TextMyName;
    public Image ImageShutMask;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
