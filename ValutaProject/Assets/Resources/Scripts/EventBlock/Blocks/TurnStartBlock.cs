using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStartBlock : EventBlock {
    public TurnStartBlock(Player caster) : base(caster) { }

    public override List<TriggerBlock> GetTrigger()
    {
        List<TriggerBlock> triggers = new List<TriggerBlock>();
        triggers.Add(new TriggerBlock(TriggerType.TurnStart, caster));
        return triggers;
    }

    public override IEnumerator ResolveBlock()
    {
        Constants.BATTLE.TurnChange();
        yield return caster.StartCoroutine(base.ResolveBlock());
    }
}
