using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuochSceneMove : SceneMoveScript {

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0)) {
            SceneMove();
        }
	}
}
