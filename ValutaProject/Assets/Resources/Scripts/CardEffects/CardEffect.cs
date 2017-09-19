using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カード効果の発動条件の種類
public enum EffectPlayType {
    none,//無し
    playEffect,//発動時効果
    staticEffect,//常駐効果
    triggerEffect,//誘発効果
    activeEffect//起動効果
}

//カード効果基底Class
[System.Serializable]
public class CardEffect
{
    [SerializeField]
    public List<EffectState> effectState = new List<EffectState>();

    //発動したPlayer
    //public Player caster;
    //親のカード
    public Card card;

    //効果処理呼び出し
    public virtual IEnumerator EffectCall(Player p,Card card,GameObject target = null) {
        //一番目の効果から処理
        foreach (EffectState e in effectState) {
            if (target) e.SetTarget(target);
            //e.Play(p,card, e);
            e.AddStack(p, card, e);
            yield return null;
        }
    }
    
    /// <summary>
    /// この効果の発動条件を取得
    /// </summary>
    /// <returns></returns>
    public virtual EffectPlayType GetEffectPlayType() {
        return EffectPlayType.none;
    }
}
