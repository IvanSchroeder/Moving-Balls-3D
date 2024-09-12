using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScreens : MonoBehaviour {
    [SerializeField] private GameObject WinScreen = null;
    [SerializeField] private GameObject LevelLabel = null;
    [SerializeField] private GameObject TimerLabel = null;
    [SerializeField] private GameObject RestartButton = null;
    [SerializeField] private float delaySeconds;
    private bool result;

    private void OnEnable() {
        SlotsManager.OnBallsSorted += LevelWin;
    }

    private void OnDisable() {
        SlotsManager.OnBallsSorted -= LevelWin;
    }

    private void LevelWin() {
        result = true;
        LevelFinished(result);
    }

    public void LevelFinished(bool result) {
        if (result) {
            WinScreen.SetActive(true);
            LevelLabel.SetActive(true);
            TimerLabel.SetActive(true);
            RestartButton.SetActive(true);
            LeanTween.scale(WinScreen, Vector3.one, 0.5f).setEaseOutBounce().setDelay(delaySeconds);
            LeanTween.scale(LevelLabel, Vector3.zero, 0.5f).setEaseInBack();
            LeanTween.scale(TimerLabel, Vector3.zero, 0.5f).setEaseInBack();
            LeanTween.scale(RestartButton, Vector3.zero, 0.5f).setEaseInBack();
        }
    }
}
