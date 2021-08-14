using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager levelManager = null;
    public Text AttackTimeText = null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(){
        levelManager.Attack();
    }
}
