using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class RotationHandler : MonoBehaviour {
    private Camera mainCamera;

    public static event Action OnRotationStart;
    public static event Action OnRotationEnd;
    public static event Action OnLeftSwipe;
    public static event Action OnRightSwipe;
    public static event Action<Vector3> OnSwipeStart;
    public static event Action<Vector3> OnSwipeEnd;

    private bool _rotate;
    private bool _rotating;
    [SerializeField] private bool _canSwipe = true;
    [SerializeField] private bool _lockSwipe = false;

    public float targetTime;
    public float elapsedTime;

    private Vector3 firstPressPos;
    private Vector3 secondPressPos;
    [SerializeField] private Vector3 currentSwipe;

    [SerializeField] private float swipeDistanceThreshold;

    private void OnEnable() {
        PlatformHandler.OnAnchored += EnableSwipping;
        PlatformHandler.OnUnanchored += DisableSwipping;

        SlotsManager.OnBallsSorted += LockSwipping;
        GameManager.OnTimerEnd += LockSwipping;

        OnRotationStart += DisableSwipping;
        OnRotationEnd += EnableSwipping;
    }

    private void OnDisable() {
        PlatformHandler.OnAnchored -= EnableSwipping;
        PlatformHandler.OnUnanchored -= DisableSwipping;

        SlotsManager.OnBallsSorted -= LockSwipping;
        GameManager.OnTimerEnd -= LockSwipping;

        OnRotationStart -= DisableSwipping;
        OnRotationEnd -= EnableSwipping;
    }

    private void Start() {
        mainCamera = Camera.main;
        EnableSwipping();
    }

    private void Update() {
        if (_lockSwipe) return;
        else {
            if (_rotate) {
                elapsedTime += Time.deltaTime;
                _rotating = true;

                if (elapsedTime >= targetTime) {
                    _rotate = false;
                    _rotating = false;
                    OnRotationEnd?.Invoke();
                }
            }
        }   
    }

    private void OnMouseDown() {
        SwipeDetectionStart();
    }

    private void OnMouseUp() {
        SwipeDetectionEnd();
    }

    private void SwipeDetectionStart() {
        if (_lockSwipe || _rotating || EventSystem.current.IsPointerOverGameObject()) return;
        else if (_canSwipe) {
            DisableSwipping();
            firstPressPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3.5f);
            OnSwipeStart?.Invoke(Utils.ScreenToWorld(mainCamera, firstPressPos));
        }
    }

    private void SwipeDetectionEnd() {
        if (_lockSwipe || _rotating) return;
        else {
            secondPressPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3.5f);

            OnSwipeEnd?.Invoke(Utils.ScreenToWorld(mainCamera, secondPressPos));

            currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y, 3.5f);

            OnRotationStart?.Invoke();
            
            if (currentSwipe.x < (-swipeDistanceThreshold)) {
                OnLeftSwipe?.Invoke();
            }
            if (currentSwipe.x > swipeDistanceThreshold) {
                OnRightSwipe?.Invoke();
            }

            _rotate = true;
            elapsedTime = 0;
        }
    }

    private void EnableSwipping() {
        _canSwipe = true;
    }

    private void DisableSwipping() {
        _canSwipe = false;
    }

    private void LockSwipping() {
        _lockSwipe = true;
    }
}
