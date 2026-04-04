using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkin : MonoBehaviour
{
    public SpriteRenderer image;
    public Sprite[] Sprite;
    public int spriteno;
    public ParticleSystem trail;

    void Awake()
    {
        if (image == null)
        {
            image = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        int selected = Mathf.Clamp(PlayerPrefs.GetInt("selectedOption", 0), 0, Sprite.Length - 1);
        if (image != null && Sprite != null && Sprite.Length > 0)
        {
            image.sprite = Sprite[selected];
        }

        if (trail != null)
        {
            trail.gameObject.SetActive(PlayerPrefs.GetInt("Trail", 0) > 0);
        }
    }
}
