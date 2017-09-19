using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventBlock{
    protected Player caster = null;//このイベントを発生させたplayer

    //初期化
    public EventBlock(Player caster) { this.caster = caster; }

    /// <summary>
    /// 処理の解決
    /// </summary>
    public virtual IEnumerator ResolveBlock() {
        List<TriggerBlock> triggers = GetTrigger();
        if (triggers.Count > 0) {
            //Debug.Log(triggers[0].ToString() + "の効果を解決します");
        }
        yield return null;
        if (Constants.BATTLE.RefreshStack()) {
            //Debug.Log(GetType());
        } 
    }

    public virtual void DeleteBlock() {
        Constants.BATTLE.DeleteStack();
    }

    /// <summary>
    /// 処理のスタック
    /// </summary>
    public virtual void AddBlock() {
        Constants.BATTLE.AddStack(this);
    }

    public virtual List<TriggerBlock> GetTrigger() {
        return new List<TriggerBlock>();
    }

    public Player GetCaster() {
        return caster;
    }

    //Triggerをどちらが確かめるか
    public virtual Player GetTriggerCaster() {
        return GetCaster();
    }
}

//カードに関するEventBlock基底クラス
public class CardBlock : EventBlock {
    protected List<Card> cards = new List<Card>();//複数のカードが入っても大丈夫なように変更
    public List<Card> GetCards() { return cards; }

    public CardBlock(Card card,Player caster):base(caster) { cards.Add(card); }
    public CardBlock(List<Card> cards, Player caster) : base(caster) { this.cards = cards; }
}