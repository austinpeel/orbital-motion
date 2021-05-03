using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawVector : MonoBehaviour
{
    [Range(0, 0.5f)]
    public float lineWidth = 0.05f;
    public float headLength;
    public LineRenderer headLR;

    private LineRenderer bodyLR;

    private void Awake()
    {
        // For drawing vectors
        bodyLR = GetComponent<LineRenderer>();
        bodyLR.startWidth = lineWidth;
        bodyLR.endWidth = bodyLR.startWidth;
        bodyLR.startColor = Color.white;
        bodyLR.endColor = bodyLR.startColor;
        bodyLR.positionCount = 0;

        if (headLR != null)
        {
            headLR.startWidth = bodyLR.startWidth;
            headLR.endWidth = bodyLR.endWidth;
            headLR.startColor = bodyLR.startColor;
            headLR.endColor = bodyLR.endColor;
        }
    }

    public void SetTailPosition(Vector3 position)
    {
        if (bodyLR.positionCount < 1)
        {
            bodyLR.positionCount = 1;
        }
        bodyLR.SetPosition(0, position);
    }

    public void SetHeadPosition(Vector3 position)
    {
        if (bodyLR.positionCount < 2)
        {
            bodyLR.positionCount = 2;
        }
        bodyLR.SetPosition(1, position);

        // Draw the arrowhead
        if (headLR != null)
        {
            headLR.positionCount = 3;
            Vector3 body = GetValue();
            Vector3 rotated90 = new Vector3(-body.y, body.x, 0);
            headLR.SetPosition(0, bodyLR.GetPosition(1) + headLength * (rotated90 - body).normalized);
            headLR.SetPosition(1, bodyLR.GetPosition(1));
            headLR.SetPosition(2, bodyLR.GetPosition(1) - headLength * (rotated90 + body).normalized);
        }
    }

    public Vector3 GetValue()
    {
        return (bodyLR.positionCount == 2) ? bodyLR.GetPosition(1) - bodyLR.GetPosition(0) : Vector3.zero;
    }

    public void Reset()
    {
        bodyLR.positionCount = 0;
        if (headLR != null)
        {
            headLR.positionCount = 0;
        }
    }
}
