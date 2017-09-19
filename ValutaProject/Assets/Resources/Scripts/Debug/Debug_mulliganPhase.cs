using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_mulliganPhase : ValutaDebug {

    private void Update()
    {
        base.PlayCheck();
    }

    public override void OnPlay()
    {
        Constants.BATTLE.MulliganStart();
        base.OnPlay();
    }
}
