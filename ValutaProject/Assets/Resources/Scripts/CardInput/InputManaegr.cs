using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//入力の種類
public enum InputType {
    onClick,//クリックされた時
    removeClick,//クリックが解除された時
    onPointer,//マウスが乗った時
    removePointer,//マウスが離れた時
}
//入力の対象
public enum InputTargetType {
    card,
    cardZone,
    player,
}

public class InputManaegr : MonoBehaviour {
    public static InputManaegr IM;
    [SerializeField]
    public Canvas canvas;
    [System.NonSerialized]
    public RectTransform cRect;
    [System.NonSerialized]
    public Vector3 canvasSize;
    [System.NonSerialized]
    public Vector3 canvasScale;

    [SerializeField]
    private ICardInput activeInput;
    private ICardInput oldActiveInput;
    private MainPhaseInput mainInput;
    private DraftPhaseInput draftInput;
    private HandDestPhaseInput handDestInput;
    private MulliganPhaseInput mulliganInput;

    void Awake() {
        IM = this;
        cRect = canvas.GetComponent<RectTransform>();
        canvasSize = cRect.sizeDelta;
        canvasScale = cRect.localScale;
        mainInput = new MainPhaseInput(this);
        draftInput = new DraftPhaseInput(this);
        handDestInput = new HandDestPhaseInput(this);
        mulliganInput = new MulliganPhaseInput(this);
        activeInput = mainInput;
        oldActiveInput = mainInput;
    }

    protected virtual void Update()
    {
        RectUpdate();
        //各フェイズ毎のタッチ処理
        switch (Constants.BATTLE.GetTurnPhase()) {
            //ドラフト処理
            case GamePhase.draft:
                activeInput = draftInput;
                break;

            //メイン処理
            case GamePhase.main:
                activeInput = mainInput;
                break;

            //それ以外
            case GamePhase.aligment:
            case GamePhase.coinTos:
                break;
            case GamePhase.mulligan:
                activeInput = mulliganInput;
                break;
            case GamePhase.start:
            case GamePhase.end: break;
            case GamePhase.handDest:
                activeInput = handDestInput;
                break;
            case GamePhase.gameEnd:
            default:
                return;
        }
        if (activeInput != oldActiveInput) {
            oldActiveInput.PhaseEnd();
            activeInput.PhaseStart();
            oldActiveInput = activeInput;
        }
        activeInput.InputUpdate();
        //oldClick = cardOnClick;

        UpdateLongTap();
    }

    private void RectUpdate() {
        if (canvasSize.x != cRect.sizeDelta.x || canvasSize.y != cRect.sizeDelta.y) {
            Debug.Log("canvasSizeの値が更新されました");
            canvasSize = cRect.sizeDelta;
            canvasScale = cRect.localScale;
        } 
    }

    /// <summary>
    /// 入力を受け取り、各イベントへ派生させる
    /// </summary>
    /// <param name="inputType">入力の種類</param>
    /// <param name="targetType">入力対象の種類</param>
    /// <param name="target">入力対象オブジェクト</param>
    public void SetInput(InputType inputType,InputTargetType targetType,GameObject target) {
        switch (targetType) {
            //カード入力
            case InputTargetType.card:
                Card card = target.GetComponent<Card>();
                if (!card) return;
                switch (inputType) {
                    case InputType.onClick:
                        SetCardClick(card);
                        break;
                    case InputType.onPointer:
                        SetPointerCard(card);
                        break;
                    case InputType.removeClick:
                        RemoveCardClick(card);
                        break;
                    case InputType.removePointer:
                        RemovePointerCard(card);
                        break;
                }
                break;
            //カードゾーン入力
            case InputTargetType.cardZone:
                CardZone cardZone = target.GetComponent<CardZone>();
                if (!cardZone) return;
                switch (inputType)
                {
                    case InputType.onClick:
                        break;
                    case InputType.onPointer:
                        SetPointerCardZone(cardZone);
                        break;
                    case InputType.removeClick:
                        break;
                    case InputType.removePointer:
                        RemovePointerCardZone(cardZone);
                        break;
                }
                break;
            //プレイヤー入力
            case InputTargetType.player:
                Player player = target.GetComponent<Player>();
                if (!player) return;
                switch (inputType)
                {
                    case InputType.onClick:
                        PlayerClick(player);
                        break;
                    case InputType.onPointer:
                        PointerPlayer(player);
                        break;
                    case InputType.removeClick:
                        RemovePlayerDrag(player);
                        break;
                    case InputType.removePointer:
                        RemovePointerPlayer(player);
                        break;
                }
                break;
        }
    }

