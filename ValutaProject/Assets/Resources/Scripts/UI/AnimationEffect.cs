using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEffect : MonoBehaviour {

    void Awake() {
        Constants.ANIMATION_EFFECT = this;
    }

    //生成したエフェクトが消えるまで待機する
    public virtual IEnumerator EffectStart(GameObject effect,GameObject target = null) {
        GameObject obj;
        if (effect != null)
            obj = Instantiate(effect);
        else obj = Instantiate(Resources.Load<GameObject>("prefabs/Effects/Magic/Light.m/Light_M"));
        obj.transform.SetParent(Constants.BATTLE.EffectCanvas.transform);

        if (target != null) {
            Player p = target.GetComponent<Player>();
            if (p != null) target = p.playerImage.gameObject;
        }

        while (obj != null) {
            if (target != null) obj.transform.position = target.transform.position;
            yield return null;
        }
            
    }
}
