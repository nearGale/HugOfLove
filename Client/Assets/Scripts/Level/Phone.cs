using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public enum AppPhoneState{
    Active,
    Hide,
    Die
}
[Serializable]
public class AppByName{
    public string name = "AppName";
    public GameObject appObject = null;
    public AppPhoneState state = AppPhoneState.Die;
    public BasicApp aliveApp = null;
    public int AppTargetPos = 0;

}

public class Phone : MonoBehaviour
{
    public RawImage Content = null;
    public Camera MainCamera = null;
    public Canvas UICanvas = null;

    public GameObject InitApp  = null;

    private RectTransform ContentRect = null;
    private PhoneRenderTexture ContentTexture = null;
    private RectTransform UICanvasRect = null;

    public BasicApp NowApp = null;
    public BasicApp LockApp = null;
    private BasicApp FormerApp = null;


    public PlayerInfo playerInfo = null;

    public List<AppByName> AppByNames = new List<AppByName>();
    public LevelManager levelManager = null;
    private Stack<AppByName> AppHistory = new Stack<AppByName>();
    
    public void SwitchNewAppByName(string name){
        Debug.Log("try active:"+name);
        foreach(AppByName item in AppByNames){
            if (item.name == name){
                if(item.state == AppPhoneState.Hide){
                    Debug.Log("Hide active:"+name);
                    NowApp = item.aliveApp;
                    ContentTexture.PreviewCamera = NowApp.AppCamera;
                    Content.texture = NowApp.AppCamera.targetTexture;

                    foreach(AppByName z in AppByNames){
                        if(z.state == AppPhoneState.Active)    z.state = AppPhoneState.Hide;
                    }
                    item.state = AppPhoneState.Active;
                    AppHistory.Push(item);
                }else if(item.state == AppPhoneState.Die){
                    Debug.Log("Die active:"+name);
                    GameObject appObject =  Instantiate(item.appObject , new Vector3(100f + item.AppTargetPos*100 , 100f ,0) , new Quaternion());
                    if(appObject.GetComponent<BasicApp>() == null){
                        Debug.LogError("wrong app prefab"+item.appObject);
                        return;
                    }
                    NowApp = appObject.GetComponent<BasicApp>();
                    ContentTexture.PreviewCamera = NowApp.AppCamera;
                    Content.texture = NowApp.AppCamera.targetTexture;

                    NowApp.myphone = this;
                    NowApp.StartApp();

                    foreach(AppByName z in AppByNames){
                        if(z.state == AppPhoneState.Active)    z.state = AppPhoneState.Hide;
                    }
                    item.state = AppPhoneState.Active;
                    item.aliveApp = NowApp;
                    AppHistory.Push(item);
                }else if(item.state == AppPhoneState.Active){
                    Debug.Log("APP Already Acitved");
                }
                return;
            }
        }
        Debug.LogError("APP NOT FOUND:"+name);
        return;
    }
    public void LockPhone(){
        SwitchNewAppByName("LockApp");
    }
    public void UnLockPhone(){
        AppHistory.Pop();
        SwitchNewAppByName(AppHistory.Peek().name);
    }
    public void WinGame(float point){
        levelManager.playerInfo.AttackTimes++;
    }

    public void UnHideNowApp(){
        SwitchNewAppByName(FormerApp.AppName);
    }

    public void TryStartGame(){
        levelManager.OnMatchSuccess(new ProjectNetWork.MessageMatchSuccess());
    }

    
    void Start()
    {
    
        UICanvasRect = UICanvas.GetComponent<RectTransform>();
        ContentRect = Content.GetComponent<RectTransform>();
        ContentTexture = Content.GetComponent<PhoneRenderTexture>();

        int i = 0;
        foreach (AppByName item in AppByNames)
        {
            item.name = item.appObject.name;
            item.AppTargetPos = i++;

            SwitchNewAppByName(item.name);
        }

        SwitchNewAppByName(InitApp.gameObject.name);
    }

    void UpdateContentPos(){
        Vector3 ContentScreenPoint =  MainCamera.WorldToScreenPoint(transform.position);
        //Vector3 ContentPos = new Vector3(ContentViewport.x-0.5f , ContentViewport.y-0.5f , ContentViewport.z);
        Vector2 ContentPos = new Vector2();

        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvasRect, ContentScreenPoint , MainCamera , out ContentPos);
        if(isRect){
            ContentRect.anchoredPosition = ContentPos;
            ContentRect.localScale = transform.localScale;
            ContentRect.rotation = transform.rotation;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateContentPos();
    }
}
