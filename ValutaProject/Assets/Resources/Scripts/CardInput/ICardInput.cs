using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardInput{

    //各phase毎のUpdate処理
    void InputUpdate();

    //このphaseの開始
    void PhaseStart();

    //このPhaseの終了
    void PhaseEnd();

    //カードのクリック
    void CardClick(Card card);

    //カードのクリック・ドラッグ終了
    void RemoveCardClick(Card card);

    //カードにマウスが乗った時
    void PointerCard(Card card);

    //カードからマウスが離れる時
    void RemovePointerCard(Card card);

    //カードゾーンにマウスが乗った時
    void PointerCardZone(CardZone zone);

    //カードゾーンからマウスが離れた時
    void RemovePointerCardZone(CardZone zone);

    //プレイヤーのクリック
    void PlayerClick(Player player);
    //プレイヤーにマウスが乗った時
    void PointerPlayer(Player player);

    //プレイヤーからマウスが離れたとき
    void RemovePointerPlayer(Player player);

    //プレイヤーからのドラッグ処理が終了した時
    void RemovePlayerDrag(Player player);


    /*以下はコピペ用のコメントアウト

    //各phase毎のUpdate処理
    public void InputUpdate() { }

    //このphaseの開始
    public void PhaseStart() { }

    //このPhaseの終了
    public void PhaseEnd() { }

    //カードのクリック
    public void CardClick(Card card) { }

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
    void RemovePointerPlayer(Player player) { }

    //プレイヤーからのドラッグ処理が終了した時
    public void RemovePlayerDrag(Player player) { }

     */
}
