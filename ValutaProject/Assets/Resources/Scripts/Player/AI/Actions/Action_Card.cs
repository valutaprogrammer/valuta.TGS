using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Card : Action {
    public Action_Card(Player player, Card card) : base(player) {
        this.card = card;
        cost = card.State.Cost;
    }

    public Card card;

    public override string GetActionName()
    {
        return base.GetActionName() + "：カード「" + card.State.cardName + "」";
    }
}
