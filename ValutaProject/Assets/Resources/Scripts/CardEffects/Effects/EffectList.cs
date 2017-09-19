using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTypeCheck {

    //引数のEffectTypeに応じたEffectの派生classを返す 　////常駐型等の実行タイミングの無いものと不明な引数の場合はnull
    public static Effect GetEffect(EffectType t) {
        switch (t) {
            case EffectType.AttackPower:
                return null;
            case EffectType.SkillPower:
                return null;
            case EffectType.AttackAdd:
            //case EffectType.AttackAdd_NextTurn:
                return new AttackAdd();
            case EffectType.SkillDamageAdd:
                return new SkillDamageAdd();
            case EffectType.Guardian:
                return null;
            case EffectType.Damage:
            case EffectType.DamageOnUnit:
            case EffectType.DamageOnPlayer:
            case EffectType.DamaegOnAll:
                return new Damage();
            case EffectType.CardDest:
                return new CardDest();
            case EffectType.RandCardDest:
                return new RandCardDest();
            case EffectType.CostAdd:
                return new CostAdd();
            case EffectType.UnitCostAdd:
                return new UnitCostAdd();
            case EffectType.AttackTime:
                return new AttackTimeAdd();
            case EffectType.CardDraw:
                return new CardDraw();
            case EffectType.CardDraft:
                return new CardDraft();
            case EffectType.GetDispCard:
                return new GetDispCard();
            case EffectType.GetDisZoneCard:
                return new CardSeach();
            case EffectType.CardUnDraft:
                return new CardUnDraft();
            case EffectType.DamageAdd:
                return new DamageAdd();
            //case EffectType.Interceptor:
            //    return new InterCepter();
            case EffectType.Counter:
                return new Counter();
            case EffectType.Heal:
                return new Heal();
            case EffectType.GameEnd:
                return new GameEnd();
            default:return null;
        }
    }
}

//エフェクトの効果
public enum EffectType {
    AttackPower,
    AttackAdd,       //攻撃力の操作                       (value,turn)
    Damage,         //プレイヤーかユニットへのダメージ    (value,target)
    DamageOnUnit,   //ユニットへのダメージ                (value,target)
    DamageOnPlayer, //プレイヤーへのダメージ              (value)
    DamaegOnAll,    //相手の全ての対象にダメージ          (value)
    CardDest,       //カードの破壊                        (value)
    RandCardDest,   //ランダムなカードの破壊              (value)
    CostAdd,        //コスト増加                          (value)
    UnitCostAdd,    //指定ユニットのコスト分コスト増加    (value,target)
    SkillPower,
    SkillDamageAdd, //スキルダメージの増加                (value,turn)
    AttackTime,     //攻撃回数の操作                      (value)
    CardDraw,       //カードを引く                        (value)
    CardDraft,      //x枚から一枚選択してカードを引く     (value)
    CardUnDraft,    //デッキの上からx枚裏向きに出し、相手が選ばなかったカードを手札に加える(value)
    GetDispCard,    //破棄カードの獲得                    (target)
    GetDisZoneCard, //墓地のカードの獲得                  (target)
    DamageAdd,     //受けるダメージの操作                 (value)
    Counter,        //カードの効果を無効にする            (target)
    Guardian,       //このカード以外を攻撃できない        (caster)
    Heal,           //ライフの回復                        (value)
    GameEnd,        //自身はゲームに敗北する              (caster)
}

public class Effect
{
    /// <summary>
    /// 効果の発動
    /// </summary>
    /// <param name="card">この効果を発動するカード</param>
    /// <param name="state">カード効果のパラメータ</param>
    public virtual IEnumerator Play(Player caster, Card card, EffectState state)
    {
        //Debug.Log(GetType());
        yield return null;
    }

    public virtual IEnumerator AnimationStart(GameObject anim,GameObject target) {
        yield return Constants.BATTLE.StartCoroutine(Constants.ANIMATION_EFFECT.EffectStart(anim, target));
    }
}

