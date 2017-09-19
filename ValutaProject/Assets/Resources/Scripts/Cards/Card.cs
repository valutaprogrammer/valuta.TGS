using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードの属性
public enum CardAligment{
    All,//両方
    SowrdMan,//剣士
    Magician,//魔術師
    None,//無し
}

public enum ObjectType {
    All,
    Unit,
    Skill,
    Support,
    Trap,
    Life,
    Player,
}

[System.Serializable]
public class CardState {
    public CardState() { ID = Constants.CardCount++; }
    public readonly int ID;
    //名称
    public string cardName;
    //読み仮名
    public string cardName_Read;

    public Sprite cardSprite;
    public bool isSpriteOnFrame = true;//false;//カード画像に既に枠が含まれているか否か

    //カードの種類
    public ObjectType cardType;

    //属性
    public CardAligment cardAligment;

    //使用時の消費コスト
    public int Cost;

    //カードテキスト
    public string text;

    //攻撃力
    public int atk
    {
        get
        {
            EffectState power = GetPower(EffectType.AttackPower);
            if (power != null) return power.value;
            else return 0;
        }
        set
        {
            SetPower(EffectType.AttackPower, value);
        }
    }

    //耐久力
    public int defence;

    //スキルダメージ
    public int skill
    {
        get
        {
            EffectState power = GetPower(EffectType.SkillPower);
            if (power != null) return power.value;
            else return 0;
        }
        set
        {
            SetPower(EffectType.SkillPower, value);
        }
    }

    //支援カードの破棄条件・罠カードの発動条件
    public TriggerType trigger;

    //効果
    [SerializeField]
    public List<CardEffect> Effect = new List<CardEffect>();
    public void SetEffect(EffectPlayType pType,EffectType etype, int value, int turn = 0, Target target = null,int cost = 0,TriggerType trigger = TriggerType.None,GameObject animEffect = null) {
        if (etype == EffectType.AttackPower || etype == EffectType.SkillPower) {
            SetPower(etype, value);
            return;
        }
        CardEffect effect;
        switch (pType) {
            case EffectPlayType.triggerEffect:
                effect = new TriggerEffect() { TriggerType = trigger };
                break;
            case EffectPlayType.staticEffect:
                effect = new StaticEffect();
                break;
            case EffectPlayType.playEffect:
                effect = new PlayEffect();
                break;
            case EffectPlayType.activeEffect:
                effect = new ActiveEffect(cost);
                break;
            case EffectPlayType.none:
            default:
                effect = new CardEffect();
                break;
        }
        //カード効果に応じてアニメーション設定
        if(animEffect == null)
        switch (etype) {
            case EffectType.CostAdd:
            case EffectType.UnitCostAdd:
            case EffectType.SkillDamageAdd:
            case EffectType.AttackAdd:
            case EffectType.AttackTime:
                    animEffect = null;
                    break;
            case EffectType.CardDest:
            case EffectType.RandCardDest: break;
            case EffectType.CardDraft:
            case EffectType.CardDraw:
            case EffectType.GetDisZoneCard:
            case EffectType.CardUnDraft:
            case EffectType.GetDispCard: break;
            case EffectType.Counter:break;
            case EffectType.Damage:
                    animEffect = Resources.Load<GameObject>(Constants.CARD_EFFECT_MAGICK_FIRE);
                break;
            case EffectType.Heal:
                    animEffect = Resources.Load<GameObject>(Constants.CARD_EFFECT_HEAL);
                    break;
        }
        effect.effectState.Add(new EffectState(etype, value, turn, target, animEffect));
        Effect.Add(effect);
    }

    /// <summary>
    /// 効果対象を指定する必要があるか否か
    /// </summary>
    /// <returns></returns>
    public bool isTargetSelect() {
        foreach (CardEffect e in Effect) {
            foreach (EffectState estate in e.effectState)
                if (estate.Target != null && !estate.Target.isAll) return true;
        }
        return false;
    }

    public CardEffect GetEffect(EffectType eType){
        foreach (CardEffect e in Effect) {
            foreach (EffectState estate in e.effectState)
                if (estate.effectType == eType) return e;
        }
        return null;
    }

