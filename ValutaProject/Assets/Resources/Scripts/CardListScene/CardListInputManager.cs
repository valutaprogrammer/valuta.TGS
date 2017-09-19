using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListInputManager : InputManaegr {


    //カードクリック開始
    protected override void SetCardClick(Card card)
    {
        CardListSceneManager.LIST.CardSelect(card);
    }



    protected override void Update()
    {
        //base.Update();
    }

    //カードゾーンにマウスが乗った時
    protected override void SetPointerCardZone(CardZone value){ }

    //カードゾーンからマウスが離れた時
    protected override void RemovePointerCardZone(CardZone value){ }

    //カードにマウスが乗った時
    protected override void SetPointerCard(Card value){ }

    protected override void RemovePointerCard(Card value){ }

    //カードクリックの終了
    protected override void RemoveCardClick(Card card){ }

    //プレイヤーのクリック
    protected override void PlayerClick(Player player){ }

    //プレイヤーにマウスが乗った時
    protected override void PointerPlayer(Player player){ }

    protected override void RemovePointerPlayer(Player player){ }

    //プレイヤーからのドラッグ処理が終了した時
    protected override void RemovePlayerDrag(Player player){ }
}
