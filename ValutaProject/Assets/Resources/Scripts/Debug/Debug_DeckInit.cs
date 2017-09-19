using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_DeckInit : ValutaDebug
{
    public int DeckCount = 60;

    void Start() {
        isOnPlay = true;
        base.PlayCheck();
    }

	void Update () {
        base.PlayCheck();
	}

    //デッキの生成
    public override void OnPlay()
    {
        base.OnPlay();
        for (int i = 0; i < DeckCount; i++) {

        }
    }
}
