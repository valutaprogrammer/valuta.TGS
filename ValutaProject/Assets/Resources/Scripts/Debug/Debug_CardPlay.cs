using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_CardPlay : ValutaDebug
{
    public Card card;

    void Update() {
        base.PlayCheck();
    }

    public override void OnPlay()
    {
        base.OnPlay();

        Player p = Constants.BATTLE.FindCardHolder(card);
        p.CardPlay(card);
        card = null;
    }
}
