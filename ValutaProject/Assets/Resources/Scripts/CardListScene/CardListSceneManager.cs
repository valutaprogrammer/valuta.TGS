using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Sort_Order {
    normal,//昇順
    revers,//降順
}
public enum Sort_Type {
    ID,
    name,
    aligment,
    type,
    cost,
    deffence,
}

public class CardListSceneManager : MonoBehaviour, ICardSort
{
    [SerializeField]
    public List<Card> Cards;
    [NonSerialized]
    public List<Card> List;//並び順保管用　CardsをID保管用を兼ねて変更不可

    [SerializeField]
    public int width, height;//1pageにカードが縦横何枚ずつ入るか

    [SerializeField]
    private GameObject SelectCardPos;
    [NonSerialized]
    public GameObject SelectCard;

    [SerializeField]
    private GameObject CardsParent;
    private Vector2 pageOffset = new Vector2(1000, 0);
    [NonSerialized]
    public int page,pageMax;
    //public int pageMax = 0;

    [SerializeField]
    private Text pageText;

    [SerializeField]
    private GameObject FilterBox;
    [SerializeField]
    private CardAligment filter_Aligment = CardAligment.All;
    [SerializeField]
    private List<GameObject> filter_Aligment_Buttons = new List<GameObject>();
    [SerializeField]
    private List<ObjectType> filter_CardType = new List<ObjectType>();
    [SerializeField]
    private List<GameObject> filter_CardType_Buttons = new List<GameObject>();

    [SerializeField]
    private Sort_Order sort_Order;

    [SerializeField]
    private Sort_Type sort_Type;
    [SerializeField]
    private List<GameObject> sort_Order_Buttons = new List<GameObject>();
    [SerializeField]
    private List<GameObject> sort_Type_Buttons = new List<GameObject>();

    public static CardListSceneManager LIST;
    private void Awake() {
        LIST = this;
    }

    private void Start() {
        Filter_Aligment(0);
        Filter_CardType(0);
        Sort_Order(0);
        Sort_Type(0);
        defaultPos = CardsParent.transform.position;
        List<CardState> CardStates = CardList.GetCardList();
        CardState recomend = null;
        foreach (CardState c in CardStates) {
            if (c == recomend) continue;
            AddCard(InitCard(c));
            recomend = c;
        }
        List = new List<Card>(Cards);
        SortUpdate();
    }

    public void Init() {
        Filter_Aligment(0);
        Filter_CardType(0);
        Sort_Order(0);
        Sort_Type(0);
        FilterChange();
        FilterBox.SetActive(false);
    }

    /// <summary>
    /// 選択したカードを拡大して表示する処理
    /// </summary>
    /// <param name="card"></param>
    public void CardSelect(Card card) {
        GameObject c = Instantiate(card.gameObject);
        c.transform.SetParent(SelectCardPos.transform);
        c.transform.localScale = c.transform.localScale * 3;
        c.transform.localPosition = Vector2.zero;
        if (SelectCardPos != null) Destroy(SelectCard);
        SelectCard = c;
    }

    /// <summary>
    /// ページを進むボタン
    /// </summary>
    public void NextButton() {
        if (page >= pageMax) return;
        page++;
        PageUpdate();
    }

    /// <summary>
    /// ページを戻るボタン
    /// </summary>
    public void BackButton() {
        if (page <= 0) return;
        page--;
        PageUpdate();
    }

    /// <summary>
    /// ページ表示の更新処理
    /// </summary>
    private void PageUpdate(){
        CardsParent.transform.position = defaultPos - (pageOffset * page);
    }


    private CardAligment def_Aligment;
    private List<ObjectType> def_CardType = new List<ObjectType>();
    /// <summary>
    /// フィルタ＆ソートのBoxの表示を切り替える処理
    /// </summary>
    /// <param name="isActive"></param>
    public void Filter_Sort_Button(bool isActive) {
        FilterBox.SetActive(isActive);
        if (isActive) {
            def_Aligment = filter_Aligment;
            def_CardType = new List<ObjectType>(filter_CardType);
        }
        else {
            Filter_Aligment((int)def_Aligment);
            Filter_CardType(0);
            foreach (ObjectType t in def_CardType) {
                Filter_CardType((int)t, true);
            }
        }
    }

