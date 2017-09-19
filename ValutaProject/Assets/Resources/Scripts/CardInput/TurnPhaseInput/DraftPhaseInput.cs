using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftPhaseInput : ICardInput {
    //InputManaegr parent;

    //生成元のInputManagerを取得
    public DraftPhaseInput(InputManaegr parent) {
        //this.parent = parent;
    }

    //各phase毎のUpdate処理
    public void InputUpdate() { }

    //このphaseの開始
    public void PhaseStart() { }

    //このPhaseの終了
    public void PhaseEnd() { }

    //カードのクリック
    public void CardClick(Card card) {
        //選択されたカードがドラフトゾーンの物なら手札に加える
        if (Constants.BATTLE.FindCardStayZone(card).GetType() == typeof(DraftManager)) {
            Constants.BATTLE.DraftDraw(card);
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
