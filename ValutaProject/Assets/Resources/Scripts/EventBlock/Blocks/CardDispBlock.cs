using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カード破棄のEventBlock
public class CardDispBlock : CardBlock{
    public CardDispBlock(Card card,Player caster) : base(card,caster) { }
    public CardDispBlock(List<Card> cards, Player caster) : base(cards, caster) { }

    public override void AddBlock() {
        foreach (Card card in cards)
            Constants.BATTLE.Field.DestAddCard(card);
        base.AddBlock();
    }

    public override IEnumerator ResolveBlock() {
        foreach (Card card in cards) {
            //Constants.BATTLE.Field.DestAddCard(card);//カードを捨ててから「捨てた」処理を行うためスタック追加時に即座に破棄
            //yield return new WaitForSeconds(1.0f);
        }
        yield return Constants.BATTLE.StartCoroutine(base.ResolveBlock());
        //Debug.Log("CardDest");
    }

    public override List<TriggerBlock> GetTrigger() {
        List<TriggerBlock> trigges = base.GetTrigger();

        trigges.Add(new TriggerBlock(TriggerType.TakeDispCard,Constants.BATTLE.GetOtherPlayer(caster)));

        //Debug.Log("CardDisp");
        return trigges;
    }
}
