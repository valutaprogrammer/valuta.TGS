using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Trigger（誘発イベント）とCaster（誘発対象）を持つクラス
public class TriggerBlock {
    public TriggerType trigger;
    public Player caster;

    public TriggerBlock(TriggerType trigger,Player caster) { this.trigger = trigger;this.caster = caster; }
}
