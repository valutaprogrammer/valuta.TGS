using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValutaDebug : MonoBehaviour {
    [SerializeField]
    public bool isOnPlay = false;

    public virtual void PlayCheck(){
        if (isOnPlay) OnPlay();
    }

    public virtual void OnPlay() {
        isOnPlay = false;
    }
}
