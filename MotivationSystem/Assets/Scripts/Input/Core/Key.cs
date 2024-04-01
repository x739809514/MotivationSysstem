using System;
using UnityEngine;

/// <summary>
/// 3 key types
/// </summary>
public enum KeyTrigger
{
    Once,
    Double,
    Continue,
}

[Serializable]
public class Key
{
    public string name;
    public KeyTrigger trigger;
    public bool enable=true;
    [HideInInspector]
    public bool isDown;
    [HideInInspector]
    public bool isDoubleDown;
    [HideInInspector]
    public bool acceptDoubleDown;
    public float interval = 1f;
    [HideInInspector]
    public float realInterval;
    public KeyCode keyCode;

    public void SetKey(KeyCode key)
    {
        keyCode = key;
    }

    public void SetEnable(bool isEnable)
    {
        enable = isEnable;
        realInterval = 0f;
    }
}

[Serializable]
public class ValueKey
{
    public string name;
    public Vector2 range = new Vector2(0, 1);
    [HideInInspector]
    public float value;
    public float addSpeed = 1f;
    public KeyCode keyCode;
    public bool enable=true;

    public void SetKey(KeyCode key)
    {
        keyCode = key;
    }

    public void SetEnable(bool isEnable)
    {
        enable = isEnable;
        value = 0;
    }
}

[Serializable]
public class AxisKey
{
    public string name;
    public Vector2 range = new Vector2(-1, 1);
    [HideInInspector]
    public float value;
    public KeyCode posKey;
    public KeyCode negKey;
    public float addSpeed = 1f;
    public bool enable=true;

    public void SetKey(KeyCode pos, KeyCode neg)
    {
        posKey = pos;
        negKey = neg;
    }

    public void SetPosKey(KeyCode pos)
    {
        posKey = pos;
    }

    public void SetNegKey(KeyCode neg)
    {
        negKey = neg;
    }

    public void SetEnable(bool isEnable)
    {
        enable = isEnable;
        value = 0;
    }
}
