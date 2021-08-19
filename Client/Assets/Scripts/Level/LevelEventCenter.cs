using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EventCenter
{
    // key —— 时间名字
    // value —— 关心这个事件的委托方法们
    public Dictionary<string, UnityAction> eventsDictonary = new Dictionary<string, UnityAction>();

    /// <summary>
    /// 给关心这个事件的方法提供监听（订阅）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    public void EventAddListener(string eventName, UnityAction action)
    {
        // 存在存在要监听的事件
        if (eventsDictonary.ContainsKey(eventName))
        {
            eventsDictonary[eventName]+= action;
        }
        // 要监听的事件不存在,添加对应事件和订阅后执行的方法
        else
        {
            eventsDictonary.Add(eventName,action);
        }
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="EventName">触发的事件名字</param>
    public void EventTrigger(string eventName)
    {
        // 存在存在要监听的事件,依次执行订阅了的委托方法
        if (eventsDictonary.ContainsKey(eventName))
        {
            // eventsDictonary[eventName]();
            eventsDictonary[eventName].Invoke();
        }
    }
}

public class EventMono : MonoBehaviour{
    public EventCenter eventCenter = null;
    virtual public void Start() {
        eventCenter = GameObject.FindGameObjectWithTag("LevelEventCenter").GetComponent<LevelEventCenter>().eventCenter;
        Debug.Log(eventCenter);
    }
}

public class LevelEventCenter : MonoBehaviour
{
    public EventCenter eventCenter = null;
    // Start is called before the first frame update
    void Start()
    {
        eventCenter =  new EventCenter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
