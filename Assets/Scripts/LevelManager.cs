using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class LevelManager : MonoBehaviour {
    [SerializeField] private List<LevelData> LevelsDataList = new List<LevelData>();
    [SerializeField] private List<LevelData> RandomLevelsDataList = new List<LevelData>();
    [SerializeField] private LevelData currentLevelData;
    [SerializeField] private LevelData currentRandomLevelData;
    [SerializeField] private LevelData _nextRandomLevelData;
    private static LevelData nextRandomLevelData;
    [SerializeField] private GameObject currentLayout;
    [SerializeField] private IntReference currentLevelCount;

    public static event Action<LevelData> OnLevelStart;
    public static event Action<int> OnLevelUpdate;

    private bool isUserCompleteLevel;
    private int score = 100;

    private void OnEnable() {
        SlotsManager.OnSlotsCreated += SendLevelData;
    }

    private void OnDisable() {
        SlotsManager.OnSlotsCreated -= SendLevelData;
    }

    private void Awake() {
        LoadCurrentLevel();
    }

    private void Start() {
        currentLevelCount.SetValueInInspector();
        OnLevelUpdate?.Invoke(currentLevelCount.value);
        OnLevelStart?.Invoke(currentLevelData);

        currentLayout = GetLevelLayout(currentLevelData);
        SpawnLevelLayout();

        //SetCameraBackground();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            currentLevelCount.value = 0;
            PlayerPrefs.SetInt("CurrentLevel", currentLevelCount.value);
            PlayerPrefs.Save();
            Debug.Log("Restarted Levels");
            RestartScene();
        }
    }

    private void LoadCurrentLevel() {
        currentLevelCount.value = PlayerPrefs.GetInt("CurrentLevel");
        TinySauce.OnGameStarted("Level " + (currentLevelCount.value + 1).ToString());
        //Load from playerprefs
        if (currentLevelCount.value < LevelsDataList.Count - 1) {
            currentLevelData = LevelsDataList[currentLevelCount.value];
        }
        else {
            currentLevelData = LevelsDataList[LevelsDataList.Count - 1];
            SetCurrentRandomLevelData();
            SetNextRandomLevelData();
        }
    }

    private void SetCurrentRandomLevelData() {
        if (nextRandomLevelData != null) {
            currentRandomLevelData = nextRandomLevelData;
        }
        else {
            currentRandomLevelData = Utils.GetRandomElement(RandomLevelsDataList);
        }
    }

    private void SetNextRandomLevelData() {
        nextRandomLevelData = Utils.GetRandomElement(RandomLevelsDataList);
        while (nextRandomLevelData == currentRandomLevelData) {
            nextRandomLevelData = Utils.GetRandomElement(RandomLevelsDataList);
        }
        _nextRandomLevelData = nextRandomLevelData;
    }

    private void SpawnLevelLayout() {
        Instantiate(currentLayout, transform.position, currentLayout.transform.rotation);
    }

    public void ContinueNextLevel() {
        currentLevelCount.value++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevelCount.value);
        PlayerPrefs.Save();
        TinySauce.OnGameFinished(isUserCompleteLevel, score, "Level " + (currentLevelCount.value + 1).ToString());
        Utils.RestartScene();
    }

    public void RestartScene() {
        if (nextRandomLevelData != null) {
            nextRandomLevelData = currentRandomLevelData;
            _nextRandomLevelData = nextRandomLevelData;
        }
        Utils.RestartScene();
    }

    private GameObject GetLevelLayout(LevelData levelData) {
        GameObject layout = currentLevelData.isRandom ? currentRandomLevelData.levelLayout : levelData.levelLayout;
        return layout;
    }

    private void SendLevelData() {
        OnLevelStart?.Invoke(currentLevelData);
    }

    private void SetCameraBackground() {
        Camera mainCamera = Utils.GetMainCamera();
        mainCamera.backgroundColor = currentLevelData.isRandom ? currentRandomLevelData.backgroundCameraColor : currentLevelData.backgroundCameraColor;
    }
}