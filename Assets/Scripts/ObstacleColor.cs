using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleColor : MonoBehaviour
{
    public static Color BaseColor;

    private SpriteRenderer sr;

    void Start()
    {
        if (transform.childCount > 0)
        {
            sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (sr != null && sr.color != BaseColor)
        {
            sr.color = BaseColor;
        }
    }
}
