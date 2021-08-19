using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public Transition myTransition = null;
    bool Transiable = true;


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

    public virtual void TransApp(BasicApp nextApp , UnityAction action){
        Debug.Log(Transiable);
        if(!Transiable) return;
        StartCoroutine(LoadLevel(nextApp , action));
    }

    public IEnumerator LoadLevel(BasicApp nextApp , UnityAction action){
        Debug.Log("TransStart");
        Transiable = false;        
        // 开始过场

        myTransition.StartTrans();
        nextApp.myTransition.StartTrans();

        // 等待一帧
        // 理由再下面有解释，但其实这里本来不需要，因为检查动画前还夹着一个检查加载的过程。基本不会在一帧内就加载完
        // 但是保险起见还是在播放动画后延迟一帧
        yield return null;

        // 等待动画播放完成
        while(!myTransition.IsAnimationDone())
            yield return null;
        
        myphone.SwitchNewAppByInstance(nextApp);
        // 结束过场
        myTransition.EndTrans();
        nextApp.myTransition.EndTrans();

        // 等待一帧
        // 因为我发现如果在开始动画后不等待一帧的话，第二个动画其实还没开始播放，
        // 后面检测动画完成检测的就是第一个动画，就起不到检测第二个动画的作用。
        yield return null;

        // 等待动画播放完成
        while(!myTransition.IsAnimationDone())
            yield return null;

        print("Done");
        Transiable = true;
        action.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
