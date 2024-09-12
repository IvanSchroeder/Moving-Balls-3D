using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnFirstTap : MonoBehaviour {
    private void OnEnable() {
        SlotsManager.OnFirstTap += DestroyThis;
    }

    private void OnDisable() {
        SlotsManager.OnFirstTap -= DestroyThis;
    }

    private void DestroyThis() {
        GameObject.Destroy(this.gameObject);
    }
}