    /// <summary>
    /// カードの陣営フィルタ
    /// </summary>
    /// <param name="num"></param>
    public void Filter_Aligment(int num) {
        filter_Aligment_Buttons[(int)filter_Aligment].GetComponent<Image>().color = Color.white;
        filter_Aligment_Buttons[num].GetComponent<Image>().color = Color.black;
        CardAligment aligment = (CardAligment)Enum.ToObject(typeof(CardAligment), num);
        if (filter_Aligment == aligment) return;
        filter_Aligment = aligment;
        //FilterChange();
    }

    /// <summary>
    /// カードの種類フィルタ
    /// </summary>
    /// <param name="num"></param>
    /// <param name="isSet">trueなら、既に選択済みでも選択を解除しない</param>
    public void Filter_CardType(int num,bool isSet) {
        if (isSet) {
            foreach (ObjectType t in filter_CardType)
                if ((int)t == num) return;
        }
        else Filter_CardType(num);
        //if (!Filter_CardType(num) && isSet) Filter_CardType(num);
    }

    /// <summary>
    /// カードの種類フィルタ
    /// </summary>
    /// <param name="num"></param>
    public void Filter_CardType(int num){
        filter_CardType_Buttons[num].GetComponent<Image>().color = Color.black;
        ObjectType cardType = (ObjectType)Enum.ToObject(typeof(ObjectType), num);
        List<ObjectType> types = new List<ObjectType>(filter_CardType);

        if (cardType == ObjectType.All) {
            foreach (ObjectType t in types) {
                filter_CardType.Remove(t);
                filter_CardType_Buttons[(int)t].GetComponent<Image>().color = Color.white;
            }
            return;
        }

        foreach (ObjectType t in types) {
            //既に選択済みなら解除する
            if (t == cardType) {
                filter_CardType.Remove(t);
                filter_CardType_Buttons[(int)t].GetComponent<Image>().color = Color.white;
                return;
            } 
        }
        filter_CardType_Buttons[(int)ObjectType.All].GetComponent<Image>().color = Color.white;

        filter_CardType.Add(cardType);
        return;
    }

    /// <summary>
    /// フィルタの更新
    /// </summary>
    public void FilterChange() {
        List = new List<Card>();
        foreach (Card c in Cards){
            if (filter_Aligment == CardAligment.All || c.State.cardAligment == filter_Aligment || c.State.cardAligment == CardAligment.All)
            {
                if (filter_CardType.Count == 0) List.Add(c);
                else foreach (ObjectType t in filter_CardType) if (t == c.State.cardType) List.Add(c);
                        else { c.gameObject.transform.position = new Vector2(0, -1000); }
            }
            else { c.gameObject.transform.position = new Vector2(0, -1000); }
        }
        SortUpdate();
        //Filter_Sort_Button(false);
        FilterBox.SetActive(false);
    }

    /// <summary>
    /// 並び順ソート
    /// </summary>
    public void Sort_Order(int num) {
        sort_Order_Buttons[(int)sort_Order].GetComponent<Image>().color = Color.white;
        sort_Order = (Sort_Order)Enum.ToObject(typeof(Sort_Order), num);
        sort_Order_Buttons[num].GetComponent<Image>().color = Color.black;
    }

