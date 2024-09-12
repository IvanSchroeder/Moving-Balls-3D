using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidesHandler : MonoBehaviour {
    [SerializeField] private MeshRenderer FirstPiece;
    [SerializeField] private MeshRenderer SecondPiece;
    [SerializeField] private MeshRenderer FirstArt;
    [SerializeField] private MeshRenderer SecondArt;
    [SerializeField] private MeshRenderer FirstPlatform;
    [SerializeField] private MeshRenderer SecondPlatform;

    private void OnEnable() {
        //SlotsManager.OnColorsSelected += SetSidesColors;
        SlotsManager.OnBallsSorted += HideMesh;
    }

    private void OnDisable() {
        //SlotsManager.OnColorsSelected -= SetSidesColors;
        SlotsManager.OnBallsSorted -= HideMesh;
    }

    private void SetSidesColors(BallType_SO _primaryType, BallType_SO _secondaryType) {
        FirstPiece.material.color = _primaryType.color;
        SecondPiece.material.color = _secondaryType.color;

        FirstArt.material.color = _primaryType.color;
        SecondArt.material.color = _secondaryType.color;
    }

    private void HideMesh() {
        FirstPiece.enabled = false;
        SecondPiece.enabled = false;

        FirstPlatform.enabled = false;
        SecondPlatform.enabled = false;
    }
}
