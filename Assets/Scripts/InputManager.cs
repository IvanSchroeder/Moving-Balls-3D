using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {
    private Camera mainCamera;

    public static event Action OnRotationStart;
    public static event Action OnRotationEnd;
    public static event Action OnLeftSwipe;
    public static event Action OnRightSwipe;
    public static event Action<Vector3> OnSwipeStart;
    public static event Action<Vector3> OnSwipeEnd;

    public static event Action<bool> OnAnchorPointChanged;
    public static event Action OnAnchored;
    public static event Action OnUnanchored;
    public static event Action<Vector3> OnTap;

    [Header("Tap Settings")]
    [SerializeField] private Transform platformsTransform;
    [SerializeField] private Transform firstAnchor, secondAnchor;
    [SerializeField] private Transform currentAnchor, targetAnchor;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private bool _canTap = true;
    public bool enableTapping;
    public float desiredDuration;
    private bool _isFirstSided = true;
    public bool isAnchored = true;
    
    [Header("Swipe Settings")]
    [SerializeField] private bool _canSwipe = true;
    public bool enableSwipping;
    public float targetTime;
    [SerializeField] private Vector3 firstPressPos;
    [SerializeField] private Vector3 secondPressPos;
    [SerializeField] private Vector3 currentSwipe;

    [Header("Input Settings")]
    [SerializeField] private bool _canInput = true;
    [SerializeField] private bool _lockInput = false;
    public bool enableInput = true;
    [SerializeField] private float swipeDistanceThreshold;
    [SerializeField] private float tapDistanceThreshold;
    [SerializeField] private float elapsedInputTime;

    private void OnEnable() {
        OnAnchored += EnableInput;
        OnUnanchored += DisableInput;

        OnRotationEnd += EnableInput;
        OnRotationStart += DisableInput;

        SlotsManager.OnBallsSorted += LockInput;
        GameManager.OnTimerEnd += LockInput;
    }

    private void OnDisable() {
        OnAnchored -= EnableInput;
        OnUnanchored -= DisableInput;

        OnRotationEnd -= EnableInput;
        OnRotationStart -= DisableInput;

        SlotsManager.OnBallsSorted -= LockInput;
        GameManager.OnTimerEnd -= LockInput;
    }

    private void Awake() {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Start() {
        if (platformsTransform == null) platformsTransform = GameObject.FindGameObjectWithTag("Platform").transform;
        if (firstAnchor == null) firstAnchor = GameObject.FindGameObjectWithTag("FirstAnchor").transform;
        if (secondAnchor == null) secondAnchor = GameObject.FindGameObjectWithTag("SecondAnchor").transform;
        
        RestartPlatformPosition();
        
        EnableInput();
    }

    private void Update() {
        if (_lockInput) return;
        if (enableTapping) Tapping();
        else if (enableSwipping) Swipping();
    }

    private void Tapping() {
        if (isAnchored) return;
        else {
            elapsedInputTime += Time.deltaTime;
            float percentageComplete = elapsedInputTime / desiredDuration;
            platformsTransform.position = Vector3.Lerp(platformsTransform.position, currentAnchor.position, speedCurve.Evaluate(percentageComplete));

            if (platformsTransform.position == currentAnchor.position) {
                isAnchored = true;
                OnAnchored?.Invoke();
                enableTapping = false;
                EnableTapping();
            }
        }
    }

    private void Swipping() {
        elapsedInputTime += Time.deltaTime;

        if (elapsedInputTime >= targetTime) {
            OnRotationEnd?.Invoke();
            enableSwipping = false;
            EnableSwipping();
        }
    }

    private void OnMouseDown() {
        InputDetectionStart();
    }

    private void OnMouseUp() {
        InputDetectionEnd();
    }

    private void InputDetectionStart() {
        if (_lockInput /*|| EventSystem.current.IsPointerOverGameObject()*/) return;
        else {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    private void InputDetectionEnd() {
        if (_lockInput /*|| EventSystem.current.IsPointerOverGameObject()*/) return;
        else if (_canInput) {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            float distance = Vector2.Distance(secondPressPos, firstPressPos);

            if (((-swipeDistanceThreshold) < currentSwipe.x && currentSwipe.x < swipeDistanceThreshold) && ((-tapDistanceThreshold) < currentSwipe.x && currentSwipe.x < tapDistanceThreshold)) {
                TapDetection();
            }
            else if (currentSwipe.x < (-swipeDistanceThreshold) || currentSwipe.x > swipeDistanceThreshold) {
                SwipeDetection();
                OnSwipeStart?.Invoke(Utils.ScreenToWorld(mainCamera, firstPressPos));
            }
        }
    }

    private void TapDetection() {
        if (_canTap) {
            OnTap?.Invoke(Utils.ScreenToWorld(mainCamera, firstPressPos));
        
            OnUnanchored?.Invoke();
            isAnchored = false;
        
            Transform tempAnchor = currentAnchor;
            currentAnchor = targetAnchor;
            targetAnchor = tempAnchor;
        
            if (currentAnchor == firstAnchor) {
                _isFirstSided = true;
            }
            else {
                _isFirstSided = false;
            }

            OnAnchorPointChanged?.Invoke(_isFirstSided);

            enableTapping = true;
            enableSwipping = false;
            elapsedInputTime = 0;

            DisableTapping();
        }
    }

    private void SwipeDetection() {
        if (_canSwipe) {
            OnSwipeEnd?.Invoke(Utils.ScreenToWorld(mainCamera, secondPressPos));

            OnRotationStart?.Invoke();
            
            if (currentSwipe.x < (-swipeDistanceThreshold)) {
                OnLeftSwipe?.Invoke();
            }
            if (currentSwipe.x > swipeDistanceThreshold) {
                OnRightSwipe?.Invoke();
            }

            enableSwipping = true;
            enableTapping = false;
            elapsedInputTime = 0;

            DisableSwipping();
        }
    }

    private void RestartPlatformPosition() {
        currentAnchor = firstAnchor;
        targetAnchor = secondAnchor;
        platformsTransform.position = firstAnchor.position;
    }

    private void EnableInput() {
        _canInput = true;
    }

    private void DisableInput() {
        _canInput = false;
    }

    private void LockInput() {
        _lockInput = true;
    }

    private void EnableTapping() {
        _canTap = true;
    }

    private void DisableTapping() {
        _canTap = false;
    }

    private void EnableSwipping() {
        _canSwipe = true;
    }

    private void DisableSwipping() {
        _canSwipe = false;
    }
}
