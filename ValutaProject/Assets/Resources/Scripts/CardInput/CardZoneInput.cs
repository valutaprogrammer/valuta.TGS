using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZoneInput : MonoBehaviour {
    
    //各種タッチイベントの設定
    void Start(){
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        //PointerEnterのイベント設定
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(MouseOn);
        trigger.triggers.Add(entry);

        //PointerExitのイベント設定
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerExit;
        entry1.callback.AddListener(MouseOff);
        trigger.triggers.Add(entry1);
    }

    //カードゾーンにマウスが乗った時
    void MouseOn(BaseEventData eventData){
        //InputManaegr.IM.SetPointerCardZone(GetComponent<CardZone>());
        InputManaegr.IM.SetInput(InputType.onPointer, InputTargetType.cardZone, gameObject);
    }

    //カードゾーンからマウスが出た時
    void MouseOff(BaseEventData eventData){
        //InputManaegr.IM.RemovePointerCardZone(GetComponent<CardZone>());
        InputManaegr.IM.SetInput(InputType.removePointer, InputTargetType.cardZone, gameObject);
    }
}
