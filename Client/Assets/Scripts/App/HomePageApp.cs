using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HomePageApp : BasicApp
{
    public InputField nameInputField = null;
    public InputField mailInputField = null;
    public PlayerInfo playerInfo = null;

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
    }

}
