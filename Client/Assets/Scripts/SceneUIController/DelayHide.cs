using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayHide : MonoBehaviour
{
    public float TimeToHide;

    private float m_LeftTime;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        m_LeftTime = TimeToHide;
    }

    void Update()
    {
        m_LeftTime -= Time.deltaTime;
        if(m_LeftTime <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
