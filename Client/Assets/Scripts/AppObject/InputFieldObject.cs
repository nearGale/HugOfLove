using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldObject : BasicAppObject
{
    InputField myInput = null;
    // Start is called before the first frame update
    void Start()
    {
        myInput = GetComponent<InputField>();
    }

    public override void OnAppClick()
    {
        base.OnAppClick();
        myInput.ActivateInputField();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
