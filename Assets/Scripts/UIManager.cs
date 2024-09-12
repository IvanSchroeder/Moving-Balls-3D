using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    [Header("Component")]
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI currentTimeLabel;
    [SerializeField] private TextMeshProUGUI currentTimeText;
    [SerializeField] private TextMeshProUGUI yourTimeLabel;
    [SerializeField] private TextMeshProUGUI yourTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeLabel;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private FloatReference currentTime;
    [SerializeField] private FloatReference bestTime;
    [SerializeField] private LevelData levelData;

    [Header("Format Settings")]
    [SerializeField] private bool hasFormat;
    [SerializeField] private TimerFormats format;
    private Dictionary<TimerFormats, string> timeFormats = new Dictionary<TimerFormats, string>();
    [SerializeField] private Color normalTimerColor;
    [SerializeField] private Color failTimerColor;
    [SerializeField] private Color bestTimeColor;

    [Header("Hand Lerping Settings")]
    [SerializeField] private GameObject handObject;
    private SpriteRenderer handSprite;
    private TrailRenderer handTrail;
    [SerializeField] private Animator handAnimator;
    [SerializeField] private float desiredDuration = 1f;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private bool lerping = false;
    private float elapsedTime;
    private Vector3 lerpTargetPosition;

    private string tempText;

    private void OnEnable() {
        LevelManager.OnLevelStart += SetLevelData;
        LevelManager.OnLevelUpdate += UpdateLevelText;

        GameManager.OnNewPersonalBest += UpdateBestTimeText;
        GameManager.OnTimerEnd += SetFailTimerColor;

        SlotsManager.OnBallsSorted += UpdateYourTimeText;

        //InputManager.OnSwipeStart += SetHandGrab;
        //InputManager.OnSwipeEnd += MoveHandSprite;

        //InputManager.OnTap += HandTapAnimation;
    }

    private void OnDisable() {
        LevelManager.OnLevelStart -= SetLevelData;
        LevelManager.OnLevelUpdate -= UpdateLevelText;

        SlotsManager.OnBallsSorted -= UpdateYourTimeText;
        
        GameManager.OnNewPersonalBest -= UpdateBestTimeText;
        GameManager.OnTimerEnd -= SetFailTimerColor;

        //InputManager.OnSwipeStart -= SetHandGrab;
        //InputManager.OnSwipeEnd -= MoveHandSprite;

        //InputManager.OnTap -= HandTapAnimation;
    }

    private void Awake() {
        handAnimator = handObject.GetComponentInChildren<Animator>();
        handSprite = handObject.GetComponentInChildren<SpriteRenderer>();
        handTrail = handObject.GetComponentInChildren<TrailRenderer>();
    }

    private void Start() {
        currentTimeLabel?.gameObject.SetActive(levelData.hasTimer);
        currentTimeText?.gameObject.SetActive(levelData.hasTimer);
        bestTimeLabel?.gameObject.SetActive(levelData.hasTimer && levelData.hasBestTimer);
        bestTimeText?.gameObject.SetActive(levelData.hasTimer && levelData.hasBestTimer);
        yourTimeLabel?.gameObject.SetActive(levelData.hasTimer);
        yourTimeText?.gameObject.SetActive(levelData.hasTimer);

        if (levelData.hasTimer) {
            timeFormats.Add(TimerFormats.Whole, "00");
            timeFormats.Add(TimerFormats.TenthDecimal, "00.0");
            timeFormats.Add(TimerFormats.HundrethDecimal, "00.00");
            UpdateTimerText();
            UpdateBestTimeText();
            UpdateYourTimeText();
            SetTimerColor(currentTimeText, normalTimerColor);
            SetTimerColor(bestTimeText, bestTimeColor);
        }

        handSprite.enabled = false;
    }

    private void Update() {
        if (!lerping) elapsedTime = 0f;
        else {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            handObject.transform.position = Vector3.Lerp(handObject.transform.position, lerpTargetPosition, speedCurve.Evaluate(percentageComplete));

            if (handObject.transform.position == lerpTargetPosition) {
                lerping = false;
                handSprite.enabled = false;
                handTrail.enabled = false;
            }
        }

        UpdateTimerText();
    }

    private void UpdateTimerText() {
        if (levelData.hasTimer) {
            tempText = hasFormat ? currentTime.value.ToString(timeFormats[format]) : currentTime.value.ToString();
            currentTimeText.text = tempText;
            currentTime.SetValueInInspector();
        }
    }

    private void SetFailTimerColor() {
        SetTimerColor(currentTimeText, failTimerColor);
    }

    private void SetTimerColor(TextMeshProUGUI _text, Color _color) {
        _text.color = _color;
    }

    private void UpdateYourTimeText() {
        if (levelData.hasTimer) {
            string pb = hasFormat ? currentTime.value.ToString(timeFormats[format]) : currentTime.value.ToString();
            yourTimeText.text = pb;
        }
    }

    private void UpdateBestTimeText() {
        if (levelData.hasTimer && levelData.hasBestTimer) {
            string pb = hasFormat ? bestTime.value.ToString(timeFormats[format]) : bestTime.value.ToString();
            bestTimeText.text = pb;
            bestTime.SetValueInInspector();
        }
    }

    private void SetHandGrab(Vector3 _position) {
        if (lerping) return;
        handSprite.enabled = true;
        handTrail.enabled = false;
        handAnimator.SetTrigger("Grab");
        handObject.transform.position = _position;
    }

    private void MoveHandSprite(Vector3 _position) {
        if (lerping) return;
        handSprite.enabled = true;
        handTrail.enabled = true;
        handAnimator.SetTrigger("Swipe");
        lerping = true;
        lerpTargetPosition = _position;
    }

    private void HandTapAnimation(Vector3 _position) {
        if (lerping) return;
        handSprite.enabled = true;
        handTrail.enabled = false;
        handAnimator.SetTrigger("Tap");
        handObject.transform.position = _position;
    }

    private void SetLevelData(LevelData _currentLevel) {
        levelData = _currentLevel;
    }

    private void UpdateLevelText(int level) {
        level++;
        levelText.text = level.ToString("000");
    }
}

public enum TimerFormats {
    Whole,
    TenthDecimal,
    HundrethDecimal
}