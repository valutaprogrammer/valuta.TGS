using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ActivEffect : Action_Card
{
    public Action_ActivEffect(Player player, Card card) : base(player, card){

    }

    public override string GetActionName()
    {
        return base.GetActionName() + "効果発動";
    }
}
