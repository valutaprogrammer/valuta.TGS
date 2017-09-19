using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カード効果のEventBlock
public class CardEffectBlock : CardBlock{
    protected EffectState state;

    public CardEffectBlock(EffectState state,Card card, Player caster) : base(card,caster) {
        this.state = state;
    }

    public override IEnumerator ResolveBlock()
    {
        if (caster == Constants.BATTLE.Players[0])
            caster.StartCoroutine(Constants.TextBox_Down.SetEffectText(cards[0].State.cardName, cards[0].State.text));
        else
            caster.StartCoroutine(Constants.TextBox_Up.SetEffectText(cards[0].State.cardName, cards[0].State.text));
        yield return caster.StartCoroutine(state.Play(caster, cards[0], state));
        yield return caster.StartCoroutine(base.ResolveBlock());
        //Debug.Log("効果処理 終了");
    }
}
