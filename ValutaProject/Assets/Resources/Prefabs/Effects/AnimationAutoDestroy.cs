using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAutoDestroy : MonoBehaviour {

    void Update(){
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) Destroy(gameObject);
    }
}