    /// <summary>
    /// 種類ソート
    /// </summary>
    public void Sort_Type(int num) {
        sort_Type_Buttons[(int)sort_Type].GetComponent<Image>().color = Color.white;
        sort_Type = (Sort_Type)Enum.ToObject(typeof(Sort_Type), num);
        sort_Type_Buttons[num].GetComponent<Image>().color = Color.black;
    }

    
    delegate void testDel(Sort_Order order);
    public void CardSortUpdate() {
        switch (sort_Type) {
            case global::Sort_Type.ID:
                List.Sort((a, b) =>
                {
                    int res = a.State.ID - b.State.ID;
                    return Order(res);
                });
                break;
            case global::Sort_Type.name:
                //List.Sort((a, b) => SortString(a.State.cardName, b.State.cardName));
                List.Sort((a, b) =>
                {
                    int res = string.Compare(a.State.cardName_Read, b.State.cardName_Read);
                    if (res == 0) return a.State.ID - b.State.ID;
                    return Order(res);
                });
                break;
            case global::Sort_Type.aligment:
                List.Sort((a, b) =>
                {
                    int res = a.State.cardAligment - b.State.cardAligment;
                    if (res == 0) return a.State.ID - b.State.ID;
                    return Order(res);
                });
                break;
            case global::Sort_Type.type:
                List.Sort((a, b) =>
                {
                    int res = a.State.cardType - b.State.cardType;
                    if (res == 0) return a.State.ID - b.State.ID;
                    return Order(res);
                });
                break;
            case global::Sort_Type.cost:
                List.Sort((a, b) =>
                {
                    int res = a.State.Cost - b.State.Cost;
                    if (res == 0) return a.State.ID - b.State.ID;
                    return Order(res);
                });
                break;
            case global::Sort_Type.deffence:
                List.Sort((a, b) =>
                {
                    int res = a.State.defence - b.State.defence;
                    if (res == 0) return a.State.ID - b.State.ID;
                    return Order(res);
                });
                //ループ内でList内の情報を変更するため、参照用に初期状態を保存しておく
                List<Card> _cards = new List<Card>(List);
                foreach (Card c in _cards)
                {
                    if (c.State.defence <= 0 && List.Remove(c)) List.Add(c);
                }
                break;
        }
    }

    private int Order(int x) {
        return sort_Order == global::Sort_Order.normal ? x : -x;
    }

    /// <summary>
    /// カードの生成
    /// </summary>
    /// <param name="cardState"></param>
    /// <returns></returns>
    public Card InitCard(CardState cardState)
    {
        GameObject cardObj = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
        Card card = SetCardType(cardObj, cardState.cardType);
        card.State = cardState;
        card.name = cardState.cardName;
        card.GetComponent<uicard>().Init();
        return card;
    }

    /// <summary>
    /// カードの追加
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(Card card)
    {
        Cards.Add(card);
        card.gameObject.transform.SetParent(CardsParent.transform);
    }
    
    private Card SetCardType(GameObject card, ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Unit: return card.AddComponent<Unit>();
            case ObjectType.Skill: return card.AddComponent<Skill>();
            case ObjectType.Support: return card.AddComponent<Support>();
            case ObjectType.Trap: return card.AddComponent<Trap>();
            case ObjectType.Life: return card.AddComponent<Life>();
            default: return card.AddComponent<Card>();
        }
    }

    [SerializeField]
    private Vector2 defaultPos;//カード配置時の中心点

    public List<Vector2> GetCardSort(List<Card> card)
    {
        if (card.Count == 0) return null;
        Vector2 scale = card[0].transform.localScale;
        Vector2 size = card[0].GetComponent<RectTransform>().sizeDelta * Constants.CardOffsetRatio;
        size = new Vector2(size.x * scale.x, size.y * scale.y);
        List<Vector2> pos = new List<Vector2>();
        Vector2 leftUpSide = new Vector2(-(size * (width - 1) / 2).x, (size * (height - 1) / 2).y);//端の位置を計算

        pageMax = 0;
        for (int i = 0; i < card.Count; i++) {
            Vector2 p = leftUpSide + (i / (width * height) * pageOffset) + new Vector2(size.x * (i % width), -size.y * (i % (width * height) / width));
            pos.Add(p);
            pageMax = i / (width * height);
        }
        return pos;
    }

    public void CardSort(List<Card> card)
    {
        List<Vector2> pos = GetCardSort(List);
        for (int i = 0; i < List.Count; i++)
        {
            List[i].Move(pos[i] + defaultPos, 0);
        }
    }

    public void SortUpdate()
    {
        page = 0;
        PageUpdate();
        CardSortUpdate();
        CardSort(List);
        if (SelectCard) Destroy(SelectCard);
    }
}
