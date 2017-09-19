using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveScript : MonoBehaviour {
    [SerializeField]
    public int SceneNumber;

    public void SceneMove() {
        SceneManager.LoadScene(SceneNumber);
    }

}
