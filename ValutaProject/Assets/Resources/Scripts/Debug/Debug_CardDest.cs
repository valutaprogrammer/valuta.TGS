using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_CardDest : ValutaDebug
{
    public Card Card;

    void Update () {
        PlayCheck();
    }

    public override void OnPlay()
    {
        base.OnPlay();
        Card.Dest();
        Card = null;
    }
}
