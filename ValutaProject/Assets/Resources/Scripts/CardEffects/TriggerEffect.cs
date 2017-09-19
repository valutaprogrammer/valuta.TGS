using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//誘発条件　＊基本的に自身の陣営の行動のみに反応
public enum TriggerType
{
    None,//無し
    TurnStart,//ターン開始時
    TurnEnd,//ターン終了時
    DraftStart,//ドラフト開始時
    UnitDest,//ユニットが破壊された時
    PlaySkill,//技カードを使用した時
    PlayUnit,//ユニットカードを使用した時
    PlayCard_4CostOrMore,//既定コスト以上のカードが使用された時
    Cost_16OrMore,//規定値以上のコストが溜まった時
    TakeAttack,//攻撃を受ける時
    TakeSkill,//技カードが使用された時
    TakeDispCard,//相手のカードが破棄された時
    TakeDamage,//ダメージを受ける時
    OnDest,//自身が破壊された時
}

//誘発効果Class
public class TriggerEffect : CardEffect
{
    public TriggerType TriggerType;

    public EventBlock TriggerBlock;

    public override IEnumerator EffectCall(Player p, Card card, GameObject target = null)
    {
        //一番目の効果から処理
        foreach (EffectState e in effectState) {
            if (target) e.SetTarget(target);
            //e.Play(p,card, e);
            e.AddStack(p, card, e,TriggerBlock);
            yield return null;
        }
    }

    public override EffectPlayType GetEffectPlayType()
    {
        return EffectPlayType.triggerEffect;
    }
}