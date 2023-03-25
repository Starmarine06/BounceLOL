using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSkin : MonoBehaviour
{
    public Image image;
    public Sprite[] Sprite;
    public int spriteno;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Image>().sprite = Sprite[PlayerPrefs.GetInt("Char")];
    }
}
