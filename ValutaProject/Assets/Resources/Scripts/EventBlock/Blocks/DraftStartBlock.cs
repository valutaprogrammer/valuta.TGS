using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftStartBlock : EventBlock {
    public DraftStartBlock(Player caster) : base(caster) { }

    public override List<TriggerBlock> GetTrigger()
    {
        List<TriggerBlock> triggers = new List<TriggerBlock>();
        triggers.Add(new TriggerBlock(TriggerType.DraftStart, Constants.BATTLE.GetOtherPlayer(caster)));
        return triggers;
    }

    public override IEnumerator ResolveBlock()
    {
        caster.StartCoroutine(Constants.BATTLE.DraftStart());
        yield return caster.StartCoroutine(base.ResolveBlock());
    }
}