//エフェクトの効果　※効果読み込み時に取得する情報のみ持つ 発動時に決定する情報はClass:CardEffectで指定
[System.Serializable]
public class EffectState : Effect
{
    //効果種別
    public EffectType effectType;
    //演出
    public GameObject animation;
    //効果量
    public int value;
    //効果時間
    public int turn;
    //効果対象
    public Target Target;

    private Player _caster;

    private Card _card;

    public EventBlock trigger;

    private GameObject _target;
    public void SetTarget(GameObject target) { _target = target; }
    public GameObject GetTarget() { return _target; }

    //private EffectState state;

    public EffectState(EffectType etype,int value,int turn = 0,Target target = null,GameObject animation = null) {
        effectType = etype;
        this.value = value;
        this.turn = turn;
        Target = target;
        this.animation = animation;
    }

    private void StateInit(Player caster, Card card, EffectState state,EventBlock trigger = null) {
        _caster = caster;
        _card = card;
        if (trigger != null) {
            this.trigger = trigger;
            Debug.Log(trigger.GetType());
        }
            
        //this.state = state;
    }

    /// <summary>
    /// 効果の発動
    /// </summary>
    public void AddStack(Player caster, Card card, EffectState state,EventBlock trigger = null) {
        StateInit(caster, card, state,trigger);
        new CardEffectBlock(this, card,caster).AddBlock();
        //Constants.BATTLE.AddStack((new CardEffectBlock(this, card)));
    }

    public override IEnumerator Play(Player caster,Card card, EffectState state)
    {
        StateInit(caster, card, state);
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        //Play();
        yield return caster.StartCoroutine(EffectTypeCheck.GetEffect(effectType).Play(_caster, _card, state));
    }

    public List<GameObject> GetTargets(Player caster) {
        List<GameObject> targets = new List<GameObject>();

        //効果対象を選択する必要があれば選択へ
        if (!Target.isAll)
        {
            GameObject t = GetTarget();
            if (t)
                targets.Add(GetTarget());
            else Debug.Log("効果対象が指定されていません");
        }
        else
            targets = Target.GetTargets(caster, value);

        return targets;
    }

    /// <summary>
    /// 選択可能な対象を取得
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetCanSelectTargets(Player caster){
        return Target.GetTargets(caster);
    }

    /*public void Play() {
        //Debug.Log("発動");
        Constants.BATTLE.StartCoroutine(EffectTypeCheck.GetEffect(effectType).Play(_caster, _card, state));
    }*/
}

public enum DamageType
{
    Attack,
    Skill,
}

//攻撃力操作
public class AttackAdd : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));

        List<GameObject> targets = state.GetTargets(caster);
        Player p;
        foreach (GameObject t in targets) {
            if ((p = t.GetComponent<Player>())) {
                yield return caster.StartCoroutine(AnimationStart(state.animation, t));
                p.Buff.SetState(state);
            }
            /* else if (t.GetComponent<Card>()) { } */
        }
    }
}

//次ターンの攻撃力操作
//public class AttackAdd_NextTurn : AttackAdd { }

public class SkillDamageAdd : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));

        List<GameObject> targets = state.GetTargets(caster);

        foreach (GameObject t in targets){
            if (t.GetComponent<Player>()){
                t.GetComponent<Player>().Buff.SetState(state);
                yield return caster.StartCoroutine(AnimationStart(state.animation, t));
            }
            /* else if (t.GetComponent<Card>()) { }*/
        }
    }
}

//ダメージの発生
public class Damage : Effect {
    public override IEnumerator Play(Player caster,Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = state.GetTargets(caster);

        int mob = 0;
        if (card != null && card.State.cardType == ObjectType.Skill) {
            mob = caster.SkillDamage;
        }
        
        foreach (GameObject obj in targets) {
            yield return caster.StartCoroutine(_Damage(obj, state.value + mob));
        }
    }
    
