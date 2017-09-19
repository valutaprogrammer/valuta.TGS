using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageBlock : EventBlock {
    public TakeDamageBlock(Player caster, int value) : base(caster) { }

    public override List<TriggerBlock> GetTrigger()
    {
        List<TriggerBlock> triggers = new List<TriggerBlock>();
        triggers.Add(new TriggerBlock(TriggerType.TakeDamage, caster));
        return triggers;
    }
}