    //カードゾーンにマウスが乗った時
    protected virtual void SetPointerCardZone(CardZone value) {
        activeInput.PointerCardZone(value);
    }

    //カードゾーンからマウスが離れた時
    protected virtual void RemovePointerCardZone(CardZone value) {
        activeInput.RemovePointerCardZone(value);
    }

    //カードにマウスが乗った時
    protected virtual void SetPointerCard(Card value) {
        activeInput.PointerCard(value);
        //Constants.CARD_BUTTONS.CardClick(value);
    }

    protected virtual void RemovePointerCard(Card value) {
        activeInput.RemovePointerCard(value);
    }

    //カードクリック開始
    protected virtual void SetCardClick(Card card){
        StartTap(card.gameObject);
    }

    //カードクリックの終了
    protected virtual void RemoveCardClick(Card card) {
        activeInput.RemoveCardClick(card);
    }

    //プレイヤーのクリック
    protected virtual void PlayerClick(Player player) {
        if (!player.isAI)
            StartTap(player.gameObject);
    }

    //プレイヤーにマウスが乗った時
    protected virtual void PointerPlayer(Player player) {
        activeInput.PointerPlayer(player);
    }

    protected virtual void RemovePointerPlayer(Player player) {
        activeInput.RemovePointerPlayer(player);
    }

    //プレイヤーからのドラッグ処理が終了した時
    protected virtual void RemovePlayerDrag(Player player) {
        activeInput.RemovePlayerDrag(player);
    }

    private void StartTap(GameObject obj) {
        CancelLongTap();
        clickObj = obj;
        clickPos = Input.mousePosition;
    }

    private void UpdateLongTap() {
        if (clickObj == null || !Input.GetMouseButton(0) || Vector2.Distance(clickPos, Input.mousePosition) > LongTapDistance) {
            CancelLongTap();
            return;
        }
        tapTime += Time.deltaTime;
        if (tapTime >= LongTapTime) {
            LongTapEnter(clickObj);
        }
    }

    private void LongTapEnter(GameObject obj) {
        if (obj.GetComponent<Player>()){
            PlayerLongTap(obj.GetComponent<Player>());
        }
        else if (obj.GetComponent<Card>()){
            CardLongTap(obj.GetComponent<Card>());
        }
        tapTime = 0;
        clickPos = Vector2.zero;
        clickObj = null;
    }
    
    private void CancelLongTap() {
        tapTime = 0;
        clickPos = Vector2.zero;
        if (clickObj != null) {
            Card card = clickObj.GetComponent<Card>();
            Player p = clickObj.GetComponent<Player>();
            if (p) {
                if (!p.isAI) {
                    activeInput.PlayerClick(clickObj.GetComponent<Player>());
                    if (!Input.GetMouseButton(0))
                        activeInput.RemovePlayerDrag(clickObj.GetComponent<Player>());
                }
            }
            else if (card) {
                p = Constants.BATTLE.FindCardHolder(card);
                if (p == null || !p.isAI) {
                    activeInput.CardClick(card);
                    if (!Input.GetMouseButton(0))
                        activeInput.RemoveCardClick(card);
                    if (Constants.CARD_SELECT_EFFECT.Zone.FindCard(card)) Constants.CARD_SELECT_EFFECT.CardSelect(card);
                }
            }
        }
        clickObj = null;
    }

    private GameObject clickObj;
    private float tapTime;
    private const float LongTapTime = 0.5f;
    private Vector2 clickPos;
    private const float LongTapDistance = 50.0f;
    //カードの長押し
    private void CardLongTap(Card card) {
        Constants.CARD_BUTTONS.CardClick(card);
    }

    //プレイヤーの長押し
    private void PlayerLongTap(Player player) {

    }
}
