using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDestPhaseInput : ICardInput {
    //private InputManaegr parent;
    private Player p;

    public HandDestPhaseInput(InputManaegr parent) {
        //this.parent = parent;
    }

    //各phase毎のUpdate処理
    public void InputUpdate() { }

    //このphaseの開始
    public void PhaseStart() {
        p = Constants.BATTLE.GetActivePlayer();
    }

    //このPhaseの終了
    public void PhaseEnd() {
        p = null;
    }

    //カードのクリック
    public void CardClick(Card card) {
        //Debug.Log("handdest");
        if (Constants.BATTLE.FindCardStayZone(card).GetType() == typeof(HandManager)) {
            if (Constants.BATTLE.FindCardHolder(card) == p) {
                Constants.BATTLE.CardDisp(card);
                if (p.GetHandZone().isAddCard()) {
                    Constants.BATTLE.HandDestEnd();
                }
            }
        }
    }

    //カードのクリック・ドラッグ終了
    public void RemoveCardClick(Card card) { }

    //カードにマウスが乗った時
    public void PointerCard(Card card) { }

    //カードからマウスが離れる時
    public void RemovePointerCard(Card card) { }

    //カードゾーンにマウスが乗った時
    public void PointerCardZone(CardZone zone) { }

    //カードゾーンからマウスが離れた時
    public void RemovePointerCardZone(CardZone zone) { }

    //プレイヤーのクリック
    public void PlayerClick(Player player) { }
    //プレイヤーにマウスが乗った時
    public void PointerPlayer(Player player) { }

    //プレイヤーからマウスが離れたとき
    public void RemovePointerPlayer(Player player) { }

    //プレイヤーからのドラッグ処理が終了した時
    public void RemovePlayerDrag(Player player) { }
}
