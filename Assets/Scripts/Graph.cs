using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public RectTransform rt;
    public LineRenderer xAxis;
    public LineRenderer yAxis;
    public LineRenderer line;
    [Range(0, 1)]
    public float x0;
    [Range(0, 1)]
    public float y0;
    [Min(2)]
    public int samples;

    private const float initialSolarMass = 1000f;
    private float currentSolarMass;

    private void Start()
    {
        SetOrigin(x0, y0);
        currentSolarMass = initialSolarMass;
        Plot(samples);
    }

    private void Plot(int n)
    {
        line.positionCount = 0;
        float xStep = rt.rect.width / n;

        for (int i = 0; i < n; i++)
        {
            float xi = -x0 * rt.rect.width + i * xStep;
            if (xi > 0)
            {
                float yi = -currentSolarMass / xi;
                if (yi >= -y0 * rt.rect.height && yi < rt.rect.height * (1 - y0))
                {
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, new Vector3(xi + x0 * rt.rect.width, yi + y0 * rt.rect.height, 0));
                }
            }
        }
    }

    private void SetOrigin(float newX0, float newY0)
    {
        xAxis.SetPosition(0, new Vector3(0, newY0 * rt.rect.height, 0));
        xAxis.SetPosition(1, new Vector3(rt.rect.width, newY0 * rt.rect.height, 0));
        yAxis.SetPosition(0, new Vector3(newX0 * rt.rect.width, 0, 0));
        yAxis.SetPosition(1, new Vector3(newX0 * rt.rect.width, rt.rect.height, 0));
    }

    public void UpdateGraph(float newSolarMass)
    {
        currentSolarMass = 2 * newSolarMass * initialSolarMass;
        Plot(samples);
    }

    public float ComputePotential(float x)
    {
        return Mathf.Max(-currentSolarMass / x, -y0 * rt.rect.height);
    }

    //private void CreateXAxis()
    //{
    //    GameObject go = new GameObject("Axis", typeof(RectTransform), typeof(LineRenderer));
    //    go.transform.SetParent(transform);
    //    go.transform.position = transform.position;

    //    LineRenderer axis = go.GetComponent<LineRenderer>();
    //    axis.startWidth = 0.4f;
    //    axis.endWidth = axis.startWidth;
    //    axis.startColor = Color.white;
    //    axis.endColor = axis.startColor;

    //    axis.positionCount = 2;
    //    axis.SetPosition(0, new Vector3(0, Y0 * rt.rect.height, 0));
    //    axis.SetPosition(1, new Vector3(rt.rect.width, Y0 * rt.rect.height, 0));

    //    xAxis = axis;
    //}

    //private void OnValidate()
    //{
    //    SetOrigin(x0, y0);
    //    Plot(samples);
    //}
}
