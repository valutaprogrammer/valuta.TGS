using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectZone : CardZone, ICardSort{

    public override bool AddCard(Card card, bool isInit = false)
    {
        if (!base.AddCard(card, isInit)) return false;
        CardSort(Cards);
        return true;
    }

    public override bool RemoveCard(Card card)
    {
        if (!base.RemoveCard(card)) return false;
        CardSort(Cards);
        return true;
    }

    //カードを並べる
    public List<Vector2> GetCardSort(List<Card> card){
        if (card.Count == 0) return null;
        RectTransform c = card[0].GetComponent<RectTransform>();
        Vector2 size = new Vector2(c.sizeDelta.x * c.localScale.x, 0);
        size = size * 1.1f;
        List<Vector2> pos = new List<Vector2>();
        Vector2 leftSide = -(size * (card.Count - 1) / 2);//端の位置を計算
        for (int i = 0; i < card.Count; i++) pos.Add(leftSide + (i * size));
        return pos;
    }

    public void CardSort(List<Card> card){
        List<Vector2> pos = GetCardSort(card);
        for (int i = 0; i < card.Count; i++){
            card[i].Move(pos[i] + new Vector2(transform.position.x, transform.position.y));
            //card[i].transform.position = pos[i] + defaultPos;
        }
    }

    public void SortUpdate(){
        CardSort(Cards);
    }
}
