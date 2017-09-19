using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//手札ゾーンClass
public class HandManager : CardZone,ICardSort{
    public const int HAND_MAX = 7;

    public bool isHandRevers;

    public override void Init()
    {
        SetCardMax(HAND_MAX);
        base.Init();
        defaultPos = transform.position;
    }

    public override CardZoneType GetCardZoneType(){
        return CardZoneType.Hand;
    }

    //手札は上限以上のカード数を保持できる
    public override bool isAddCard (int count = 1) { return true; }

	//手札上限を超えてカードを所有しているか確認
	public bool isHandOver(){ return (cards.Count > HAND_MAX); }

    public override bool AddCard(Card card,bool isInit = false)
    {
        if (!base.AddCard(card,isInit)) return false;
        card.SetIsSet(isHandRevers);
        CardSort(Cards);
        return true;
    }

    public override bool RemoveCard(Card card)
    {
        if (!base.RemoveCard(card)) return false;
        CardSort(Cards);
        return true;
    }

    //[SerializeField]
    //private Vector2 sortOffset = new Vector2(160.0f, 0);//カード配置時の余剰幅
    private Vector2 defaultPos;//カード配置時の中心点

    const float canvasSizeX = 768;
    //カードを並べる
    public List<Vector2> GetCardSort(List<Card> card)
    {
        if (card.Count == 0) return null;
        RectTransform c = card[0].GetComponent<RectTransform>();
        Vector2 size = new Vector2(c.sizeDelta.x * c.localScale.x, 0);
        List<Vector2> pos = new List<Vector2>();
        Vector2 leftSide = -(size * (card.Count - 1) / 2);//端の位置を計算
        //手札が画面からはみ出しそうなら間を詰める
        if (leftSide.x < -canvasSizeX / 2 + size.x / 2) {
            size.x = canvasSizeX / card.Count;
            leftSide.x = (-canvasSizeX / 2) + size.x / 2;
        }

        for (int i = 0; i < card.Count; i++) pos.Add(leftSide + (i * size));
        return pos;
    }

    public void CardSort(List<Card> card)
    {
        List<Vector2> pos = GetCardSort(card);
        for (int i = 0; i < card.Count; i++) {
            card[i].Move(pos[i] + defaultPos);
        }
    }

    public void SortUpdate() {
        CardSort(Cards);
    }
}
