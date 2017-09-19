using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardDeck {
    Card GetTopCard();
    List<Card> GetCards(int count);
}
