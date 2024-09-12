using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnFirstSwipe : MonoBehaviour {
    private void OnEnable() {
        SlotsManager.OnFirstSwipe += DestroyThis;
    }

    private void OnDisable() {
        SlotsManager.OnFirstSwipe -= DestroyThis;
    }

    private void DestroyThis() {
        GameObject.Destroy(this.gameObject);
    }
}
