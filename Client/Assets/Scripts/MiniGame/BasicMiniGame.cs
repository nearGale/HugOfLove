using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMiniGame : MonoBehaviour
{
    public virtual void Start()
    {
        EventCenter.Instance.EventAddListener(EventCenterType.QuitMiniGame , QuitMiniGame);
    }

    public virtual void Update()
    {
        
    }
    public virtual void Success(){
        EventCenter.Instance.EventTrigger(EventCenterType.SuccessMiniGame);
    }
    public virtual void QuitMiniGame(params object[] data){
        Debug.Log("Destroy mini");
        GameObject.Destroy(gameObject);
    }
}
