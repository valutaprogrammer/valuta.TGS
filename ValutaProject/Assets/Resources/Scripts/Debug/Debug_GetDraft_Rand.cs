using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_GetDraft_Rand : ValutaDebug {
    void Update()
    {
        base.PlayCheck();
    }

    public override void OnPlay()
    {
        base.OnPlay();
        Card card = Constants.BATTLE.Field.GetDraftZone().GetRandCard();
        if (card != null)
            Constants.BATTLE.DraftDraw(card);
    }
}
