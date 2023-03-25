using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkin : MonoBehaviour
{
    public SpriteRenderer image;
    public Sprite[] Sprite;
    public int spriteno;
    public ParticleSystem trail;

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<SpriteRenderer>().sprite = Sprite[PlayerPrefs.GetInt("selectedOption")];
        if (PlayerPrefs.GetInt("trail") == 1)
        {
            trail.gameObject.active = true;
        }
    }
}
