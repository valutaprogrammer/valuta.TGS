using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//効果スタックの確認と解決
public class Debug_EventStackChecker : ValutaDebug {
    public EventBlock[] blocks;

    private void Update() {
        blocks = Constants.BATTLE.EventStack.ToArray();

        PlayCheck();
    }

    public override void OnPlay()
    {
        base.OnPlay();
        Constants.BATTLE.ResolveStack();
    }
}
