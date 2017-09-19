using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupportManager : CardStayZone,ICardSort {
    private const int SUPPORT_MAX = 5;

    public override int GetSkill()
    {
        int skill = 0;
        foreach (Card card in Cards)
            skill += card.State.skill;
        return skill;
    }

    public override void Init()
    {
        SetCardMax(SUPPORT_MAX);
        base.Init();
        defaultPos = transform.position;
    }

    public void AligmentUpdate(CardAligment aligment) {
        Sprite sprite = null;
        if (aligment == CardAligment.SowrdMan) { sprite = Resources.Load<Sprite>(Constants.SUPPORTZONE_ALIGMENT_SOWRD);  }
        else if (aligment == CardAligment.Magician) { sprite = Resources.Load<Sprite>(Constants.SUPPORTZONE_ALIGMENT_MAGICK); }
        Image image = GetComponent<Image>();
        if (image) {
            image.sprite = sprite;
            image.SetNativeSize();
        }
    }

    public override CardZoneType GetCardZoneType()
    {
        return CardZoneType.Support;
    }

    public override bool AddCard(Card card,bool isInit = false)
    {
        if (!base.AddCard(card,isInit)) return false;
        CardSort(Cards);
        return true;
    }

    public override bool RemoveCard(Card card)
    {
        if (!base.RemoveCard(card)) return false;
        CardSort(Cards);
        return true;
    }

    /// <summary>
    /// 支援カードの破棄条件を確認し、適合するものを破棄する。確認と破棄が終了するまで待機する。
    /// </summary>
    /// <param name="type">対象の破棄条件</param>
    /// <returns></returns>
    public IEnumerator SupportDestTriggerCheck(TriggerType type) {
        Support supportCard = null;
        List<Card> cards = new List<Card>(Cards);//処理中にCardsから離れる(破壊される)ので、別の場所でデータ保持
        foreach (Card card in cards) {
            supportCard = card.GetComponent<Support>();
            if (supportCard != null) {
                if (supportCard.DestTriggerCheck(type)) {
                    supportCard.Dest();
                    yield return new WaitForSeconds(0.2f);//破壊演出のためにちょっと待機
                } 
                supportCard = null;
            }
        }
    }

    /// <summary>
    /// 発動済みカードの常駐誘発効果を確認し、発動する処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator EffectTriggerCheck(TriggerType type,EventBlock block) {
        Card target = null;
        List<Card> cards = new List<Card>(Cards);//処理中にCardsから離れる(破壊される)ので、別の場所でデータ保持
        foreach (Card card in cards)
        {
            target = card.GetComponent<Support>();
            if (target != null)
            {
                foreach (CardEffect effect in target.State.Effect) {
                    TriggerEffect te = effect as TriggerEffect;
                    if (te != null && te.TriggerType == type) {
                        te.TriggerBlock = block;
                        yield return StartCoroutine(effect.EffectCall(Constants.BATTLE.FindCardZoneHolder(this), target));
                    }
                }
                target = null;
            }
        }
    }

    public List<Card> GetCanPlayTrapCards(TriggerType trigger) {
        int cost = Constants.BATTLE.FindCardZoneHolder(this).cost;
        List<Card> traps = new List<Card>();
        foreach (Card card in Cards) {
            if (card.State.cardType == ObjectType.Trap && card.State.Cost <= cost) {
                if (card.State.trigger == trigger) {
                    traps.Add(card);
                }
            }
        }
        return traps;
    }

    //[SerializeField]
    //private Vector2 sortOffset;//カード配置時の余剰幅
    private Vector2 defaultPos;//カード配置時の中心点
    public const float frameWidheOffset = 0.9f;//支援ゾーン枠の幅
    private Vector2 zoneSize;
    public List<Vector2> GetCardSort(List<Card> card) {
        if (card.Count <= 0) return null;
        zoneSize = GetComponent<RectTransform>().sizeDelta;
        zoneSize *= frameWidheOffset;
        RectTransform cRect = card[0].GetComponent<RectTransform>();
        Vector2 size = new Vector2(cRect.sizeDelta.x * card[0].transform.localScale.x * Constants.CardOffsetRatio, 0);
        List<Vector2> pos = new List<Vector2>();
        Vector2 leftSide = -(size * (card.Count - 1) / 2);//端の位置を計算

        //手札が画面からはみ出しそうなら間を詰める
        if (leftSide.x < -zoneSize.x / 2 + size.x / 2) {
            size.x = zoneSize.x / card.Count;
            leftSide.x = (-zoneSize.x / 2) + size.x / 2;
        }

        for (int i = 0; i < card.Count; i++) pos.Add(leftSide + (i * size));
        return pos;
    }

    public void CardSort(List<Card> card) {
        List<Vector2> pos = GetCardSort(card);
        for (int i = 0; i < card.Count; i++) {
            card[i].Move(pos[i] + defaultPos);
            //card[i].transform.position = pos[i] + defaultPos;
        }
    }

    public void SortUpdate()
    {
        CardSort(Cards);
    }
}
