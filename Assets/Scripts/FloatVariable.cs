using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewFloatVariable", menuName = "Assets/Scriptable Objects/Variables/New Float Variable")]
public class FloatVariable : ScriptableObject {
    [SerializeField, GetSet("value")]
    private float _value;

    public float value {
        get { return _value; }
        set { _value = value; }
    }
}
