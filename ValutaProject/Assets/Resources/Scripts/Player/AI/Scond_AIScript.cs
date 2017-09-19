using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scond_AIScript : MonoBehaviour {
    private Player player;
    [SerializeField]
    bool isStart = false;

    //const int ATK_SCORE = 2,SKILL_SCORE = 3,SUPPORT_SCORE = 2;
    public aiConst ai = null;

	// Use this for initialization
	void Awake () {
        player = GetComponent<Player>();
	}

    void Init() {
        switch (player.cardAligment) {
            case CardAligment.SowrdMan:
                ai = new Swordman_Const();
                break;
            case CardAligment.Magician:
                ai = new Magician_Const();
                break;
        }
    }

    void Update() {
        if (isStart) {
            isStart = false;
            StartCoroutine(Think());
        }
    }

    class PlayerState {
        public PlayerState(Player p,aiConst ai) {
            this.p = p;this.ai = ai;
            life = p.Life; cost = p.cost; atk = p.AttackPower;skill = p.SkillDamage; atkCount = p.Buff.GetMaxAtkCount();atkedCount = p.attackCount;
            hand = new List<Card>(p.GetHandZone().GetCards()); unit = new List<Card>(p.GetUnitZone().GetCards()); support = new List<Card>(p.GetSupportZone().GetCards());Dest = new List<Card>(Constants.BATTLE.Field.GetDisCardZone().GetCards());
            InitActiveEffects();
            //allAction = new List<AI_Action>();
            enemy = new EnemyState(Constants.BATTLE.GetOtherPlayer(p));
        }
        //他のPlayerStateをコピーする
        public PlayerState(PlayerState state) {
            parent = state;
            p = state.p;ai = state.ai;
            life = state.life;cost = state.cost;atk = state.atk;skill = state.skill; atkCount = state.atkCount;atkedCount = state.atkedCount;_score = state._score;actionCount = state.actionCount;score_mod = state.score_mod;
            hand = new List<Card>(state.hand);unit = new List<Card>(state.unit);support = new List<Card>(state.support);Dest = new List<Card>(state.Dest);
            ActiveEffectCards = new List<Card>(state.ActiveEffectCards);
            //allAction = new List<AI_Action>(state.allAction);
            enemy = new EnemyState(state.enemy);
        }

        public void ScoreUpdate() {
            _score = 0;
            if (cost * ai.cost < ai.cost_border)
                _score += cost * ai.cost;
            else _score += ai.cost_border;
            //_score -= enemy.cost;
            _score += skill * ai.skill;
            _score -= enemy.skill * ai.e_skill;
            _score += atk * ai.atk;
            _score += support.Count * ai.support;
            if (hand.Count < 8)//手札が最大数を超えたら評価しない
                _score += hand.Count * ai.hand;
            else _score += 7 * ai.hand;
            if (life < enemy.atk - p.Defence) _score -= 1000;
            else _score -= enemy.atk * ai.e_atk;
            if (enemy.life <= 0) _score = 10000;
            else _score -= enemy.life * ai.e_life;
        }

        void InitActiveEffects() {
            ActiveEffectCards = new List<Card>();
            foreach (Card c in unit)
                if (c.isActiveEffectPlay(cost,true)) ActiveEffectCards.Add(c);
            foreach (Card c in support)
                if (c.isActiveEffectPlay(cost,true)) ActiveEffectCards.Add(c);
        }

        public void PlayActiveEffect(Card c) {
            ActiveEffectCards.Remove(c);
        }

        public void AddUnit(Card c) {
            unit.Add(c);
            if (c.isActiveEffectPlay(cost,true)) {
                ActiveEffectCards.Add(c);
            }
        }
        public bool RemoveUnit(Card c) {
            ActiveEffectCards.Remove(c);
            return unit.Remove(c);
        }

        public void AddSupport(Card c) {
            support.Add(c);
            Debug.Log("支援カード追加");
            if (c.isActiveEffectPlay(cost,true)) {
                ActiveEffectCards.Add(c);
                Debug.Log("起動効果が追加");
            }
        }
        public bool RemoveSupport(Card c){
            ActiveEffectCards.Remove(c);
            return support.Remove(c);
        }

        public Player p;
        public aiConst ai;
        public int life, cost, skill, atk, atkCount, atkedCount, score_mod, actionCount;
        public int _score;
        public int score { get { return _score + score_mod; } }
        public List<Card> hand, unit, support, Dest;
        public List<AI_Action> allAction;
        //public List<PlayerState> History = new List<PlayerState>();
        public PlayerState parent;
        public AI_Action? enterdAction;
        public EnemyState enemy;
        public List<Card> ActiveEffectCards;
    }

    class EnemyState {
        public EnemyState(Player enemy) {
            p = enemy;
            life = enemy.Life; cost = enemy.cost; atk = enemy.AttackPower;
            hand = new List<Card>(p.GetHandZone().GetCards()); unit = new List<Card>(p.GetUnitZone().GetCards()); support = new List<Card>(p.GetSupportZone().GetCards());
            TargetUpdate();
        }
        public EnemyState(EnemyState state) {
            p = state.p;
            life = state.life;cost = state.cost;skill = state.skill;atk = state.atk;
            hand = new List<Card>(state.hand);unit = new List<Card>(state.unit); support = new List<Card>(state.support);
            TargetUpdate();
        }

        public void TargetUpdate() {
            isGuardiun = null;
            foreach (Card c in unit) {
                Unit u = c.GetComponent<Unit>();
                if (u.GetTargetPriority() == Constants.TARGET_GUADIAN) {
                    isGuardiun = u;
                    break;
                }
            }
        }

        public Player p;
        public int life, cost, skill, atk;
        public List<Card> hand, unit, support;
        public Unit isGuardiun = null;//優先攻撃対象がいるか、いるならそのカード
    }

    enum ActionType {
        cardPlay,
        cardDisp,
        activeEffect,
        Attack,
    }

    struct AI_Action {
        public Card card;
        public GameObject target;
        public ActionType actType;
    }

    public IEnumerator Turn() {

        //現在の情報を取得
        PlayerState state = new PlayerState(player,ai);

        //行動の全取得
        GetAllAction(state);

        //行動思考に入る。行動の順番で枝分かれさせていき、最も高いものが実行される
        yield return StartCoroutine(Think(state));
    }

    void GetAllAction(PlayerState state) {
        state.allAction = new List<AI_Action>();
        state.hand.Sort((a, b) => {
            if (a == null) return -1;
            if (b == null) return 1;
            return a.State.ID - b.State.ID;
        });
        CardState cardState = null;
        bool isUnit = false, isSupport = false;

        state.enemy.TargetUpdate();

        //攻撃行動
        if (state.atkCount > state.atkedCount) {
            //優先攻撃対象の確認
            if (state.enemy.isGuardiun) {
                if (state.enemy.isGuardiun.State.defence <= state.atk) {
                    state.allAction.Add(new AI_Action() { actType = ActionType.Attack, target = state.enemy.isGuardiun.gameObject });
                }
            }
            else {
                //対象毎に分岐させる
                foreach (Card c in state.enemy.unit) {
                    if (c == null || cardState == c.State) continue;
                    cardState = c.State;
                    if (c.State.defence <= state.atk)
                        state.allAction.Add(new AI_Action() { actType = ActionType.Attack, target = c.gameObject });
                }
                if (state.enemy.p.Defence < state.atk)
                    state.allAction.Add(new AI_Action() { actType = ActionType.Attack, target = state.enemy.p.gameObject });
            }
        }

        //カード使用
        foreach (Card c in state.hand) {
            //前回と同じカードか空のカードは無視
            if (c == null || cardState == c.State) continue;
            cardState = c.State;
            switch (c.State.cardType) {
                case ObjectType.Unit:
                    isUnit = true;
                    if (state.unit.Count < 2 && c.isPlay(state.cost, state.p.cardAligment))
                        state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardPlay });
                    break;
                case ObjectType.Support:
                //case ObjectType.Trap:
                    isSupport = true;
                    if (state.support.Count < 5 && c.isPlay(state.cost, state.p.cardAligment))
                        state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardPlay });
                    break;
                case ObjectType.Skill:
                    if (c.isPlay(state.cost, state.p.cardAligment)) {
                        if (c.State.isTargetSelect()) {
                            //優先攻撃対象の確認
                            if (state.enemy.isGuardiun) {
                                state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardPlay, target = state.enemy.isGuardiun.gameObject });
                            }
                            else {
                                //対象毎に分岐させる
                                foreach (Card target in state.enemy.unit)
                                    state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardPlay, target = target.gameObject });
                                foreach (ObjectType type in c.State.Effect[0].effectState[0].Target.targetObjectType)
                                    if (type == ObjectType.Player)
                                        state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardPlay, target = state.enemy.p.gameObject });
                            }
                        }
                        else state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardPlay });
                    }
                    break;
            }
        }
        cardState = null;
        //カード起動効果発動
        foreach (Card c in state.ActiveEffectCards) {
            if (c.isActiveEffectPlay(state.cost,true)) {
                if (c == null || cardState == c.State) continue;
                cardState = c.State;
                Debug.Log("起動効果を発動");
                switch (c.State.cardType) {
                    case ObjectType.Unit:
                        state.allAction.Add(new AI_Action() { card = c, actType = ActionType.activeEffect });
                        break;
                    case ObjectType.Support:
                        cardState = c.State;
                        List<Card> targets = new List<Card>();
                        //現在一種類しか起動効果がないので、専用の処理を記述/////////////////////
                        foreach (Card destCard in state.Dest)
                            if (destCard.State.cardType == ObjectType.Skill && destCard.State.cardAligment == CardAligment.Magician)
                                targets.Add(destCard);
                        targets.Sort((a, b) => a.State.ID - b.State.ID);
                        for (int i = 0; i + 1 < targets.Count; i++)
                            if (targets[i].State == targets[i + 1].State) targets.Remove(targets[i--]);
                        /////////////////////////////////////////////////////////////////////////

                        foreach (Card target in targets)
                            state.allAction.Add(new AI_Action() { card = c, actType = ActionType.activeEffect, target = target.gameObject });
                        break;
                }
            }
        }

        //撤退行動を連続させない
        if (state.enterdAction.HasValue && state.enterdAction.Value.actType != ActionType.cardDisp) {
            //カード撤退
            if (state.unit.Count >= 2 && isUnit)
                foreach (Card c in state.unit) {
                    if (c == null || cardState == c.State) continue;
                    cardState = c.State;
                    state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardDisp });
                }
            if (state.support.Count >= 5 && isSupport)
                foreach (Card c in state.support) {
                    if (c == null || cardState == c.State) continue;
                    cardState = c.State;
                    state.allAction.Add(new AI_Action() { card = c, actType = ActionType.cardDisp });
                }
        }
    }

    public IEnumerator Think() {
        PlayerState state = new PlayerState(player,ai);
        GetAllAction(state);
        //Debug.Log(state.allAction.Count);
        if (isThinking == null)
            yield return isThinking = StartCoroutine(Think(state));
        else {
            StopCoroutine(isThinking);
            yield return isThinking = StartCoroutine(Think(state));
        }
    }

    private Coroutine isThinking;
    private IEnumerator Think(PlayerState state) {
        Debug.Log("思考開始");
        List<PlayerState> backup = new List<PlayerState>();
        //Debug.Log(state.allAction.Count);
        backup.Add(state);
        int breakTime = 0;
        float time = 0;//経過時間を計算
        float waitTime = 2.0f;//待機時間を設定

        for (int i = 0; i < backup.Count; i++) {

            foreach (AI_Action act in backup[i].allAction) {
                backup.Add(ActionEnter(backup[i], act));
                breakTime++;
            }

            if (++breakTime > 100) {
                yield return null;
                time += Time.deltaTime;
                Debug.Log("AI思考中");
                breakTime = 0;
            }
        }

        yield return null;
        
        PlayerState most = backup[0];
        foreach (PlayerState ps in backup) {
            ps.ScoreUpdate();
            if (ps.score > most.score || (ps.score == most.score && ps.actionCount < most.actionCount)) most = ps;
        }
        //Debug.Log(most.score + "点:残り体力" + most.enemy.life + ":" + most.skill);
        Debug.Log("使用可能な起動効果：" + most.ActiveEffectCards.Count);

        yield return null;
        time += Time.deltaTime;

        List<AI_Action> actions = new List<AI_Action>();
        while (most != null && most.enterdAction.HasValue) {
            //Debug.Log(most.enterdAction.Value.actType);
            actions.Add(most.enterdAction.Value);
            most = most.parent;
        }
        actions.Reverse();
        int num = 1;
        foreach (AI_Action act in actions)
            Debug.Log(num++ + "番目の行動：" + act.actType + ":" + act.card + ":" + act.target);

        //設定した時間まで待機
        while (time < waitTime) {
            yield return null;
            time += Time.deltaTime;
        }

        //履歴を辿り、その状態に移行させる
        foreach (AI_Action act in actions) {
            if (Constants.BATTLE.GetTurnPhase() != GamePhase.main) yield break;
            switch (act.actType) {
                case ActionType.cardPlay:
                    player.CardPlay(act.card, act.target);// act.card.Play(act.target);
                    break;
                case ActionType.cardDisp:
                    act.card.Disp();
                    break;
                case ActionType.activeEffect:
                    act.card.ActiveEffectPlay();
                    break;
                case ActionType.Attack:
                    player.Attack(act.target);
                    break;
            }
            time = 0;
            //設定した時間まで待機
            while (time < waitTime) {
                while (Constants.BATTLE.EventStack.Count > 0) {
                    yield return null;
                    time = 0;
                }
                yield return null;
                time += Time.deltaTime;
            }
        }
        //Debug.Log("行動終了");
        yield return StartCoroutine(TrapSet());
        Constants.BATTLE.TurnEnd();
        isThinking = null;
    }

    private IEnumerator TrapSet() {
        List<Card> hand = new List<Card>(player.GetHandZone().GetCards());
        foreach (Card c in hand) {
            if (player.GetSupportZone().GetCardsCount() == 5) break;
            if (c.State.cardType != ObjectType.Trap) continue;
            yield return new WaitForSeconds(0.5f);
            player.CardPlay(c);
        }
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator Draft() {
        Init();
        while (Constants.BATTLE.Field.GetDraftZone().GetCards().Count == 0) yield return null;
        float time = 0;//経過時間を計算
        float waitTime = 2.0f;//待機時間を設定

        List<Card> cards = Constants.BATTLE.Field.GetDraftZone().GetCards();
        PlayerState state;
        PlayerState most = null;
        Card mostCard = cards[0];
        foreach (Card c in Constants.BATTLE.Field.GetDraftZone().GetCards()) {
            if (!player.isCardPlay(c)) continue;
            state = new PlayerState(player,ai);
            state.hand.Add(c);
            GetAllAction(state);

            ////
            List<PlayerState> backup = new List<PlayerState>();
            backup.Add(state);
            int breakTime = 0;

            for (int i = 0; i < backup.Count; i++) {

                foreach (AI_Action act in backup[i].allAction) {
                    backup.Add(ActionEnter(backup[i], act));
                    breakTime++;
                }

                if (++breakTime > 100) {
                    yield return null;
                    time += Time.deltaTime;
                    Debug.Log("AI思考中");
                    breakTime = 0;
                }
            }

            yield return null;
            time += Time.deltaTime;
            if (most == null) most = backup[0];

            foreach (PlayerState ps in backup) {
                ps.ScoreUpdate();
                if (ps.score > most.score || (ps.score == most.score && ps.actionCount < most.actionCount)) {
                    most = ps;
                    mostCard = c;
                } 
            }
        }
        //設定した時間まで待機
        while (time < waitTime) {
            yield return null;
            time += Time.deltaTime;
        }

        Constants.BATTLE.DraftDraw(mostCard);
    }

    public IEnumerator Trap(TriggerType trigger,List<Card> cards,EventBlock block) {
        yield return null;
        Card openTrap = null;
        switch (trigger) {
            case TriggerType.TakeAttack:
                AttackBlock atkBlock = block as AttackBlock;
                if (atkBlock == null) yield break;
                int damage = block.GetCaster().AttackPower;
                int needValue = 0;//必要な最低値
                if (atkBlock.GetTarget().GetComponent<Player>()) {
                    damage -= player.Defence;
                    if (damage >= player.Life) needValue = damage - player.Life;
                    else if (damage > 0) needValue = 1;
                }
                else {
                    damage -= atkBlock.GetTarget().GetComponent<Card>().State.defence;
                    needValue = damage + 1;
                }
                foreach (Card c in cards) {
                    foreach (CardEffect ce in c.State.Effect) {
                        foreach (EffectState es in ce.effectState) {
                            if (es.effectType == EffectType.AttackAdd && es.value >= -needValue) {
                                if (openTrap == null || openTrap.State.Cost > c.State.Cost)
                                    openTrap = c;
                            }
                        }
                    }
                }
                break;
            case TriggerType.TakeSkill:
                openTrap = cards[0];
                break;
            case TriggerType.TakeDispCard:
                CardDispBlock disp = block as CardDispBlock;
                if (disp.GetCards()[0].State.cardAligment == player.cardAligment)
                    openTrap = cards[0];
                break;
            default:
                openTrap = cards[0];
                break;
        }

        if (openTrap) {
            yield return new WaitForSeconds(0.5f);
            Constants.BATTLE.CardOpen(openTrap);
        }
    }

    /// <summary>
    /// 行動結果を予測する
    /// </summary>
    /// <param name="state">実行前の状況</param>
    /// <param name="action">実行する行動</param>
    /// <returns>実行後の状況</returns>
    PlayerState ActionEnter(PlayerState state,AI_Action action) {
        state = new PlayerState(state);
        state.enterdAction = action;
        state.actionCount++;

        //なんか色々
        switch (action.actType) {
            case ActionType.Attack:
                if (action.target) {
                    state.atkedCount++;
                    Damage(state, action.target, state.atk);
                }
                break;
            case ActionType.cardPlay:
                state.hand.Remove(action.card);
                SupportDestTrigger(state, action);
                state.cost -= action.card.State.Cost;
                switch (action.card.State.cardType) {
                    case ObjectType.Unit:
                        state.AddUnit(action.card);// unit.Add(action.card);
                        state.atk += action.card.State.atk;
                        state.skill += action.card.State.skill;
                        break;
                    case ObjectType.Support:
                        state.AddSupport(action.card);//support.Add(action.card);
                        state.atk += action.card.State.atk;
                        state.skill += action.card.State.skill;
                        break;
                    case ObjectType.Trap:
                        state.AddSupport(action.card);//support.Add(action.card);
                        state.cost += action.card.State.Cost;
                        break;
                    case ObjectType.Skill:
                        state.Dest.Add(action.card);
                        break;
                    default: break;
                }
                foreach (CardEffect ce in action.card.State.Effect) {
                    if (ce.GetEffectPlayType() == EffectPlayType.triggerEffect) {
                        state.score_mod += GetTriggerEffectPoint(state, ce as TriggerEffect);
                        state.score_mod -= action.card.State.atk * ai.atk;
                        continue;
                    }
                    else if (ce.GetEffectPlayType() == EffectPlayType.activeEffect && action.card.State.cardType == ObjectType.Support) {
                        state.score_mod += 3;
                    }
                    if (ce.GetEffectPlayType() != EffectPlayType.playEffect) continue;
                    foreach (EffectState es in ce.effectState) {
                        switch (es.effectType) {
                            case EffectType.AttackAdd:
                                if (es.Target.targetType == TargetType.friend) {
                                    state.atk += es.value;
                                    //ターン終了で攻撃力が戻るので、継続的なスコアには加算しない
                                    state.score_mod -= es.value * ai.atk;
                                }
                                else {
                                    state.enemy.atk += es.value;
                                } 
                                break;
                            case EffectType.AttackTime:
                                state.atkCount = es.value;
                                break;
                            case EffectType.CardDest:
                                if (action.target)
                                    Dest(state, action.target);
                                else if (es.Target.isAll)
                                    Dest(state, es.GetTargets(state.p));
                                break;
                            case EffectType.CardDraft:
                                state.hand.Add(null);
                                break;
                            case EffectType.CardDraw:
                                for (int i = 0; i < es.value; i++) state.hand.Add(null);
                                break;
                            case EffectType.CardUnDraft:
                                for (int i = 0; i < es.value - 1; i++) state.hand.Add(null);
                                break;
                            case EffectType.CostAdd:
                                state.cost += es.value;
                                break;
                            case EffectType.Damage:
                                int damage = es.value + (action.card.State.cardType == ObjectType.Skill ? state.skill : 0);
                                //Debug.Log(action.card.State.cardName + ":" + damage);
                                if (action.target)
                                    Damage(state, action.target, damage);
                                else if (es.Target.isAll)
                                    Damage(state, es.Target.GetTargets(state.p), damage);
                                else Debug.Log("対象が不明です");
                                break;
                            case EffectType.Counter:
                                state.score_mod += 2;
                                break;
                        }
                    }
                }
                break;
            case ActionType.cardDisp:
                switch (action.card.State.cardType) {
                    case ObjectType.Unit:
                        state.RemoveUnit(action.card);//.unit.Remove(action.card);
                        state.atk -= action.card.State.atk;
                        state.skill -= action.card.State.skill;
                        break;
                    case ObjectType.Support:
                        state.RemoveSupport(action.card);//support.Remove(action.card);
                        state.atk -= action.card.State.atk;
                        state.skill -= action.card.State.skill;
                        break;
                    case ObjectType.Trap:
                        state.RemoveSupport(action.card);//support.Remove(action.card);
                        break;
                }
                state.score_mod -= 2;
                foreach (CardEffect ce in action.card.State.Effect)
                    if (ce.GetEffectPlayType() == EffectPlayType.triggerEffect)
                        state.score_mod -= GetTriggerEffectPoint(state, ce as TriggerEffect);
                break;
            case ActionType.activeEffect:
                foreach (CardEffect ce in action.card.State.Effect) {
                    ActiveEffect ae = ce as ActiveEffect;
                    if (ae != null) {
                        state.cost -= ae.cost;
                        state.PlayActiveEffect(action.card);
                        foreach (EffectState es in ce.effectState) {
                            switch (es.effectType) {
                                case EffectType.AttackAdd:
                                    state.atk += es.value;
                                    break;
                                case EffectType.GetDisZoneCard:
                                    if (action.target) {
                                        Card c = action.target.GetComponent<Card>();
                                        state.hand.Add(c);
                                        state.Dest.Remove(c);
                                    }
                                    break;
                            }
                        }
                    }
                }
                break;
        }
        GetAllAction(state);
        return state;
    }

    /// <summary>
    /// 支援カードの破棄条件を満たしたら破棄する
    /// </summary>
    /// <param name="state"></param>
    /// <param name="action"></param>
    void SupportDestTrigger(PlayerState state,AI_Action action) {
        if (action.card != null && action.card.State.Cost >= 4) {
            foreach (Card c in new List<Card>(state.support))
                if (c.State.cardType == ObjectType.Support && c.State.trigger == TriggerType.PlayCard_4CostOrMore) {
                    state.RemoveSupport(c);//.support.Remove(c);
                    state.Dest.Add(c);
                }
        }
    }

    int GetTriggerEffectPoint(PlayerState state,TriggerEffect effect) {
        if (effect == null)
            return 0;

        int p = 0;
        foreach (EffectState es in effect.effectState) {
            switch (es.effectType) {
                case EffectType.UnitCostAdd:
                    foreach (Card c in state.unit)
                        p += c.State.Cost;
                    break;
                case EffectType.AttackAdd:
                    p += es.value * ai.atk;
                    break;
            }
        }
        return p;
    }

    void Dest(PlayerState state,List<GameObject> targets) {
        foreach (GameObject t in targets) Dest(state,t);
    }

    void Dest(PlayerState state, GameObject target) {
        Card c = target.GetComponent<Card>();
        if (!state.RemoveUnit(c)) state.RemoveSupport(c);//support.Remove(c);
        state.Dest.Add(c);
    }

    void Damage(PlayerState state, List<GameObject> targets,int value) {
        foreach (GameObject t in targets)
            Damage(state, t, value);
    }

    void Damage(PlayerState state, GameObject target,int value) {
        Card c = target.GetComponent<Card>();
        if (c && c.State.defence <= value) {
            state.enemy.unit.Remove(c);
            state.Dest.Add(c);
            state.enemy.atk -= c.State.atk;
            state.enemy.skill -= c.State.skill;
            state.score_mod += 1;
            Debug.Log("攻撃対象を破壊可能");
            return;
        }
        Player p = target.GetComponent<Player>();
        if (p && value > p.Defence) {
            state.enemy.life -= value - p.Defence;
            state.score_mod += 1;
        }
    }
}

public class aiConst{
    //自分
    public int hand, unit, support, trap;//カード枚数
    public int atk, skill, life, cost;//ステータス
    public int cost_border = 15;//コストが溜まりすぎないよう評価最大値を設定
    //敵
    public int e_hand, e_unit, e_support, e_trap;//カード枚数
    public int e_atk, e_skill, e_life;//ステータス
}

public class Swordman_Const : aiConst {
    public Swordman_Const(){
        hand = 1;unit = 3;support = 1;trap = 2;
        atk = 3;skill = 0;life = 2;cost = 1;
        e_hand = 1;e_unit = 3;e_support = 2;e_trap = 0;
        e_atk = 0;e_skill = 3;e_life = 2;
    }
}

public class Magician_Const : aiConst {
    public Magician_Const() {
        hand = 1; unit = 1; support = 2; trap = 2;
        atk = 3; skill = 3; life = 2;cost = 1;
        e_hand = 1; e_unit = 4; e_support = 2; e_trap = 0;
        e_atk = 3; e_skill = 0; e_life = 1;
    }
}