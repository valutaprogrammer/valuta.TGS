using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_GameInit : ValutaDebug
{

	void Update () {
        base.PlayCheck();
	}

    public override void OnPlay()
    {
        base.OnPlay();
        Constants.BATTLE.GameInit();
    }
}
