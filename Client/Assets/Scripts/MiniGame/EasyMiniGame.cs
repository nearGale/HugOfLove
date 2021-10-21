using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EasyMiniGame : BasicMiniGame
{
    public Button ButtonEasySuccess;
    List<int> CharList = new List<int>();
    public int CodeLength = 10;
    List<int> InputList = new List<int>();
    bool CanInput = true;
    public Text TextCharList;
    public Text TextInputList;
    public override void Start()
    {
        base.Start();
        for (int i = 0; i < CodeLength; i++)
        {
            CharList.Add(
                (int)(Random.Range(65 , 90))
            );
        }
        Debug.Log(CharList);
    }

    private KeyCode GetKeyCode()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    return k;
                }
            }
        }
        return KeyCode.None;
    }


    public override void Update()
    {
        KeyCode keyCode = GetKeyCode();
        if (keyCode == KeyCode.None){
            return;
        }else{
            Debug.Log(123);
            KeyCode currentKey = keyCode;
            Debug.Log("Current Key is : " + currentKey.ToString());
            InputList.Add((int)(currentKey));
            if(CharList[ InputList.Count - 1 ] == InputList[ InputList.Count - 1 ] ){
                if(CharList.Count == InputList.Count)   Success();
            }else{
                InputList = new List<int>();
            }
        }
    

        string a = "";
        foreach (var item in CharList)
        {
            a = a + (char)item;
        }
        TextCharList.text = a;
        Debug.Log(a);

        //Debug.Log(a);

        string b = "";
        foreach (var item in InputList)
        {
            b = b + (char)item;
        }
        TextInputList.text = b;

    }
    public override void Success(){
        base.Success();
        QuitMiniGame();
    }
}
