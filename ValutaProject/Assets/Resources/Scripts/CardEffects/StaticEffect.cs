using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//常駐効果Class
public class StaticEffect : CardEffect {
    public override EffectPlayType GetEffectPlayType()
    {
        return EffectPlayType.staticEffect;
    }
}
