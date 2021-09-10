using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectNetWork;
public class LevelManager2in1 : MonoBehaviour
{
    public Phone phone_left = null;
    public Phone phone_right = null;

    public PlayerInfo Info_left = new PlayerInfo();
    public PlayerInfo Info_right = new PlayerInfo();

    float DebugSpawnItemTime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.Instance.EventAddListener(EventCenterType.MatchSuccess,OnMatchSuccess);
        EventCenter.Instance.EventAddListener(EventCenterType.Attacked , OnAttacked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMatchSuccess(params object[] data){
    }

    public void OnSpawnItem(params object[] data){
       
    }

    public void OnAttacked(params object[] data){
        
    }
}
