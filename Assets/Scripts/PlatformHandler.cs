using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlatformHandler : MonoBehaviour {
    public static event Action<bool> OnAnchorPointChanged;
    public static event Action OnAnchored;
    public static event Action OnUnanchored;
    public static event Action<Vector3> OnTap;

    private Camera mainCamera;
    [SerializeField] private Transform platformsTransform;

    [SerializeField] private Transform firstAnchor, secondAnchor;
    [SerializeField] private Transform currentAnchor, targetAnchor;

    public bool isAnchored = true;
    public float desiredDuration;
    private float elapsedTime;

    private bool _isFirstSided = true;
    [SerializeField] private bool _canTap = true;
    [SerializeField] private bool _lockTap = false;

    [SerializeField] private AnimationCurve speedCurve;

    private void Awake() {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Start() {
        if (platformsTransform == null) platformsTransform = GameObject.FindGameObjectWithTag("Platform").transform;
        if (firstAnchor == null) firstAnchor = GameObject.FindGameObjectWithTag("FirstAnchor").transform;
        if (secondAnchor == null) secondAnchor = GameObject.FindGameObjectWithTag("SecondAnchor").transform;
        
        RestartPlatformPosition();
        EnableTapping();
    }

    private void OnEnable() {
        RotationHandler.OnRotationEnd += EnableTapping;
        RotationHandler.OnRotationStart += DisableTapping;

        SlotsManager.OnBallsSorted += LockTapping;
        GameManager.OnTimerEnd += LockTapping;

        OnUnanchored += DisableTapping;
        OnAnchored += EnableTapping;
    }

    private void OnDisable() {
        RotationHandler.OnRotationEnd -= EnableTapping;
        RotationHandler.OnRotationStart -= DisableTapping;

        SlotsManager.OnBallsSorted -= LockTapping;
        GameManager.OnTimerEnd -= LockTapping;

        OnUnanchored -= DisableTapping;
        OnAnchored -= EnableTapping;
    }
    
    private void Update() {
        if (_lockTap) return;
        else {
            if (isAnchored) return;
            else {
                elapsedTime += Time.deltaTime;
                float percentageComplete = elapsedTime / desiredDuration;
                platformsTransform.position = Vector3.Lerp(platformsTransform.position, currentAnchor.position, speedCurve.Evaluate(percentageComplete));
                if (platformsTransform.position == currentAnchor.position) {
                    isAnchored = true;
                    OnAnchored?.Invoke();
                }
            }
        }
    }
    
    private void OnMouseDown() {
        TapDetection();
    }

    private void TapDetection() {
        if (_lockTap) return;
        else if (_canTap) {
            elapsedTime = 0;
    
            Vector3 tapPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    
            OnTap?.Invoke(Utils.ScreenToWorld(mainCamera, tapPosition));
    
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
        }
    }

    private void RestartPlatformPosition() {
        currentAnchor = firstAnchor;
        targetAnchor = secondAnchor;
        platformsTransform.position = firstAnchor.position;
    }

    private void EnableTapping() {
        _canTap = true;
    }

    private void DisableTapping() {
        _canTap = false;
    }

    private void LockTapping() {
        _lockTap = true;
    }
}
