using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position) {
        if (camera.orthographic) position.z = camera.nearClipPlane;
        position.z = 3.3f;
        return camera.ScreenToWorldPoint(position);
    }

    public static void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static int GetFirstIndex<T>(List<T> list) {
        int firstIndex = 0;
        return firstIndex;
    }

    public static T GetFirstElement<T>(List<T> list) {
        var firstElement = list[GetFirstIndex(list)];
        return firstElement;
    }

    public static int GetRandomIndex<T>(List<T> list) {
        int randomIndex = Random.Range(0, list.Count - 1);
        return randomIndex;
    }

    public static T GetRandomElement<T>(List<T> list) {
        var randomElement = list[GetRandomIndex(list)];
        return randomElement;
    }

    public static int GetLastIndex<T>(List<T> list) {
        int lastIndex = list.Count - 1;
        return lastIndex;
    }

    public static T GetLastElement<T>(List<T> list) {
        var lastElement = list[GetLastIndex(list)];
        return lastElement;
    }

    public static Camera GetMainCamera() {
        Camera camera = Camera.main;
        return camera;
    }
}
