using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EasyMiniGame : BasicMiniGame
{
    public Button ButtonEasySuccess;
    List<int> CharList = new List<int>();
    int CodeLength = 5;
    List<int> InputList = new List<int>();
    bool CanInput = true;
    public Text TextCharList;
    public Text TextInputList;
    public Animator TextAni;
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

    IEnumerator shake(int frame){
        for (float i = 0; i < frame; i++)
        {
            if(i%5 == 0){
                transform.position += new Vector3(Random.Range(-10 , 10) , Random.Range(-10 , 10) , 0);
            }
            yield return null;//表示下一帧再继续执行后面的代码
        }
    }


    public override void Update()
    {
        KeyCode keyCode = GetKeyCode();
        if (keyCode == KeyCode.None || CanInput == false){
            return;
        }else{
            KeyCode currentKey = keyCode;
            InputList.Add((int)(currentKey));
            if(CharList[ InputList.Count - 1 ] == InputList[ InputList.Count - 1 ]-32   ){
                StartCoroutine(shake(20));
                if(CharList.Count == InputList.Count)   {
                    TextAni.SetTrigger("beat");
                    Invoke("Success" , 0.8f);
                    CanInput = false;
                }
            }else{
                InputList = new List<int>();
                TextAni.SetTrigger("beat2");
                EventCenter.Instance.EventTrigger(EventCenterType.PlayerErrorAudio );
            }
        }
    

        string a = "";
        foreach (var item in CharList)
        {
            a = a + (char)item;
        }
        TextCharList.text = a;

        //Debug.Log(a);

        string b = "";
        foreach (var item in InputList)
        {
            b = b + (char)item;
        }
        TextInputList.text = b.ToUpper();

    }
    public override void Success(){
        Debug.Log("sc");
        base.Success();
        QuitMiniGame();
    }
}
