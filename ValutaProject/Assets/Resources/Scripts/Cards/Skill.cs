using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//技カードClass
public class Skill : Card {

    /// <summary>
    /// カード使用可否確認　target指定版
    /// </summary>
    /// <param name="target"></param>
    /// <param name="caster"></param>
    /// <returns></returns>
    public bool isPlay(GameObject target = null,Player caster = null) {
        Player targetPlayer = Constants.BATTLE.FindObjectHolder(target);
        foreach (CardEffect e in State.Effect) {
            //対象が正しいか確認
            foreach (EffectState eState in e.effectState) {
                if (eState.Target.isAll) return true;
                if (eState.Target.isTarget(target,caster,targetPlayer)) return true;
            }
        }
        return false;
    }
}
