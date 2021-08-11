using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonObject : BasicAppObject
{
    Button myButton = null;
    public override void OnAppClick()
    {
        base.OnAppClick();
        myButton.onClick.Invoke();
    }
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
