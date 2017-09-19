using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClassTest : ValutaDebug {
    public Card card;

    void Start() {

    }

	void Update () {
        base.PlayCheck();
	}

    public override void OnPlay()
    {
        base.OnPlay();
        //type swicthはC#7から！
        if (card is Unit) unitTest((Unit)card);
        else if (card is Trap) test((Trap)card);
        else test(card);

        //card.CardCopy(card);
    }

    void unitTest(Unit u) { test(u); }

    void traTest(Trap t) { test(t); }

    void test(Trap t) {
        Debug.Log(t.State.cardName + "is a trap");
    }

    void test(Unit u)
    {
        Debug.Log(u.State.cardName + "is a unit");
    }

    void test(Card c) {
        Debug.Log(c.State.cardName + "is a card");
    }

    
}
