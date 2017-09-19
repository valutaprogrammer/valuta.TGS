using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カード発動時のEventBlock
public class CardPlayBlock : CardBlock{
    private GameObject target;
    public CardPlayBlock(Card card, Player caster,GameObject target = null) : base (card,caster)
    {
        this.target = target;
    }

    public GameObject GetTarget() {
        return target;
    }

    public override IEnumerator ResolveBlock()
    {
        yield return caster.StartCoroutine(base.ResolveBlock());
        //一度処理の解決まで呼んでから効果の処理を追加する
        foreach (Card c in cards)
            yield return caster.StartCoroutine(Constants.BATTLE.CardPlayEffectPlay(c, caster, target));
        Debug.Log("カード使用処理　終了");
    }

    public override void DeleteBlock()
    {
        base.DeleteBlock();
        foreach (Card c in cards)
            c.Dest();
    }

    public override List<TriggerBlock> GetTrigger()
    {
        List<TriggerBlock> triggers = new List<TriggerBlock>();

        //どのカードが含まれるか確認し、トリガーを追加している
        bool isUnit = false;
        bool isSkill = false;
        bool is4CostOrMore = false;
        foreach (Card c in cards) {
            switch (c.State.cardType) {
                case ObjectType.Unit:
                    isUnit = true;
                    break;
                case ObjectType.Skill:
                    isSkill = true;
                    break;
            }
            if (c.State.Cost >= 4) is4CostOrMore = true;
        }
        if (isUnit) triggers.Add(new TriggerBlock(TriggerType.PlayUnit,caster) );
        if (isSkill) {
            triggers.Add(new TriggerBlock(TriggerType.PlaySkill, caster));
            triggers.Add(new TriggerBlock(TriggerType.TakeSkill, Constants.BATTLE.GetOtherPlayer(caster)));
        }
        if (is4CostOrMore) triggers.Add(new TriggerBlock(TriggerType.PlayCard_4CostOrMore, caster));
        //////コード汚い！！！！！！！！

        return triggers;
    }
}