    public IEnumerator Play(GameObject target,int damage) {
        yield return Constants.BATTLE.StartCoroutine(_Damage(target, damage));
    }

    private IEnumerator _Damage(GameObject target,int damage) {
        IAttackTarget ia = target.GetComponent<IAttackTarget>();
        if (ia == null) yield break; ;
        if (target.GetComponent<Player>()) target = target.GetComponent<Player>().playerImage.gameObject;
        yield return Constants.BATTLE.StartCoroutine(AnimationStart(Constants.ATTACK_ANIMATION, target));
        //DamageEffect.DamageInit(ia.Damage(damage), target);
        yield return Constants.BATTLE.StartCoroutine(ia.Damage(damage));
    }
}

//カード破壊 安森
public class CardDest : Effect
{
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = new List<GameObject>();
        //効果対象を選択する必要があれば選択へ
        if (!state.Target.isAll){
            Debug.Log("効果対象を選択してください");
        }
        else {
            targets = state.Target.GetTargets(caster);
        }

        foreach (GameObject obj in targets)
        {
            dest = caster.StartCoroutine(Play(obj, state));
            Debug.Log(obj.name);
        }
        while (dest != null) yield return null;
    }

    Coroutine dest = null;
    public IEnumerator Play(GameObject target,EffectState state)
    {
        Card card = target.GetComponent<Card>();
        yield return Constants.BATTLE.StartCoroutine(AnimationStart(state.animation, target));
        if (card != null) { card.Dest(); }
        dest = null;
    }
}

public class RandCardDest : Effect{
    public override IEnumerator Play(Player caster, Card card, EffectState state) {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = new List<GameObject>();
        targets = state.Target.GetTargets(caster);

        if (targets.Count == 0) yield break;

        for (int i = 0; i < state.value && targets.Count > 0; i++) {
            int num = Random.Range(0, targets.Count);
            yield return caster.StartCoroutine(Dest(state, targets[num].GetComponent<Card>()));
            targets.Remove(targets[num]);
        }
    }

    private IEnumerator Dest(EffectState state,Card card) {
        if (card == null) yield break;
        yield return Constants.BATTLE.StartCoroutine(AnimationStart(state.animation, card.gameObject));
        card.Dest();
    }
}

//コスト操作 担当者：針ヶ谷天紀
public class CostAdd : Effect
{
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = state.GetTargets(caster);

        foreach (GameObject obj in targets)
        {
            Play(obj, state.value);
            yield return caster.StartCoroutine(AnimationStart(state.animation, obj));
            //Debug.Log(obj.name);
        }
    }

    public void Play(GameObject caster, int cost)
    {
        if (caster.GetComponent<Player>())
        {
            //Debug.Log("cost add to " + caster.name);
            caster.GetComponent<Player>().cost += cost;
            //Debug.Log(caster.GetComponent<Player>().cost);
        }
    }
}

public class UnitCostAdd : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = state.GetTargets(caster);

        CardDestBlock block = state.trigger as CardDestBlock;
        if (block == null) {
            block = Constants.BATTLE.ResolvedBlock as CardDestBlock;
        } 
        int value = 0;
        if (block != null) { foreach (Card c in block.GetCards()) value += c.State.Cost; }
        else { Debug.Log("カード破壊情報を読み取れませんでした"); }

        foreach (GameObject obj in targets) {
            Play(obj, value);
            yield return caster.StartCoroutine(AnimationStart(state.animation, obj));
            Debug.Log(obj.name);
        }
    }

    private void Play(GameObject caster, int cost) {
        if (caster.GetComponent<Player>()) {
            Debug.Log("cost add to " + caster.name);
            caster.GetComponent<Player>().cost += cost;
            Debug.Log(caster.GetComponent<Player>().cost);
        }
    }
}

