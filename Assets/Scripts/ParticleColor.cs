using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColor : MonoBehaviour
{
    public static Color BaseColor;

    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps != null)
        {
            ParticleSystem.MainModule settings = ps.main;
            settings.startColor = new ParticleSystem.MinMaxGradient(BaseColor);
        }
    }
}