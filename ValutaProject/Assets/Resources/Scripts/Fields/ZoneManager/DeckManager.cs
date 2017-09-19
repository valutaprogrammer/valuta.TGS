using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//デッキゾーンClass
public class DeckManager : CardZone,ICardDeck {

    public override void Init()
    {
        base.Init();
        AddCard(CardList.GetCardList());
        Shuffle();
    }

    public override CardZoneType GetCardZoneType()
    {
        return CardZoneType.Deck;
    }

    //デッキの一番上のカードを取得し、デッキからそのカードを排除 カードが無ければnull
    public Card GetTopCard() {
        if (Cards.Count <= 0) {
            DeckRefresh();
            if (Cards.Count <= 0) return null;
        } 
        Card card = Cards[Cards.Count - 1];
        RemoveCard(card);
        return card;
    }

    //カードを取得する
    public List<Card> GetCards(int count) {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < count; i++) {
            Card card = GetTopCard();
            if (card == null) break;
            cards.Add(card);
        }
        return cards;
    }

    //デッキが空の時、捨て札のカード(ライフカード以外)をデッキに加えてシャッフルする 一度に大量のカードが動くとクラッシュするので何度かに分ける
    public void DeckRefresh() {
        List<Card> destCards = new List<Card>(Constants.BATTLE.Field.GetDisCardZone().GetCards());
        int rand;
        while (true) {
            rand = Random.Range(0, destCards.Count);
            if (destCards[rand].State.cardType == ObjectType.Life) continue;
            AddCard(destCards[rand], true);
            break;
        }
        if (corRefresh == null)
            corRefresh = StartCoroutine(AllDeckRefresh());
    }

    Coroutine corRefresh;
    private IEnumerator AllDeckRefresh() {
        List<Card> destCards = new List<Card>(Constants.BATTLE.Field.GetDisCardZone().GetCards());
        foreach (Card c in destCards) {
            if (c.State.cardType != ObjectType.Life) {
                AddCard(c,true);
                yield return null;
            }
        }
        Shuffle();
        corRefresh = null;
    }

    public override bool AddCard(Card card,bool isInit = false)
    {
        if (base.AddCard(card,isInit)) {
            card.SetIsSet(true);
            return true;
        } 
        else return false;
    }
}
