using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInput : MonoBehaviour {
    private Image image;
    private Card card;

    //[SerializeField]
    //List<Vector3> DraftPos;
    //Vector3 setpos;
    //private int draftMoveCount = 0;
    //GameObject MoveCard;
    void Start(){
        card = GetComponent<Card>();
        //ここから
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = gameObject.AddComponent<EventTrigger>();// GetComponent<EventTrigger>();

        // GetMouseButtonDownのイベントを設定
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(ClickST);
        trigger.triggers.Add(entry);

        // GetMouseButtonUpのイベントを設定
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerUp;
        entry1.callback.AddListener(ClickED);
        trigger.triggers.Add(entry1);
        image = GetComponent<Image>();

        //PointerEnterのイベント設定
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(MouseOn);
        trigger.triggers.Add(entry);

        //PointerExitのイベント設定
        entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerExit;
        entry1.callback.AddListener(MouseOff);
        trigger.triggers.Add(entry1);
    }

    //マウスクリックのイベント対象操作
    private void TargetActive(bool isTarget){
        image.raycastTarget = isTarget;

        List<GameObject> childs = new List<GameObject>();
        GetAllChilds(image.gameObject, childs);
        foreach (GameObject obj in childs) {
            Image cImage = obj.GetComponent<Image>();
            if (cImage) { cImage.raycastTarget = isTarget; }
            Text cText = obj.GetComponent<Text>();
            if (cText) { cText.raycastTarget = isTarget; }
        }
    }

    private void GetAllChilds(GameObject obj,List<GameObject> returnObjs) {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject cObj = obj.transform.GetChild(i).gameObject;
            returnObjs.Add(cObj);
            GetAllChilds(cObj, returnObjs);
        }
    }

    //マウスが押された時のイベント
    void ClickST(BaseEventData eventData)
    {
        //InputManaegr.IM.SetCardClick(card);
        InputManaegr.IM.SetInput(InputType.onClick, InputTargetType.card, card.gameObject);
        TargetActive(false);
    }

    //マウスが放された時のイベント
    void ClickED(BaseEventData eventData)
    {
        //InputManaegr.IM.RemoveCardClick(card);
        InputManaegr.IM.SetInput(InputType.removeClick, InputTargetType.card, card.gameObject);
        TargetActive(true);
    }

    //マウスが上に乗った時のイベント
    void MouseOn(BaseEventData eventData) {
        //InputManaegr.IM.SetPointerCard(card);
        InputManaegr.IM.SetInput(InputType.onPointer, InputTargetType.card, card.gameObject);
    }

    //マウスが上から離れた時のイベント
    void MouseOff(BaseEventData eventData) {
        //InputManaegr.IM.RemovePointerCard(card);
        InputManaegr.IM.SetInput(InputType.removePointer, InputTargetType.card, card.gameObject);
    }

    //カード（自身）の移動
    private bool MoveFlag = false;
    IEnumerator Move(GameObject target, Vector3 goal, float t)
    {
        if (MoveFlag) yield break;
        MoveFlag = true;
        float nowTime = 0;
        Vector3 start = target.transform.position;
        Vector3 speed = (goal - start) / t;
        while ((nowTime += Time.deltaTime) <= t)
        {
            target.transform.position = start + (speed * nowTime);

            yield return null;
        }
        target.transform.position = goal;
        MoveFlag = false;
    }
}
