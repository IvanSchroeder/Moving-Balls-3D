using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Assets/Scriptable Objects/Levels/New Level Data")]
public class LevelData : ScriptableObject {
    public GameObject levelLayout;
    public BallType_SO firstBallType;
    public BallType_SO secondBallType;
    public int maxFirstBallType;
    public int maxSecondBallType;
    public int slotsCount;
    public int middleSlotsAmount;
    public bool isRandom;
    public bool isSideDependant;
    public bool hasTimer;
    public bool hasBestTimer;
    public float bestTime;

    public Color backgroundCameraColor;
}
