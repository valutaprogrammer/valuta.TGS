using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_TrapOpen : ValutaDebug {
    public Trap trap;
    
	void Update () {
        base.PlayCheck();
	}

    public override void OnPlay()
    {
        base.OnPlay();
        Player p = Constants.BATTLE.FindCardHolder(trap);
        p.TrapOpen(trap);
        trap = null;
    }
}
