using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffectScript : MonoBehaviour {
    float t = 0;
    const float max = 0.5f;
    void Update() {
        t += Time.deltaTime;
        if (t > max) {
            Debug.Log("testEffect");
            Destroy(gameObject);
        } 
    }
}
