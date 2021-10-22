using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManagerResult : MonoBehaviour
{
    public Text r;
    // Start is called before the first frame update
    void Start()
    {
        r.text = LevelManagerNetTest.Instance.ResultWin ? "胜利" : "失败" ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart(){
        SceneManager.LoadScene("HomePage");
    }
}
