using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulliganManaegr : CardZone
{
    [SerializeField]
    private GameObject EnterButton;//決定ボタン
    private Player[] players;
    private bool PlayerCheck = false;
    private void OnPlayerCheck() { PlayerCheck = true; }
    public bool GetPlayerCheck() { return PlayerCheck; }
    private Vector2 defaultPos;
    public const float wait = 0.1f;

    public void MulliganStart()
    {
        gameObject.SetActive(true);
        if (!Constants.BATTLE.FindCardZoneHolder(this).isAI) EnterButton.SetActive(true);
        else EnterButton.SetActive(false);
        defaultPos = transform.position;
        players = Constants.BATTLE.Players;
        AIScript ai;
        if ((ai = Constants.BATTLE.FindCardZoneHolder(this).GetComponent<AIScript>())) StartCoroutine(ai.Mulligun());
    }

    public void MulliganCardChange()
    {
        int x = GetCardsCount();
        for (int i = 0; i < x; i++)
        {
            Constants.BATTLE.FindCardZoneHolder(this).CardDraw();
            StartCoroutine(CardRestore());
        }
        Constants.BATTLE.Field.GetDeckZone().Shuffle();
    }

    //戻す枚数分カードを引く処理
    public IEnumerator CardDraw() {
        int x = GetCardsCount();
        Player p = Constants.BATTLE.FindCardZoneHolder(this);
        for (int i = 0; i < x; i++) {
            p.CardDraw();
            yield return new WaitForSeconds(wait);
        }
    }

    //カードをデッキに戻す処理
    public IEnumerator CardRestore() {
        while (cards.Count > 0) {
            Constants.BATTLE.Field.DeckAddCard(cards[0]);
            yield return new WaitForSeconds(wait);
        }
    }

    //マリガン決定処理
    public void OnEnterButton()
    {
        if (EnterButton != null) EnterButton.SetActive(false);
        OnPlayerCheck();
        //全てのプレイヤーがマリガンを終えていたら、手札を引き直す
        bool isEnd = true;
        foreach (Player p in players)
            if (!p.GetMulliganManager().GetPlayerCheck()) isEnd = false;
        if (isEnd) StartCoroutine(Constants.BATTLE.MulliganEnter());
        /*
        if (players[0].GetMulliganManager().GetPlayerCheck() && players[1].GetMulliganManager().GetPlayerCheck())
        {
            StartCoroutine(Constants.BATTLE.MulliganEnter());
        }
        */
    }

    public override bool AddCard(Card card, bool isInit = false)
    {
        bool isRevers = card.GetIsSet();
        if (!base.AddCard(card, isInit)) return false;
        card.SetIsSet(isRevers);
        CardSort(Cards);
        return true;
    }

    public override bool RemoveCard(Card card)
    {
        if (!base.RemoveCard(card)) return false;
        CardSort(Cards);
        return true;
    }

    public List<Vector2> GetCardSort(List<Card> card)
    {
        if (card.Count == 0) return null;
        RectTransform c = card[0].GetComponent<RectTransform>();
        Vector2 size = new Vector2(c.sizeDelta.x * c.localScale.x, 0);
        List<Vector2> pos = new List<Vector2>();
        Vector2 leftSide = -(size * (card.Count - 1) / 2);//端の位置を計算
        for (int i = 0; i < card.Count; i++) pos.Add(leftSide + (i * size));
        return pos;
    }

    public void CardSort(List<Card> card)
    {
        List<Vector2> pos = GetCardSort(card);
        for (int i = 0; i < card.Count; i++)
        {
            card[i].Move(pos[i] + defaultPos);
        }
    }

    public void SortUpdate()
    {
        CardSort(Cards);
    }
}
