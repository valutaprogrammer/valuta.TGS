using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulliganPhaseInput : ICardInput
{
    /*private Player ActivePlayer;
    private Player NonActivePlayer;*/
    Player[] players;
    private MulliganManaegr mulliganManaegr;
    private HandManager handManager;

    public MulliganPhaseInput(InputManaegr parent){

    }
    //各phase毎のUpdate処理
    public void InputUpdate() { }

    //このphaseの開始
    public void PhaseStart()
    {
        players = Constants.BATTLE.Players;
        foreach (Player p in players) p.GetMulliganManager().MulliganStart();
    }

    //このPhaseの終了
    public void PhaseEnd()
    {
        foreach (Player p in players)
            p.GetMulliganManager().gameObject.SetActive(false);
        Constants.BATTLE.Field.GetDeckZone().Shuffle();
    }
    //カードのクリック
    public void CardClick(Card card)
    {
        //Debug.Log(Constants.BATTLE.FindCardStayZone(card).GetType());
        foreach (Player p in players)
        {
            //選択されたカードがハンドゾーンの物ならマリガンゾーンに加える
            if (Constants.BATTLE.FindCardStayZone(card) == p.GetHandZone())
            {
                if ((Constants.BATTLE.FindCardHolder(card) == p) && !p.GetMulliganManager().GetPlayerCheck())
                {
                    p.GetMulliganManager().AddCard(card);
                }
            }
            else if (Constants.BATTLE.FindCardStayZone(card) == p.GetMulliganManager())
            {
                if (Constants.BATTLE.FindCardHolder(card) == p && !p.GetMulliganManager().GetPlayerCheck())
                {
                    p.GetHandZone().AddCard(card);
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
