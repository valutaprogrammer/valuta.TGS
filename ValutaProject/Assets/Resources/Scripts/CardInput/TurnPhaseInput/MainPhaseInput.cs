using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPhaseInput : ICardInput {

    public MainPhaseInput(InputManaegr parent) { this.parent = parent; }

    private InputManaegr parent;
    //マウスの乗っているカードゾーン
    private CardZone pointerCardZone;
    //マウスの乗っているカード
    private Card pointerCard;
    //ドラッグ中のカード
    private Card draggingCard;
    private bool isHandCard;
    //private Card oldDraggingCard;
    //現在クリック・ドラッグ中のカードが配置されている場所
    private CardZone cardHoldZone;
    private Player activePlayer;
    //現在ドラッグ中のプレイヤー
    private Player dragPlayer;
    //ドラッグ先のプレイヤー
    private Player pointerPlayer;

    //各phase毎のUpdate処理
    public void InputUpdate() {
        CardDragMove();
        //oldDraggingCard = draggingCard;
    }

    //このphaseの開始
    public void PhaseStart() {
        pointerCardZone = null;
        pointerCard = null;
        draggingCard = null;
        //oldDraggingCard = null;
        cardHoldZone = null;
        //Debug.Log("mainStart");
        activePlayer = Constants.BATTLE.GetActivePlayer();
        activePlayer.playerImage.SetIsActive(true);
    }

    //このPhaseの終了
    public void PhaseEnd() {
        //Debug.Log("mainEnd");
        SetClickCard(null);
        if (activePlayer != null)
            activePlayer.playerImage.SetIsActive(false);
        activePlayer = null;
        Constants.CARD_BUTTONS.CardRemove();
    }

    //カードのクリック
    public void CardClick(Card card) {
        SetClickCard(card);
        RemovePointerCard(card);
    }

    //カードのクリック・ドラッグ終了
    public void RemoveCardClick(Card card) {
        if (draggingCard == card){
            CardPlay();
            SetClickCard(null);
        }
    }

    private void SetClickCard(Card card) {
		if (Constants.BATTLE.FindCardStayZone(card)){
			draggingCard = card;
            cardRect = card.GetComponent<RectTransform>();
            isHandCard = (Constants.BATTLE.FindCardHolder(card) == activePlayer && Constants.BATTLE.FindCardStayZone(card).GetType() == typeof(HandManager));
            cardHoldZone = Constants.BATTLE.FindCardStayZone(card);
        }
        else {
            cardRect = null;
            isHandCard = false;
            cardHoldZone = null;
        }
    }

    //カードにマウスが乗った時
    public void PointerCard(Card card){
        pointerCard = card;
    }

    //カードからマウスが離れる時
    public void RemovePointerCard(Card card) {
        if (pointerCard == card) pointerCard = null;
    }

    //カードゾーンにマウスが乗った時
    public void PointerCardZone(CardZone zone) {
        pointerCardZone = zone;
    }

    //カードゾーンからマウスが離れた時
    public void RemovePointerCardZone(CardZone zone) {
        if (pointerCardZone == zone) pointerCardZone = null;
    }

    //プレイヤーのクリック
    public void PlayerClick(Player player) {
        dragPlayer = player;
        pointerPlayer = null;
    }

    //プレイヤーにマウスが乗った時
    public void PointerPlayer(Player player) {
        pointerPlayer = player;
    }

    //プレイヤーからマウスが離れたとき
    public void RemovePointerPlayer(Player player) {
        if (pointerPlayer == player) pointerPlayer = null;
    }

    //プレイヤーからのドラッグ処理が終了した時
    public void RemovePlayerDrag(Player player) {
        if (activePlayer == null) return;
        if (dragPlayer == activePlayer)
        {
            if (pointerCard != null && Constants.BATTLE.FindCardHolder(pointerCard) != dragPlayer && Constants.BATTLE.FindCardStayZone(pointerCard).GetType() == typeof(UnitManager) && pointerCard.GetComponent<IAttackTarget>() != null && Constants.BATTLE.IsAttackTarget(pointerCard.gameObject))
            {
                ///攻撃を試みる
                Debug.Log("Attack as " + dragPlayer + " to " + pointerCard);
                activePlayer.Attack(pointerCard.gameObject);
                //activePlayer.playerImage.SetIsActive(false);
                pointerCard = null;
            }
            else if (pointerPlayer != null && pointerPlayer != activePlayer && Constants.BATTLE.IsAttackTarget(pointerPlayer.gameObject))
            {
                ///攻撃を試みる
                Debug.Log("Attack as " + dragPlayer + " to " + pointerPlayer);
                activePlayer.Attack(pointerPlayer.gameObject);
                //activePlayer.playerImage.SetIsActive(false);
                pointerPlayer = null;
            }
            
            else Debug.Log(Constants.BATTLE.FindCardHolder(pointerCard) + ";" + dragPlayer);
        }
        else Debug.Log("none Attack as " + dragPlayer + " to " + pointerPlayer);

        dragPlayer = null;
    }

    private RectTransform cardRect;
    //カードをドラッグで移動させる
    private void CardDragMove(){
        if (draggingCard && isHandCard && activePlayer.isCardPlay(draggingCard)){
            //オブジェクトのRectTransformを１度だけ取得
            if (cardRect == null) cardRect = draggingCard.GetComponent<RectTransform>();

            //Vector3 size = new Vector3(parent.canvasSize.x * parent.canvasScale.x, parent.canvasSize.y * parent.canvasScale.y, 0) / 2;
            Vector3 movePos = Input.mousePosition;// - size;
            //cardRect.anchoredPosition = new Vector3(movePos.x / parent.canvasScale.x, movePos.y / parent.canvasScale.y, movePos.z);
            Vector3 anchorPos = new Vector3(movePos.x / parent.canvasScale.x, movePos.y / parent.canvasScale.y, movePos.z);
            Transform Parent = cardRect.transform.parent;

            while (Parent != null && !Parent.gameObject.GetComponent<Canvas>())
            {
                //cardRect.anchoredPosition -= new Vector2(Parent.localPosition.x, Parent.localPosition.y);
                Parent = Parent.parent;
            }

            draggingCard.AnchorMove(anchorPos, 0);
        }
    }

    //カード使用を試みる
    private void CardPlay() {
        if (!isHandCard) return;

        GameObject target = null;
        if (pointerCard) target = pointerCard.gameObject;
        else if (pointerPlayer) target = pointerPlayer.gameObject;

        //カード使用先が正しいならそこにカードを配置する
		if (isHandCard && draggingCard && Constants.BATTLE.isCardPlay(draggingCard, pointerCardZone,target)){
            //カードを使用
            Player caster = Constants.BATTLE.FindCardHolder(draggingCard);
            if (!caster.CardPlay(draggingCard,target)) CardSort(cardHoldZone);
            Constants.CARD_BUTTONS.CardRemove();
        }
        //違うならば、カードを元の位置に戻す（再ソート）
        else{ CardSort(cardHoldZone); }
    }

    private void CardSort(CardZone zone) {
        ICardSort sort = zone.GetComponent<ICardSort>();
        if (sort != null) sort.SortUpdate();
    }
}
