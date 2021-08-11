using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PullApp : BasicApp
{
    public Slider BeatSlider = null;
    public SpriteRenderer Rope = null;
    public float BeatValue = .5f;
    public float BeatCycleTime = 3f;

    private float CycleStatTime = 0f;
    private PullLaObject myLaObject = null;

    public float PullCD = 3f;
    private float nowPullCD = 0f;

    private float VSTargetValue = 0f;
    private float VSPosValue = 0f;
    public float VSRopeLength = 4f;

    public bool PullDebug = true;

    //public float VSMoveSpeed = 5f;
    public float VSEase = 1f;
    public float minVSMove = .2f;

    float debugtime1 = 0f;
    bool debugStart = false;


    // Start is called before the first frame update
    void Start()
    {
        AppName = "PullApp";

        myLaObject = GetComponentInChildren<PullLaObject>();
        myLaObject.myPullApp = this;
    }

    // Update is called once per frame
    void Update()
    {
        BeatValue = DoCycle((Time.time - CycleStatTime)%BeatCycleTime);

        DoRopePos();

        
        if(nowState == AppState.Runing){
            BeatSlider.value = BeatValue;

            if(nowPullCD >= 0)  nowPullCD -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.W)){
            VSTargetValue-=getVSvalueByDeviation(BeatValue - 0.5f);
        }

        if(Input.GetKeyDown(KeyCode.Z)){
            debugStart = true;
        }

        if(Input.GetKeyDown(KeyCode.X)){
            debugStart = false;
        }

        if(Time.realtimeSinceStartup - debugtime1 > 2.5f && debugStart){
            debugtime1 = Time.realtimeSinceStartup;
            VSTargetValue-=getVSvalueByDeviation(0f);
        }

        if(VSPosValue > 1){
            //VSPosValue = 0;
            VSTargetValue = 0;
            myphone.WinGame(1);
        }

    }

    float DoCycle(float nowTime){
        float a = .5f;
        if(nowTime < BeatCycleTime / 2){
            a = -(8/(BeatCycleTime*BeatCycleTime))*Mathf.Pow( (nowTime - BeatCycleTime/4) , 2) + 1f;
        }else{
            a = (8/(BeatCycleTime*BeatCycleTime))*Mathf.Pow( (nowTime - BeatCycleTime*3/4) , 2);
        }
        return a;
    }

    void DoRopePos(){
        float x = Mathf.Abs(VSTargetValue - VSPosValue) >= minVSMove ? (VSTargetValue - VSPosValue) : 0;
        float speedx = VSEase * x * x * (x>0?1:-1);
        VSPosValue += speedx * Time.deltaTime;
        Vector3 Pos = Rope.transform.position;
        Pos.x += speedx * Time.deltaTime * VSRopeLength;
        Rope.transform.position = Pos;
        
        //Debug.Log("Target" + VSTargetValue);
        //Debug.Log("Pos" + VSPosValue);
    }

    public float getVSvalueByDeviation(float d){
        float v = 0f;
        d = Mathf.Abs(d);
        v = 0.5f - d;
        return v;
    }

    public void ClickPull(){
        if(nowPullCD > 0)   return;
        nowPullCD = PullCD;
        //Debug.Log("偏差值：" + (BeatValue - 0.5f).ToString());
        VSTargetValue+=getVSvalueByDeviation(BeatValue - 0.5f);
    }

    public override void StartApp()
    {
        base.StartApp();
        CycleStatTime = Time.time;
        nowState = AppState.Runing;
    }
}
