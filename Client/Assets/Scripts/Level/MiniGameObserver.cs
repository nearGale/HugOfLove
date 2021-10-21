using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MiniGameObserver : MonoBehaviour
{
    public List<GameObject> MiniGameList;
    public BasicMiniGame nowMiniGame;
    public Canvas MiniGameCanvas;
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.Instance.EventAddListener(EventCenterType.SuccessMiniGame , OnMiniGameSuccess);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMiniGameSuccess(params object[] data){
        EventCenter.Instance.EventTrigger(EventCenterType.BlockSuccess);
    }

    public void StartRandomMiniGame(){
        GameObject o =  Instantiate(MiniGameList[0] , parent:MiniGameCanvas.transform);
        nowMiniGame = o.GetComponent<BasicMiniGame>();
    }
}
