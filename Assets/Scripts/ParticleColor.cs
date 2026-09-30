using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColor : MonoBehaviour
{
    public static Color BaseColor;

    private ParticleSystem ps;
    private Color appliedColor;
    private bool hasApplied;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps != null && (!hasApplied || appliedColor != BaseColor))
        {
            ParticleSystem.MainModule settings = ps.main;
            settings.startColor = new ParticleSystem.MinMaxGradient(BaseColor);
            appliedColor = BaseColor;
            hasApplied = true;
        }
    }
}