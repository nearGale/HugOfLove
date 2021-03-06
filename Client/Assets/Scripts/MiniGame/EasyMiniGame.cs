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
        CanInput= true;
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
        float a = .12f;
        Vector3 aa = transform.position;
        for (float i = 0; i < frame; i++)
        {
            if(i%5 == 0){
                transform.position += new Vector3(Random.Range(-a , a) , Random.Range(-a , a) , 0);
            }
            yield return null;//表示下一帧再继续执行后面的代码
        }
        //transform.position = aa;
    }


    public override void Update()
    {
        KeyCode keyCode = GetKeyCode();
        if (keyCode == KeyCode.None || CanInput == false){
            //return;
        }else{
            KeyCode currentKey = keyCode;
            InputList.Add((int)(currentKey));

            //匹配
            if(CharList[ InputList.Count - 1 ] == InputList[ InputList.Count - 1 ]-32 || CharList[ InputList.Count - 1 ] == InputList[ InputList.Count - 1 ]){
                StartCoroutine(shake(20));
            }else{
                InputList = new List<int>();
                TextAni.SetTrigger("beat2");
                EventCenter.Instance.EventTrigger(EventCenterType.PlayerErrorAudio );
            }
        }

        if(CharList.Count <= InputList.Count && CanInput)   {
            TextAni.SetTrigger("beat");
            Invoke("Success" , 0.8f);
            CanInput = false;
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

        Debug.Log(InputList);
        Debug.Log(TextCharList);

    }
    public override void Success(){
        Debug.Log("sc");
        base.Success();
        QuitMiniGame();
    }
}
