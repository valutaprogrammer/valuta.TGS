using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃のEventBlock
public class AttackBlock : EventBlock {
    private Damage attack;
    private GameObject target;

    public AttackBlock(GameObject target, Player caster) : base(caster){
        this.target = target;
        attack = new Damage();
    }

    public GameObject GetTarget() {
        return target;
    }

    public override IEnumerator ResolveBlock()
    {
        yield return caster.StartCoroutine(attack.Play(target, caster.AttackPower));
        yield return caster.StartCoroutine(base.ResolveBlock());
    }

    public override List<TriggerBlock> GetTrigger()
    {
        List<TriggerBlock> triggers = new List<TriggerBlock>() { new TriggerBlock(TriggerType.TakeAttack,GetTriggerCaster()) };
        if (target.GetComponent<Player>()) triggers.Add(new TriggerBlock(TriggerType.TakeDamage, GetTriggerCaster()));
        return triggers;
    }

    public override Player GetTriggerCaster()
    {
        return Constants.BATTLE.GetOtherPlayer(base.GetTriggerCaster());
    }
}