    protected EffectState GetPower(EffectType type) {
        foreach (CardEffect _effect in Effect)
            if (_effect.effectState.Count > 0 && _effect.effectState[0].effectType == type)
                return _effect.effectState[0];
        return null;
    }
    protected void SetPower(EffectType type,int value) {
        //再設定する前に効果を初期化
        foreach (CardEffect CE in Effect)
            if (CE.effectState.Count > 0 && CE.effectState[0].effectType == type)
                Effect.Remove(CE);
        if (value != 0)
        {
            CardEffect e = new StaticEffect();
            e.effectState.Add(new EffectState(type, value));
            Effect.Add(e);
        }
    }
}

//カード基底クラス
[RequireComponent(typeof(CardInput))]
public class Card : MonoBehaviour
{
    [SerializeField]
    private CardState state;
    public CardState State {
        get { return state; }
        set { state = value; }
    }

    public CardRuntimeState RunTimeState = new CardRuntimeState();

    //カード一枚ずつが持つ情報
    public struct CardRuntimeState {
        public bool isSet;//カードが裏側か
        public bool isActiveEffectPlayed;//カードがこのターン起動効果を発動したか

        //ターン開始時、一ターンのみの変数を初期化
        public void Refresh() {
            //Debug.Log("refreshed");
            isActiveEffectPlayed = false; }
    }

    public bool GetIsSet() { return RunTimeState.isSet; }
    public void SetIsSet(bool value) {
        RunTimeState.isSet = value;
        uicard ui = GetComponent<uicard>();
        if (ui) {
            if (value) ui.CardSet();
            else ui.CardOpen();
        }
    }

    //カード発動 使用コストを返す
    public virtual int Play(GameObject target = null) {
        Constants.BATTLE.CardPlay(this,target);
        return state.Cost;
    }

    //カード破壊
    public virtual void Dest() {
        //Constants.BATTLE.CardDest(this);
        Constants.BATTLE.AddStack(new CardDestBlock(this,Constants.BATTLE.FindCardHolder(this)));
    }

    //カード破棄
    public virtual void Disp() {
        Constants.BATTLE.CardDisp(this);
    }

    public virtual void ActiveEffectPlay() {
        if (!isActiveEffectPlay()) return;
        Debug.Log("起動効果を発動します");
        Constants.BATTLE.CardActiveEffecPlay(this);
    }

    public virtual bool isActiveEffectPlay(int? cost = null,bool isAI = false) {
        return Constants.BATTLE.isActiveEffectPlay(this,isAI,cost);
    }

    public virtual bool isPlay(int haveCost,CardAligment aligment = CardAligment.All) {
        return (isAligment(aligment) && haveCost >= state.Cost);
    }

    private bool isAligment(CardAligment aligment) {
        if (state.cardAligment == CardAligment.All) return true;
        if (state.cardAligment == aligment) return true;
        return false;
    }

    public void Move(Vector3 pos,float t = 0.35f) {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (MoveToCor != null) {
            StopCoroutine(MoveToCor);
        }
        if (t > 0)
            MoveToCor = StartCoroutine(MoveTo(pos, t));
        else transform.position = pos;
    }

    private Coroutine MoveToCor;
    private RectTransform rect;
    IEnumerator MoveTo(Vector3 pos,float t) {
        Constants.Sound.CallSE(Constants.Sound.card_Move);
        //Vector3 nowPos = transform.position;
        Vector3 goal = pos - transform.parent.position;
        Vector3 nowPos = transform.localPosition;
        //Vector3 dis = pos - nowPos;
        Vector3 dis = pos - transform.parent.position - nowPos;
        float nowT = Time.deltaTime;
        while (t > nowT) {
            //transform.position = nowPos + (dis * (nowT / t));
            transform.localPosition = nowPos + (dis * (nowT / t));
            yield return null;
            nowT += Time.deltaTime;
        }
        //transform.position = pos;
        transform.localPosition = goal;
        MoveToCor = null;
    }

    public void AnchorMove(Vector3 pos, float t = 0.25f) {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (MoveToCor != null)
        {
            StopCoroutine(MoveToCor);
        }
        if (t > 0)
            MoveToCor = StartCoroutine(MoveTo(pos, t));
        else {
            transform.position = pos;
        }
    }

    IEnumerator AnchorMoveTo(Vector3 pos, float t = 0.25f) {
        Vector3 nowPos = rect.anchoredPosition;
        Vector3 dis = pos - nowPos;
        float nowT = Time.deltaTime;
        while (t > nowT)
        {
            rect.anchoredPosition = nowPos + (dis * (nowT / t));
            yield return null;
            nowT += Time.deltaTime;
        }
        rect.anchoredPosition = pos;
        MoveToCor = null;
    }
}
