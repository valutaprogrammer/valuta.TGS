using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ライフカードの情報
public struct LifeState {
    //カードの種類
    List<Life> Cards;
}

//ライフゾーンClass
public class LifeManager : CardZone,ICardDeck {
    const int LIFE_MAX = 10;

    public override void Init()
    {
        SetCardMax(LIFE_MAX);
        base.Init();
        AddCard(CardList.GetLifeCards());
    }

    public override CardZoneType GetCardZoneType()
    {
        return CardZoneType.Life;
    }

    public override bool AddCard(Card card, bool isInit = false)
    {
        if (base.AddCard(card, isInit))
        {
            card.SetIsSet(true);
            return true;
        }
        else return false;
    }

    //デッキの一番上のカードを取得し、デッキからそのカードを排除 カードが無ければnull
    public Card GetTopCard()
    {
        if (Cards.Count <= 0) return null;
        Card card = Cards[Cards.Count - 1];
        RemoveCard(card);
        return card;
    }

    //カードを取得する
    public List<Card> GetCards(int count)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < count; i++)
        {
            Card card = GetTopCard();
            if (card == null) break;
            cards.Add(card);
        }
        return cards;
    }

    public IEnumerator GetDamage(int damage,Player p) {
        if (damage <= 0) yield break;

        Card lastCard = null;
        Card card = null;
        for (int i = 0; i < damage; i++) {
            card = GetTopCard();
            if (card != null) {
                lastCard = card;
                lastCard.Dest();
            }
            yield return new WaitForSeconds(0.2f);
        }
        if (GetCardsCount() <= 0)
            Constants.BATTLE.GameEnd();
        else if (lastCard != null) //StartCoroutine(Constants.BATTLE.CardPlayEffectPlay(lastCard, p));
        {
            //MessageUI.Message.AddMessage("ライフカード発動\n" + lastCard.State.text);
            new CardPlayBlock(lastCard, Constants.BATTLE.FindCardZoneHolder(this)).AddBlock();
        }
            
    }
}
