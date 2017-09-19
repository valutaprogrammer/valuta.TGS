using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CardSelect_Option(List<Card> card);
public delegate void CardSelect_Callback(Player caster,Card res,List<Card> otherCard);

public class CardSelectEffect : MonoBehaviour {

    [SerializeField]
    public CardSelectZone Zone;
    [SerializeField]
    private RectTransform image;
    private Player target;//カードを選択するプレイヤー

    public bool isWaiting = false;//カード選択を待機中か否か

    void Awake() {
        Constants.CARD_SELECT_EFFECT = this;
        //if (Zone != null) Zone.gameObject.SetActive(false);
        ImageSizeUpdate();
    }

    /// <summary>
    /// カード選択処理の呼び出し
    /// </summary>
    /// <param name="target">カードを選択するプレイヤー</param>
    /// <param name="cards">選択候補のカード</param>
    /// <param name="_option">選択候補のカードに行う処理　例：カードを裏向きにする</param>
    /// <param name="_call">カードが選択された際に行う処理</param>
    public void Init(Player target, List<Card> cards,CardSelect_Option _option,CardSelect_Callback _call) {
        MessageUI.Message.AddMessage("カードを選択してください");
        this.target = target;
        Zone.Init();
        foreach (Card c in cards)
            Zone.AddCard(c);
        ImageSizeUpdate();
        //Zone.gameObject.SetActive(true);
        _option(cards);
        this._call = _call;
        isWaiting = true;

        if (target.GetComponent<AIScript>()) StartCoroutine(target.GetComponent<AIScript>().CardSelect(cards));
    }

    public void CardSelect(Card card)
    {
        Zone.RemoveCard(card);
        //caster.AddHand(card);
        Dest(card);
    }

    CardSelect_Callback _call;
    //カードが選択された後、他のカードを破棄する処理
    public void Dest(Card card) {
        List<Card> cards = Zone.GetCards();
        /*foreach (Card c in cards) {
            Constants.BATTLE.Field.DestAddCard(c);
        }*/
        Zone.Init();
        ImageSizeUpdate();
        if (_call != null) _call(target, card, cards);
        isWaiting = false;
        //Zone.gameObject.SetActive(false);
    }

    public IEnumerator SelectWait() {
        while (isWaiting)
            yield return null;
    }

    private void ImageSizeUpdate() {

        List<Vector2> pos = Zone.GetCardSort(Zone.GetCards());
        if (pos == null || pos.Count == 0){
            ImageSizeChange(Vector2.zero);
            return;
        }
        Vector2 min = pos[0];
        Vector2 max = pos[0];
        foreach (Vector2 p in pos) {
            if (p.x > max.x) max.x = p.x;
            else if (p.x < min.x) min.x = p.x;
            if (p.y > max.y) max.y = p.y;
            else if (p.y < min.y) min.y = p.y;
        }
        RectTransform cRect = Zone.GetCards()[0].GetComponent<RectTransform>();
        Vector2 localSize = cRect.transform.localScale;
        Vector2 cardSize = new Vector2(cRect.sizeDelta.x * localSize.x, cRect.sizeDelta.y * localSize.y);
        Vector2 offset = cardSize * 0.1f;
        ImageSizeChange(max - min + cardSize + offset);
    }

    private void ImageSizeChange(Vector2 size) {
        image.sizeDelta = size;
    }

    
}