//攻撃回数操作
public class AttackTimeAdd : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = state.GetTargets(caster);

        foreach (GameObject obj in targets) {
            yield return caster.StartCoroutine(AddAttackCount(obj, state));
        }
    }

    private IEnumerator AddAttackCount(GameObject target,EffectState state) {
        Player p = target.GetComponent<Player>();
        if (p == null) yield break; ;
        p.Buff.SetState(state);
        p.AttackCountUpdate();
        yield return Constants.BATTLE.StartCoroutine(AnimationStart(state.animation, target));
    }
}

public class GetDispCard : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state) {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        yield return caster.StartCoroutine(_GetCard(caster, state.GetTarget().GetComponent<Card>()));
    }

    public IEnumerator _GetCard(Player caster, Card c) {
        yield return new WaitForSeconds(0.5f);
        caster.AddHand(c);
    }
}

//特定カードを手札に加える
public class GetCard : Effect {

}

//カードを山札から手札に加える 担当者：針ヶ谷天紀
public class CardDraw : GetCard
{
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        yield return caster.StartCoroutine(Play(caster, state));
    }

    public IEnumerator Play(Player caster,EffectState state)
    {
        yield return caster.StartCoroutine(AnimationStart(state.animation, caster.gameObject));
        for (int i = 0; i < state.value; i++)
        {
            caster.CardDraw();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

//複数枚から一枚選んで手札に加える
public class CardDraft : GetCard {
    public override IEnumerator Play(Player caster, Card card, EffectState state) {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = state.GetTargets(caster);
        List<Card> cards = new List<Card>();
        Card c = null;
        foreach (GameObject obj in targets) {
            Debug.Log(obj);
            if ((c = obj.GetComponent<Card>()) != null) {
                cards.Add(c);
            }
        }

        Draft(caster, cards);
    }

    protected virtual void Draft(Player caster,List<Card> cards) {
        Constants.CARD_SELECT_EFFECT.Init(caster, cards, Option, result);
    }

    protected virtual void Option(List<Card> cards) { }
    private void result(Player caster, Card res, List<Card> other) {
        caster.AddHand(res);
        Constants.BATTLE.CardDisp(other);
    }
}

public class CardSeach : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state) {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<GameObject> targets = state.GetTargets(caster);
        List<Card> cards = new List<Card>();
        Card c = null;
        foreach (GameObject obj in targets) {
            Debug.Log(obj);
            if ((c = obj.GetComponent<Card>()) != null) {
                cards.Add(c);
            }
        }
        cards.Sort((a, b) => { return a.State.ID - b.State.ID; });
        //重複するカードを削除
        for (int i = 0; i + 1 < cards.Count; i++)
            if (cards[i].State == cards[i + 1].State) cards.Remove(cards[i--]);
        for (int i = 0; i < cards.Count; i++)
            if (!caster.AligmentCheck(cards[i].State.cardAligment)) cards.Remove(cards[i--]);
        if (c)
            yield return caster.StartCoroutine(AnimationStart(state.animation, c.gameObject));
        Draft(caster, cards);
    }

    protected virtual void Draft(Player caster, List<Card> cards) {
        Constants.CARD_SELECT_EFFECT.Init(caster, cards, Option, result);
    }

    protected virtual void Option(List<Card> cards) { }
    private void result(Player caster, Card res, List<Card> other)
    {
        caster.AddHand(res);
        Constants.BATTLE.CardDisp(other);
    }
}

