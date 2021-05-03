using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarHalo : MonoBehaviour
{
    private new Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    public void UpdateHaloRange(float range)
    {
        light.range = 1.3f * 2 * range;
    }

}
