using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    public BallType_SO ballType;

    public float desiredDuration = 1f;
    public float elapsedTime;
    private bool rotating = false;
    private Vector3 targetPosition;

    [SerializeField] private AnimationCurve speedCurve;

    [SerializeField] private GameObject starsParticle;

    private void OnEnable() {
        SlotsManager.OnBallsSorted += PlayWinParticles;
    }

    private void OnDisable() {
        SlotsManager.OnBallsSorted -= PlayWinParticles;
    }

    private void Update() {
        if (!rotating) elapsedTime = 0f;
        else {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            transform.position = Vector3.Lerp(transform.position, targetPosition, speedCurve.Evaluate(percentageComplete));

            if (transform.position == targetPosition) {
                rotating = false;
            }
        }
    }

    public void SetRotationState(bool _rotating, Vector3 _targetPosition) {
        rotating = _rotating;
        targetPosition = _targetPosition;
    }

    public void PlayWinParticles() {
        GameObject particles = Instantiate(starsParticle, transform.position, starsParticle.transform.rotation);
        particles.transform.localScale = transform.localScale;
        particles.GetComponent<ParticleSystem>().Play();
    }
}
