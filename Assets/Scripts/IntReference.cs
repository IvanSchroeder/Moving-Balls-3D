using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class IntReference {
    public IntVariable Variable;
    public IntVariable InitialVariable;

    public bool useConstant = false;

    [SerializeField, GetSet("value")] private int _value;
    public int value {
        get { return useConstant ? constantValue : Variable.value; }
        set { 
            Variable.value = value; 
            _value = Variable.value;
        }
    }

    [SerializeField, GetSet("constantValue")] private int _constantValue;
    public int constantValue {
        get { return _constantValue; }
        set { _constantValue = value; }
    }

    //[SerializeField] private float _initialValue;
    /*public float initialValue {
        get { return _initialValue; }
        set { _initialValue = value; }
    }*/

    public void SetValueInInspector() {
        value = Variable.value;
    }

    public void ResetValue() {
        value = InitialVariable.value;
        SetValueInInspector();
    }

    public static implicit operator float(IntReference reference) {
        return reference.value;
    }
}