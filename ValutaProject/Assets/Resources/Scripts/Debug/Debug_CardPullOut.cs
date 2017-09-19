using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_CardPullOut : ValutaDebug {
    [SerializeField]
    public Card card;
    
	void Update () {
        PlayCheck();
	}

    public override void OnPlay()
    {
        base.OnPlay();
        Constants.BATTLE.CardPullOut(card);
    }
}
