using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//罠カードClass
public class Trap : Card {
    //public TriggerType Trigger;//State.triggerが発動条件です。
    public TriggerType GetTriggerType() {
        return State.trigger;
    }

    //セットは使用コスト0
    public override int Play(GameObject target = null){
        return Set();
    }

    //セットは使用コスト0、場が空いているかのみ確認
    public override bool isPlay(int haveCost,CardAligment aligment = CardAligment.All){
        return Constants.BATTLE.FindCardPlayZone(this, Constants.BATTLE.FindCardHolder(this)).isAddCard();
    }

    public int Set() {
        if (!Constants.BATTLE.CardSet(this)) {
            Debug.Log("処理が失敗しました");
        } 
        return 0;
    }

    //使用コストを返す
    public int Open(GameObject target = null) {
        Constants.BATTLE.CardOpen(this,target);
        return State.Cost;
    }

    public bool isOpen(int haveCost) {
        return (haveCost >= State.Cost);
    }
}
