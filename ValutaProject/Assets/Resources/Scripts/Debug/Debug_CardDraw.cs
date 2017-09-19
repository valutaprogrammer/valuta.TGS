using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_CardDraw : ValutaDebug
{
    
	void Update () {
        base.PlayCheck();
    }

    public override void OnPlay()
    {
        base.OnPlay();
        if (Constants.BATTLE.GetActivePlayer() != null)
            Constants.BATTLE.GetActivePlayer().CardDraw();
    }
}
