using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemDisplayBox : MonoBehaviour
{
    public Text ItemDescriptionText = null;
    public RawImage ItemRawImage = null;
    public void ShowRawItem(BuyItem buyItem){
        ItemDescriptionText.text = buyItem.description;
        ItemRawImage.texture = buyItem.Texture;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
