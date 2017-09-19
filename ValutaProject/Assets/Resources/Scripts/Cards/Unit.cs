using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ユニットカードClass
public class Unit : Card,IAttackTarget {
    //public int atk { get; set; }
    //public int Dffence { get; set; }

    //ダメージを受ける
    public IEnumerator Damage(int damage)
    {
        DamageEffect.DamageInit(damage, gameObject);
        if (State.defence <= damage) StartCoroutine(DilayDest()); //Constants.BATTLE.CardDest(this);
        //return damage;
        yield return null;
    }

    void Start() {
        foreach (CardEffect ce in State.Effect) {
            foreach (EffectState es in ce.effectState) {
                if (es.effectType == EffectType.Guardian)
                    targetPriority = Constants.TARGET_GUADIAN;
            }
        }
    }

    public int targetPriority = Constants.TARGET_NOMAL;
    public int GetTargetPriority() {
        return targetPriority;
    }

    private IEnumerator DilayDest(float dilay = 0.5f) {
        yield return new WaitForSeconds(dilay);
        Constants.BATTLE.CardDest(this);
    }
}
