using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public enum EventCenterType{
    SpawnItemByID,
    MatchSuccess,
    Attacked

}

public class Single<T> where T :new ()
{
    private static T instance ;
    public static T Instance {
        get{
            if(instance ==null){
                instance = new T();
            }
            return instance;
        }
    }
}

public class EventCenter :Single<EventCenter>
{
    // key —— 时间名字
    // value —— 关心这个事件的委托方法们
    public Dictionary<EventCenterType, UnityAction<object[]>> eventsDictonary = new Dictionary<EventCenterType, UnityAction<object[]>>();

    /// <summary>
    /// 给关心这个事件的方法提供监听（订阅）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    public void EventAddListener(EventCenterType eventName, UnityAction<object[]> action)
    {
        // 存在存在要监听的事件
        if (eventsDictonary.ContainsKey(eventName))
        {
            eventsDictonary[eventName] += action;
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
    public void EventTrigger(EventCenterType eventName , params object[] data)
    {
        // 存在存在要监听的事件,依次执行订阅了的委托方法
        if (eventsDictonary.ContainsKey(eventName))
        {
            // eventsDictonary[eventName]();
            eventsDictonary[eventName].Invoke(data);
        }
    }
}