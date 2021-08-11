using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullLaObject : BasicAppObject
{
    public PullApp myPullApp = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnAppClick()
    {
        base.OnAppClick();
        Debug.Log("拉！！！！！");
        if(myPullApp!= null){
            myPullApp.ClickPull();
        }
    }
}
