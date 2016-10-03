using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SerializedColor
{
    [SerializeField]
    private float r;

    public float R
    {
        get
        {
            return r;
        }
    }

    [SerializeField]
    private float g;

    public float G
    {
        get
        {
            return g;
        }
    }

    [SerializeField]
    private float b;

    public float B
    {
        get
        {
            return b;
        }
    }

    [SerializeField]
    private float a;

    public float A
    {
        get
        {
            return a;
        }
    }

    public Color ThisColor
    {
        get
        {
            return new Color(r, g, b, a);
        }

        set
        {
            r = value.r;
            g = value.g;
            b = value.b;
            a = value.a;
        }
    }

    public SerializedColor()
    {
        r = 0.0f;
        g = 0.0f;
        b = 0.0f;
        a = 0.0f;
    }

    public SerializedColor(Color toSet)
    {
        r = toSet.r;
        g = toSet.g;
        b = toSet.b;
        a = toSet.a;
    }
}
