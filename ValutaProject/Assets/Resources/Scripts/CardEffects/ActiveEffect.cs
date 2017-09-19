using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//起動効果Class
public class ActiveEffect : CardEffect {
    //消費コスト
    public int cost;

    public ActiveEffect(int cost) { this.cost = cost; }

    public bool isPlay(Player caster,int? haveCost = null) {
        if (!haveCost.HasValue) haveCost = caster.cost;
        return (haveCost >= cost && effectState[0].Target.GetTargets(caster).Count > 0);
    }

    public override EffectPlayType GetEffectPlayType()
    {
        return EffectPlayType.activeEffect;
    }

    public override IEnumerator EffectCall(Player p, Card card, GameObject target = null)
    {
        card.RunTimeState.isActiveEffectPlayed = true;
        p.cost -= cost;
        yield return base.EffectCall(p, card, target);
    }
}
