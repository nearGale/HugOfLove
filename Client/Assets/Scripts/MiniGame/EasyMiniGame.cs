using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EasyMiniGame : BasicMiniGame
{
    public Button ButtonEasySuccess;
    public override void Start()
    {
        base.Start();
        ButtonEasySuccess.onClick.AddListener(Success);
    }
    public override void Success(){
        base.Success();
        QuitMiniGame();
    }
}
