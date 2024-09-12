using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FloatReference {
    public FloatVariable Variable;
    public FloatVariable InitialVariable;

    public bool useConstant = false;

    [SerializeField, GetSet("value")] private float _value;
    public float value {
        get { return useConstant ? constantValue : Variable.value; }
        set { 
            Variable.value = value;
            _value = Variable.value;
        }
    }

    [SerializeField, GetSet("constantValue")] private float _constantValue;
    public float constantValue {
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

    public static implicit operator float(FloatReference reference) {
        return reference.value;
    }
}