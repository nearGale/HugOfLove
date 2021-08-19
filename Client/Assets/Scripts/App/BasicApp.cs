using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AppState
{
    StartPage,
    End,
    Runing
}

public class BasicApp : MonoBehaviour
{
    public string AppName = "BasicApp";
    public AppState nowState = AppState.StartPage;
    public Camera AppCamera = null;
    public Phone myphone = null;


    // Start is called before the first frame update
    void Start()
    {
        nowState = AppState.StartPage;
    }

    public virtual void StartApp(){

    }

    public virtual void ShutApp(){

    }

    public virtual void KillApp(){
        Destroy(gameObject);
    }

    public virtual void HideApp(){
        myphone.LockPhone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
