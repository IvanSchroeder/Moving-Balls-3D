using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private FloatReference currentTime;
    [SerializeField] private FloatReference bestTime;
    [SerializeField] private LevelData levelData;

    [Header("Timer Settings")]
    [SerializeField] private bool activateTimer;
    [SerializeField] private bool countDown;

    [Header("Limit Settings")]
    [SerializeField] private bool hasLimit;
    [SerializeField] private FloatReference timerLimit;

    public static event Action OnTimerEnd;
    public static event Action OnNewPersonalBest;

    private void OnEnable() {
        LevelManager.OnLevelStart += SetLevelData;

        SlotsManager.OnFirstMove += StartTimer;
        SlotsManager.OnBallsSorted += PauseTimer;
        SlotsManager.OnBallsSorted += CheckPersonalBestTimer;

        OnTimerEnd += PauseTimer;
    }

    private void OnDisable() {
        LevelManager.OnLevelStart -= SetLevelData;

        SlotsManager.OnFirstMove -= StartTimer;
        SlotsManager.OnBallsSorted -= PauseTimer;
        SlotsManager.OnBallsSorted -= CheckPersonalBestTimer;

        OnTimerEnd -= PauseTimer;
    }

    private void Start() {
        Application.targetFrameRate = 60;
        //Screen.SetResolution(720, 1080, true);
        currentTime.ResetValue();
        bestTime.SetValueInInspector();
        timerLimit.SetValueInInspector();

        bestTime.value = PlayerPrefs.GetFloat("BestTime");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (levelData.hasTimer) {
            if (activateTimer) {
                currentTime.value = countDown ? currentTime.value - Time.deltaTime : currentTime.value + Time.deltaTime;
                currentTime.SetValueInInspector();
    
                if (hasLimit && ((countDown && currentTime.value <= timerLimit) || (!countDown && currentTime.value >= timerLimit))) {
                    currentTime.value = timerLimit;
                    OnTimerEnd?.Invoke();
                }
            }
        }
    }

    private void CheckPersonalBestTimer() {
        if (levelData.hasTimer && levelData.hasBestTimer) {
            if (bestTime.value == 0f || currentTime.value < bestTime.value) {
                bestTime.value = currentTime.value;
                bestTime.SetValueInInspector();
                PlayerPrefs.SetFloat("BestTime", bestTime.value);
                PlayerPrefs.Save();
                OnNewPersonalBest?.Invoke();
            }
        }
    }

    public void ResetPersonalBestTime() {
        bestTime.ResetValue();
        OnNewPersonalBest?.Invoke();
    }

    private void StartTimer() {
        activateTimer = true;
    }

    private void PauseTimer() {
        activateTimer = false;
    }

    private void SetLevelData(LevelData _currentLevel) {
        levelData = _currentLevel;
    }
}
