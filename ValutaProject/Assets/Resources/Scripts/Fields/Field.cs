using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CardZone各ClassのManagerClass
public class Field : MonoBehaviour
{
    [SerializeField]
    private DraftManager DraftManager;
    public DraftManager GetDraftZone() { return DraftManager; }

    [SerializeField]
    private DisCardManager DisCardManager;
    public DisCardManager GetDisCardZone() { return DisCardManager; }

    [SerializeField]
    private DeckManager DeckManager;
    public DeckManager GetDeckZone() { return DeckManager; }

    public void Init() {
        DraftManager.Init();
        DisCardManager.Init();
        DeckManager.Init();
    }

    public CardZone FindCard(Card card) {
        if (DraftFindCard(card)) return DraftManager;
        else if (DeckFindCard(card)) return DeckManager;
        else if (DestFindCard(card)) return DisCardManager;
        else return null;
    }

    public void DraftAddCard(Card card) { DraftManager.AddCard(card); }
    public bool DraftFindCard(Card card) { return DraftManager.FindCard(card); }
    public bool isDraftCharge() { return DraftManager.isCharge(); }
    public void DraftCharge() { DraftManager.DraftCharge(); }

    public void DeckAddCard(Card card) { DeckManager.AddCard(card); }
    public List<Card> DeckGetCard(int count = 1) { return DeckManager.GetCards(count); }
    public bool DeckFindCard(Card card) { return DeckManager.FindCard(card); }

    public void DestAddCard(Card card) { DisCardManager.AddCard(card); }
    public bool DestFindCard(Card card) {return DisCardManager.FindCard(card); }
}
