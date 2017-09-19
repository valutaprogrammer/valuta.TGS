using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_GetDraft : ValutaDebug {
    public Card card;
	
	void Update () {
        base.PlayCheck();
	}

    public override void OnPlay()
    {
        base.OnPlay();
        Constants.BATTLE.DraftDraw(card);
        card = null;
    }
}
