using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カード破壊のEventBlock
public class CardDestBlock : CardBlock {
    public CardDestBlock(Card card,Player caster) : base(card,caster) { }
    public CardDestBlock(List<Card> cards, Player caster) : base(cards,caster) { }

    public override IEnumerator ResolveBlock() {
        foreach (Card card in cards) {
            foreach (CardEffect e in card.State.Effect) {
                if (e.GetEffectPlayType() == EffectPlayType.triggerEffect && (e as TriggerEffect).TriggerType == TriggerType.OnDest) {
                    yield return caster.StartCoroutine(e.EffectCall(caster, card));
                }
            }
            //Constants.BATTLE.CardDest(card);
            Constants.BATTLE.Field.DestAddCard(card);
            yield return new WaitForSeconds(1.0f);
        }
        yield return Constants.BATTLE.StartCoroutine(base.ResolveBlock());
        //Debug.Log("CardDest");
    }

    public override List<TriggerBlock> GetTrigger() {
        List<TriggerBlock> trigges = base.GetTrigger();
        bool isUnit = false;
        foreach (Card c in cards) {
            if (c.State.cardType == ObjectType.Unit)
                isUnit = true;
        }
        if (isUnit) trigges.Add(new TriggerBlock(TriggerType.UnitDest, caster));

        //Debug.Log("CardDest");
        return trigges;
    }
}
