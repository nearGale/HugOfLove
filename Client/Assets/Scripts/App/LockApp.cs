using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockApp : BasicApp
{
    // Start is called before the first frame update
    void Start()
    {
        AppName = "Lock";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnHideNowApp(){
        myphone.UnLockPhone();
    }
}
