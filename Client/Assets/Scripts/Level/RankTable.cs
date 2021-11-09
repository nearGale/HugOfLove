using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankTable : MonoBehaviour
{
    public List<GameObject> Rows;
    public GameObject RowItem;
    // Start is called before the first frame update
    void Start()
    {
        Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddLine(object f1 , object f2 , object f3){
        GameObject a =  Instantiate(RowItem , transform);
        a.transform.Find("C1").GetComponent<Text>().text = f1.ToString();
        a.transform.Find("C2").GetComponent<Text>().text = f2.ToString();
        a.transform.Find("C3").GetComponent<Text>().text = f3.ToString();

        Rows.Add(a);
    }

    public void Clear(){
        foreach (var item in Rows)
        {
            Destroy(item);
        }
        Rows = new List<GameObject>();
    }
}
