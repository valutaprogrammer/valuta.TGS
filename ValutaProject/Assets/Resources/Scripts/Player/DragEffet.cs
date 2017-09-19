using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragEffet : MonoBehaviour {
    public Canvas canvas;
    [SerializeField]
    GameObject CursorObjectArrow;
    [SerializeField]
    GameObject CursorObjectStick;
    [SerializeField]
    float StickWidth;
    bool DragOn = false;

    [SerializeField]
    Player player;

    [SerializeField]
    private bool isActive = false;
    public void SetIsActive(bool value) {
        isActive = value;
        if (isActive) transform.SetSiblingIndex(1);
        else transform.SetSiblingIndex(0);
    }
    public bool GetIsActive() { return isActive; }

    GameObject CursorArrow;
    GameObject CursorStick;
    Vector3 Startpos;
    Vector3 canvasSize;
    Rect rect;
    RectTransform ArrowRect;
	RectTransform StickRect;
    // Use this for initialization
    void Start () {
        SetIsActive(false);
        rect = canvas.pixelRect;
        canvasSize = new Vector3(rect.width, rect.height, 0);

        gameObject.AddComponent<EventTrigger>();

        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        // GetMouseButtonDownのイベントを選択
        entry.eventID = EventTriggerType.InitializePotentialDrag;
        // コールバック登録
        entry.callback.AddListener(DragStart);
        // EventTriggerに追加
        trigger.triggers.Add(entry);

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        // GetMouseButtonDownのイベントを選択
        entry1.eventID = EventTriggerType.Drag;
        // コールバック登録
        entry1.callback.AddListener(Drag);
        // EventTriggerに追加
        trigger.triggers.Add(entry1);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        // GetMouseButtonDownのイベントを選択
        entry2.eventID = EventTriggerType.PointerUp;
        // コールバック登録
        entry2.callback.AddListener(DragEnd);
        // EventTriggerに追加
        trigger.triggers.Add(entry2);

        //マウスが乗った時
        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerEnter;
        entry3.callback.AddListener(OnMouse);
        trigger.triggers.Add(entry3);

        //マウスが離れたとき
        EventTrigger.Entry entry4 = new EventTrigger.Entry();
        entry4.eventID = EventTriggerType.PointerExit;
        entry4.callback.AddListener(RemoveMouse);
        trigger.triggers.Add(entry4);
    }
	

    public void DragStart(BaseEventData eventData)
    {
		if (!player.isAI && Input.GetMouseButton (0) && isActive) {
			if (!DragOn) {
                //InputManaegr.IM.PlayerClick(player);
                InputManaegr.IM.SetInput(InputType.onClick, InputTargetType.player, player.gameObject);
                ArrowInit();
			}
		}
    }

    void Drag(BaseEventData eventData)
    {
        if (!player.isAI)
            ArrowUpdate();
    }

    void DragEnd(BaseEventData eventData)
	{
		if (Input.GetMouseButtonUp (0)) {
            //InputManaegr.IM.RemovePlayerDrag(player);
            InputManaegr.IM.SetInput(InputType.removeClick, InputTargetType.player, player.gameObject);
        }
        ArrowDest();
    }

    //マウスが乗った時
    private void OnMouse(BaseEventData eventData) {
        //InputManaegr.IM.PointerPlayer(player);
        InputManaegr.IM.SetInput(InputType.onPointer, InputTargetType.player, player.gameObject);
    }

    private void RemoveMouse(BaseEventData eventData) {
        //InputManaegr.IM.RemovePointerPlayer(player);
        InputManaegr.IM.SetInput(InputType.removePointer, InputTargetType.player, player.gameObject);
    }

    /// <summary>
    /// プレイヤーからドラッグ先まで伸びる矢印を生成する
    /// </summary>
    public void ArrowInit() {
        Startpos = gameObject.transform.position;
        CursorArrow = Instantiate(CursorObjectArrow, Startpos, Quaternion.identity);
        CursorStick = Instantiate(CursorObjectStick, Startpos, Quaternion.identity);
        if (player.cardAligment == CardAligment.SowrdMan) {
            CursorArrow.GetComponent<Image>().color = Constants.S_COLOR;
            CursorStick.GetComponent<Image>().color = Constants.S_COLOR;
        }
        else if (player.cardAligment == CardAligment.Magician) {
            CursorArrow.GetComponent<Image>().color = Constants.M_COLOR;
            CursorStick.GetComponent<Image>().color = Constants.M_COLOR;
        }

        ArrowRect = CursorArrow.GetComponent<RectTransform>();
        StickRect = CursorStick.GetComponent<RectTransform>();
        StickWidth = StickRect.sizeDelta.x;
        CursorStick.transform.SetParent(transform);
        CursorArrow.transform.SetParent(transform);

        ArrowUpdate();
    }

    /// <summary>
    /// 生成した矢印の位置を更新する
    /// </summary>
    public void ArrowUpdate() {
        if (CursorArrow == null) return;

        ArrowRect.anchoredPosition = Input.mousePosition - (canvasSize / 2);
        Transform Parent = CursorArrow.transform.parent;
        if (!Parent.gameObject.GetComponent<Canvas>()) {
            ArrowRect.anchoredPosition -= new Vector2(Parent.localPosition.x, Parent.localPosition.y);
        }
        CursorStick.transform.localPosition = (CursorArrow.transform.position - Startpos) / 2;
        float a = Vector2.Distance(CursorArrow.transform.position, gameObject.transform.position);
        StickRect.sizeDelta = new Vector2(StickWidth, a);
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePos = ray.origin;
        Vector3 diff = mousePos - CursorArrow.transform.localPosition;
        Vector3 norm = diff.normalized;
        CursorArrow.transform.localPosition += norm;
        float deg = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        CursorArrow.transform.eulerAngles = new Vector3(0, 0, deg + 90);
        CursorStick.transform.eulerAngles = new Vector3(0, 0, deg + 90);
        DragOn = true;
    }

    /// <summary>
    /// 生成した矢印を破棄する
    /// </summary>
    public void ArrowDest() {
        Destroy(CursorArrow);
        Destroy(CursorStick);
        DragOn = false;
    }
}
