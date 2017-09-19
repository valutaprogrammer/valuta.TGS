using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードを配置する場の種類
public enum CardZoneType {
    Deck,
    Life,
    Draft,
    Dest,
    Hand,
    Unit,
    Support,
}


//各カードゾーン基底Class
[RequireComponent(typeof(CardZoneInput))]
public class CardZone : MonoBehaviour
{
    //配置されるカード
    [SerializeField]
    protected List<Card> cards = new List<Card>();
    protected List<Card> Cards { get { return cards; }set { cards = value; } }
    public virtual int GetCardsCount() { return Cards.Count; }
    [SerializeField]
    private int CardMax = 1000;
    public virtual void SetCardMax(int max) { CardMax = max; }
    public virtual int GetCardMax() { return CardMax; }//カード最大配置数

    //初期化
    public virtual void Init() {
        Cards = new List<Card>();
    }

    public virtual CardZoneType GetCardZoneType() { return CardZoneType.Dest; }

    //シャッフル
    public void Shuffle() {
        for (int i = 0; i < Cards.Count; i++) {
            Card c = Cards[i];
            int r = Random.Range(0, Cards.Count);
            Cards[i] = Cards[r];
            Cards[r] = c;
        }
    }

    //ターン終了時に情報更新
    public virtual void Tick() {
        foreach (Card c in cards)
            c.RunTimeState.Refresh();
    }

    //カードをこの場に移動する
    public virtual bool AddCard(List<Card> card,bool isInit = false) {
        if (isInit)
        {
            foreach (Card c in card)
            {
                AddCard(c, isInit);
            }
        }
        /*foreach (Card c in card) {
            AddCard(c,isInit);
        }*/
        else StartCoroutine(AddCardCor(card, isInit));
        return true;
    }

    public virtual List<Card> GetCards() {
        return Cards;
    }

    private IEnumerator AddCardCor(List<Card> card,bool isInit = false) {
        foreach (Card c in card)
        {
            AddCard(c, isInit);
            yield return new WaitForSeconds(0.2f);
        }
    }

    //カードをこの場に加える　（一枚）
    public virtual bool AddCard(Card card,bool isInit = false) {
        CardZone zone = null;

        if ((zone = Constants.BATTLE.FindCardStayZone(card)) != null) {
            zone.RemoveCard(card);
        }

        Cards.Add(card);
        card.transform.SetParent(transform);
        if (isInit)
            card.Move(transform.position, 0);
        else card.Move(transform.position, 0.5f);
        //card.transform.position = transform.position;
        //card.transform.localScale = transform.localScale;
        return true;
    }

    //カードをこの場に加える (複数枚)
    public bool AddCard(List<CardState> cardState,bool isInit = false) {
        List<Card> cards = new List<Card>();
        foreach (CardState c in cardState) {
            cards.Add(Constants.BATTLE.InitCard(c));
        }
        return AddCard(cards,true);
    }

    //カードが配置可能か 引数countは配置する数 
    public virtual bool isAddCard(int count = 1) {
        //カード数が最大以上ならfalseを返す
		//現在配置されているカード数+配置予定のカード数が上限を上回ったらfalse
		return (Cards.Count + count <= GetCardMax());
    }

    //カードを他の場に移動する
    public virtual bool RemoveCard(Card card) {
        if (Cards.Remove(card)){
            card.SetIsSet(false);
            return true;
        }
        else return false;
    }

    //引数のCardがここのCardsに含まれるか
    public virtual bool FindCard(Card card) {
        foreach (Card c in Cards) if (c == card) return true;
        return false;
    }
    
    //引数のタイプの対象がこの場にあるか確認し、Listに格納して返す
    public virtual void FindCardType(ObjectType t,List<GameObject> finds,CardAligment aligment = CardAligment.All) {
        foreach (Card card in Cards) {
            if ((card.State.cardType == t || t == ObjectType.All) &&
                (aligment == CardAligment.All || card.State.cardAligment == aligment))
                finds.Add(card.gameObject);
        }
    }

    //ランダムなカードを取得
    public Card GetRandCard() {
        if (GetCardsCount() <= 0) return null;
        int rand = Random.Range(0, GetCardsCount());
        return Cards[rand];
    }
}
