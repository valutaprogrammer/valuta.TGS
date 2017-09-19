using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWindowSize : MonoBehaviour {

    [RuntimeInitializeOnLoadMethod]
    static void GameInit() {
        //Screen.SetResolution(640,960,false);
        Screen.SetResolution(768, 1024, false);
    }

}
