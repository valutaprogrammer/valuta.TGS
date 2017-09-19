using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEndBlock : EventBlock {
    public TurnEndBlock(Player caster) : base(caster) { }

    //他の処理が挟まった場合用に待機
    public override void AddBlock() {
        //base.AddBlock();
        caster.StartCoroutine(WaitForStack());
    }

    public IEnumerator WaitForStack() {
        while (Constants.BATTLE.EventStack.Count > 0) yield return null;
        base.AddBlock();
    }

    public override List<TriggerBlock> GetTrigger() {
        List<TriggerBlock> triggers = new List<TriggerBlock>();
        triggers.Add(new TriggerBlock(TriggerType.TurnEnd, caster));
        //Debug.Log("TurnEnd");
        return triggers;
    }

    public override IEnumerator ResolveBlock() {
        //Constants.BATTLE.TurnEnd();
        new TurnStartBlock(Constants.BATTLE.GetOtherPlayer(caster)).AddBlock();
        //Debug.Log("TurnEnd");
        yield return caster.StartCoroutine(base.ResolveBlock());
    }
}
