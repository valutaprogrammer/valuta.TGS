using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostChangeBlock : EventBlock{
    public CostChangeBlock(int value,Player caster):base(caster) { this.value = value;this.caster = caster; }

    protected int value = 0;

    public override List<TriggerBlock> GetTrigger()
    {
        List<TriggerBlock> triggers = new List<TriggerBlock>();
        if (value >= 16) triggers.Add(new TriggerBlock(TriggerType.Cost_16OrMore, caster));
        return triggers;
    }
}
