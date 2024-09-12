using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBall", menuName = "Assets/Scriptable Objects/New Ball")]
public class BallType_SO : ScriptableObject
{
    public GameObject ballPrefab;
    
    public int ballID;
    public Color color;
    public string ballColor;
}