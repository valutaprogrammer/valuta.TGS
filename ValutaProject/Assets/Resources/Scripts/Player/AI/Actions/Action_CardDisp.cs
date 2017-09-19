using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CardDisp : Action_Card
{
    public Action_CardDisp(Player player, Card card,GameObject target = null) : base(player, card)
    {

    }

    public override string GetActionName()
    {
        return base.GetActionName() + "破棄";
    }
}
