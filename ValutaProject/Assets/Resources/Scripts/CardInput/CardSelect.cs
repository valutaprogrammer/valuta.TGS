using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelect : MonoBehaviour {
    private EventTrigger.Entry entry;
    [SerializeField]
    private EventTrigger trigger;

    private GameObject Effect;

    // Use this for initialization
    void Start () {
        //SetSelectCardをタッチのコールバックに設定
        try {
            trigger = GetComponent<EventTrigger>();
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(SelectedCard);
            trigger.triggers.Add(entry);
        }
        catch {
            Debug.LogAssertion("カード選択用コールバックの指定に失敗しました");
            Destroy(this);
        }
        Debug.Log("カード選択用コールバックを指定しました");

        //選択可能エフェクトを生成
        Effect = Instantiate(Resources.Load(Constants.CARD_CAN_SELECT_EFFECT)) as GameObject;
        Effect.transform.SetParent(transform);
        Effect.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// このカードが選択された時に呼び出されるコールバック
    /// </summary>
    public void SelectedCard(BaseEventData eventData) {
        CardSelectManager.CS.SetSelectCard(GetComponent<Card>());
    }

    public void SelectEnd()
    {
        try {
            trigger.triggers.Remove(entry);
        }
        catch {
            //Debug.Log(trigger.triggers); //この時点でエラー triggerがnullでした
        }
        Destroy(Effect.gameObject);
        Debug.Log("OnDest");
        Destroy(this);
    }

    /*
    private void OnDisable() //変数が破棄された後に呼ばれるので相応しくなかった…
    {
        Debug.Log("OnDisable");
        SelectEnd();
    }
    */
}