//デッキからカードをn枚引き、相手に一枚選ばせる。選ばれたカードを捨て札に送り、残りは手札に加える
public class CardUnDraft : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        Debug.Log("DraftEffect");
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        List<Card> cards = Constants.BATTLE.CardDraw(state.value);
        yield return caster.StartCoroutine(AnimationStart(state.animation, caster.gameObject));
        yield return caster.StartCoroutine(UnDraft(caster, cards));
        Debug.Log("unDraft 終了");
    }

    protected IEnumerator UnDraft(Player caster, List<Card> cards)
    {
        //一時的ドラフトゾーンを生成し、そこにCardsを追加
        //選択されたら手札に加える
        Debug.Log("選択可能なカードは");
        foreach (Card c in cards)
            Debug.Log(c);
        Debug.Log("です。");

        //相手にカードを選択させる
        Constants.CARD_SELECT_EFFECT.Init(Constants.BATTLE.GetOtherPlayer(caster), cards, Option,result);
        yield return caster.StartCoroutine(Constants.CARD_SELECT_EFFECT.SelectWait());
        Debug.Log("選択終了");
    }

    protected virtual void Option(List<Card> cards) {
        foreach (Card c in cards)
            c.SetIsSet(true);
    }

    private void result(Player target,Card res,List<Card> other) {
        Constants.BATTLE.CardDisp(res);
        Constants.BATTLE.GetOtherPlayer(target).AddHand(other);
        //caster.AddHand(other);
    }
}

//ダメージの操作 例：攻撃を受ける時、そのダメージを-1する
public class DamageAdd : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));

        List<GameObject> targets = state.GetTargets(caster);
        foreach (GameObject t in targets) {
            if (t.GetComponent<Player>()) {
                t.GetComponent<Player>().Buff.SetState(state);
                yield return caster.StartCoroutine(AnimationStart(state.animation, t));
            }
            if (t.GetComponent<Card>()) {

            }
        }
    }
}

//自身の場のカードを任意数破壊し、枚数分の被ダメージ軽減
//public class InterCepter : DamageAdd { }

// カードの効果を無効にする
public class Counter : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        CardBlock target = Constants.BATTLE.EventStack.Peek() as CardBlock;
        EffectCounter();
        if (target != null && target.GetCards().Count > 0)
            yield return caster.StartCoroutine(AnimationStart(state.animation, target.GetCards()[0].gameObject));
    }

    private void EffectCounter() {
        Constants.BATTLE.DeleteStack();
    }
}

//攻撃対象を自身のみにする
//public class Guardian : Effect { }

public class Heal : Effect {
    public override IEnumerator Play(Player caster, Card card, EffectState state)
    {
        yield return base.Play(caster, card, state);
        List<GameObject> targets = new List<GameObject>();
        targets = state.Target.GetTargets(caster);
        foreach (GameObject obj in targets)
        {
            yield return caster.StartCoroutine(Play(obj, state.value, state));
        }
    }

    public IEnumerator Play(GameObject target, int heal,EffectState state)
    {
        Player targetPlayer = target.GetComponent<Player>();
        if (targetPlayer)
        {
            yield return Constants.BATTLE.StartCoroutine(_Heal(targetPlayer, heal, state));
        }
    }
    private IEnumerator _Heal(Player target, int heal,EffectState state)
    {
        IAttackTarget ia = target.GetComponent<IAttackTarget>();
        GameObject obj = target.playerImage.gameObject;
        if (ia == null) yield break;
        yield return Constants.BATTLE.StartCoroutine(AnimationStart(state.animation, obj));
        yield return Constants.BATTLE.StartCoroutine(target.Heal(heal));
        //DamageEffect.DamageInit(ia.Damage(-heal), obj);
        //yield return Constants.BATTLE.StartCoroutine(ia.Damage(heal));
    }
}

//ドラフトゾーンのカードを全て捨て札に送る
public class DraftRefresh : Effect{
    public override IEnumerator Play(Player caster, Card card, EffectState state){
        yield return caster.StartCoroutine(base.Play(caster, card, state));
        yield return caster.StartCoroutine(_DraftRefresh(state));
    }

    private IEnumerator _DraftRefresh(EffectState state) {
        DraftManager draftZone = Constants.BATTLE.Field.GetDraftZone();
        yield return Constants.BATTLE.StartCoroutine(AnimationStart(state.animation, draftZone.gameObject));
        List<Card> draftCards = draftZone.GetCards();
        foreach (Card c in draftCards)
            c.Disp();
        draftZone.DraftCharge();
    }
}

//ゲームに敗北（最後のライフカードの効果）
public class GameEnd : Effect { }