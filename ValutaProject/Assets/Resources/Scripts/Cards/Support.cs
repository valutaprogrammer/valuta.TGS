using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//支援カードClass
public class Support : Card {
    //破壊条件
    public bool DestTriggerCheck(TriggerType type) {
        return (State.trigger == type);
    }

    public TriggerType GetDestTrigger() {
        return State.trigger;
    }
}
