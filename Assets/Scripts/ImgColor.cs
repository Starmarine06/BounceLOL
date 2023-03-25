using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgColor : MonoBehaviour
{
    public static Color BaseColor;

    void Update()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = BaseColor;
    }
}
