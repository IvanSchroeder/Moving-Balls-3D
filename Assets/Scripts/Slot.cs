using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Slot : MonoBehaviour
{
    [SerializeField] private string fontName;
    [SerializeField] private GameObject slotGOReference;
    [SerializeField] private Vector3 slotPosition;
    [SerializeField] private Ball storedBall;
    [SerializeField] private bool canRotate;
    [SerializeField] private Slot previousSlot;
    [SerializeField] private Slot nextSlot;

    public string _fontName {
        get { return fontName; }
        set { fontName = value; }
    }

    public GameObject _slotGOReference {
        get { return slotGOReference; }
        set { slotGOReference = value; }
    }

    public Vector3 _slotPosition {
        get { return slotPosition; }
        set { slotPosition = value; }
    }

    public Ball _storedBall {
        get { return storedBall; }
        set { storedBall = value; }
    }

    public bool _canRotate {
        get { return canRotate; }
        set { canRotate = value;}
    }

    public Slot _previousSlot {
        get { return previousSlot; }
        set { previousSlot = value; }
    }

    public Slot _nextSlot {
        get { return nextSlot; }
        set { nextSlot = value; }
    }

    public Slot(string _fontName, GameObject _slotGOReference, Vector3 _slotPosition, bool _canRotate) {
        fontName = _fontName;
        slotGOReference = _slotGOReference;
        slotPosition = _slotPosition;
        canRotate = _canRotate;
    }

    public Slot InitializeSlot(Slot slot, string _fontName, GameObject _slotReference, Vector3 _slotPosition, bool _canRotate) {
        fontName = _fontName;
        slotGOReference = _slotReference;
        slotPosition = _slotPosition;
        canRotate = _canRotate;
        return slot;
    }
}
