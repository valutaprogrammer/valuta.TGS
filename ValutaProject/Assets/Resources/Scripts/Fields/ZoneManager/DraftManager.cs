using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ドラフトゾーンClass
public class DraftManager : CardZone,ICardSort {
    private const int DRAFT_MAX = 3;//ドラフト数

    public override void Init() {
        SetCardMax(DRAFT_MAX);
        base.Init();
    }

    public override CardZoneType GetCardZoneType() {
        return CardZoneType.Draft;
    }

    public void DraftCharge() {
        if (cards.Count == 0)
            AddCard(Constants.BATTLE.CardDraw(GetCardMax()));
        //Debug.Log(cards.Count);
    }

    public bool isCharge() {
        if (Cards.Count <= 0) return true;
        return false;
    }

    public override bool AddCard(Card card,bool isInit = false)
    {
        if (!base.AddCard(card,isInit)) return false;
        //Debug.Log("add Draft");
        CardSort(Cards);
        return true;
    }

    public override bool RemoveCard(Card card)
    {
        if (!base.RemoveCard(card)) return false;
        //CardSort(Cards);
        if (cards.Count == 0) DraftCharge();
        return true;
    }

    //[SerializeField]
    //private Vector2 sortOffset = new Vector2(5.0f, 0);//カード配置時の余剰幅
    private Vector2 defaultPos;//カード配置時の中心点

    public List<Vector2> GetCardSort(List<Card> card)
    {
        if (card.Count == 0) return null;
        RectTransform cardRect = card[0].GetComponent<RectTransform>();
        Vector2 size = new Vector2(cardRect.sizeDelta.x * cardRect.localScale.x * Constants.CardOffsetRatio, 0);
        List<Vector2> pos = new List<Vector2>();
        Vector2 leftSide = -(size * (card.Count - 1) / 2);//端の位置を計算
        for (int i = 0; i < card.Count; i++) pos.Add(leftSide + (i * size));
        return pos;
    }

    public void CardSort(List<Card> card)
    {
        defaultPos = transform.position;
        List<Vector2> pos = GetCardSort(card);
        for (int i = 0; i < card.Count; i++) {
            card[i].Move(pos[i] + defaultPos);
            //card[i].transform.position = pos[i] + defaultPos;
        }
    }

    public void SortUpdate() {
        CardSort(Cards);
    }
}
