using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用時効果Class
public class PlayEffect : CardEffect {
    public override EffectPlayType GetEffectPlayType()
    {
        return EffectPlayType.playEffect;
    }
}
