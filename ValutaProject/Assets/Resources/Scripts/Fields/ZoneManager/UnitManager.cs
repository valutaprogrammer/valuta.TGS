using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ユニットゾーンClass
public class UnitManager : CardStayZone {
    public List<CardZone> UnitPos;
    private const int UNIT_POS_IN_COUNT = 1;//UnitPos内の一つのzoneに入るカード枚数

    public override List<Card> GetCards()
    {
        List<Card> cards = new List<Card>();
        foreach (CardZone zone in UnitPos) {
            List<Card> zoneCards = zone.GetCards();
            foreach (Card card in zoneCards)
                cards.Add(card);
        }
        return cards;
    }

    public override void Tick()
    {
        foreach (Card c in GetCards())
            c.RunTimeState.Refresh();
    }

    //攻撃力の取得
    public override int GetAtack()
    {
        int atk = 0;
        List<Card> cards = GetCards();
        foreach (Card card in cards)
            atk += card.State.atk;

        return atk;
    }

    public override int GetCardsCount()
    {
        int count = 0;
        foreach (CardZone zone in UnitPos) { count += zone.GetCardsCount(); }
        return count;
    }

    public override void Init()
    {
        base.Init();
        //この場所ではなくUnitPosにカードを配置するため最大数は0
        SetCardMax(0);
        foreach (CardZone zone in UnitPos)
        {
            zone.Init();
            zone.SetCardMax(UNIT_POS_IN_COUNT);
        }
    }

    public void AligmentUpdate(CardAligment ali) {
        Sprite aliSprite = null;
        if (ali == CardAligment.Magician) aliSprite = Resources.Load<Sprite>(Constants.UNITZONE_ALIGMENT_MAGICK);
        else if(ali == CardAligment.SowrdMan) aliSprite = Resources.Load<Sprite>(Constants.UNITZONE_ALIGMENT_SOWRD);
        //Debug.Log(ali + "" + aliSprite);

        foreach (CardZone pos in UnitPos) {
            Image posImage = pos.GetComponent<Image>();
            if (posImage != null) {
                posImage.sprite = aliSprite;
                if (posImage.sprite != null) posImage.color = new Color(1, 1, 1, 1);
                else posImage.color = new Color(0, 0, 0, 0);
            }
        }
    }

    public override CardZoneType GetCardZoneType()
    {
        return CardZoneType.Unit;
    }

    public override bool AddCard(Card card,bool isInit = false)
    {
        foreach (CardZone zone in UnitPos) {
            if (zone.isAddCard()) {
                zone.AddCard(card,isInit);
                return true;
            }
        }
        return false;
    }

    

    public override bool isAddCard(int count = 1)
    {
        int space = 0;
        foreach (CardZone zone in UnitPos) {
            if (zone.isAddCard()) space++;
        }
        return (space >= count);
    }

    public override bool RemoveCard(Card card)
    {
        foreach (CardZone zone in UnitPos) {
            if (zone.RemoveCard(card)) return true;
        }
        return false;
    }

    public override bool FindCard(Card card)
    {
        foreach (CardZone zone in UnitPos) {
            if (zone.FindCard(card)) return true;
        }
        return false;
    }

    public override void FindCardType(ObjectType t, List<GameObject> finds, CardAligment aligment = CardAligment.All)
    {
        foreach (CardZone zone in UnitPos) {
            zone.FindCardType(t, finds,aligment);
        }
    }

    public override int GetCardMax() { return UnitPos.Count; }
}
