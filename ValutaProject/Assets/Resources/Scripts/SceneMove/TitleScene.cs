using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : SceneMoveScript {

    [SerializeField]
    private GameObject obj;
    /*
	void Update () {
        if (!isMove && Input.GetMouseButtonDown(0)) {
            StartCoroutine(MainSceneMove());
        }
	}

    private bool isMove;
    IEnumerator MainSceneMove() {
        isMove = true;
        float t = 0;
        for (int i = 0; i < 10;i++) {
            while (t < 0.15f) {
                t += Time.deltaTime;
                yield return null;
            }
            obj.SetActive(!obj.activeSelf);
            t = 0.0f;
        }
        isMove = false;
        SceneMove();
    }*/
}
