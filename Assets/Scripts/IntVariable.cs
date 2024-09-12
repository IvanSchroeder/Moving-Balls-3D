using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewIntVariable", menuName = "Assets/Scriptable Objects/Variables/New Int Variable")]
public class IntVariable : ScriptableObject {
    [SerializeField, GetSet("value")]
    private int _value;

    public int value {
        get { return _value; }
        set { _value = value; }
    }
}