using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IAttackTarget
{
    //使用できるカードの属性
    [SerializeField]
    private CardAligment _cardAligment;
    public CardAligment cardAligment { get { return _cardAligment; }
        set {
            _cardAligment = value;
            costUI.TextUpdate(this,true);
            UnitManager.AligmentUpdate(value);
            SupportManager.AligmentUpdate(value);
        } }

    /// <summary>
    /// 使用条件に適合する属性ならtrue、それ以外ならfalseを返す
    /// </summary>
    /// <param name="aligment"></param>
    /// <returns></returns>
    public bool AligmentCheck(CardAligment aligment) {
        return !(_cardAligment == (aligment == CardAligment.SowrdMan ? CardAligment.Magician : CardAligment.SowrdMan));
    }

    public bool isAI;

    public int Life { get { return LifeManager.GetCardsCount(); } }


    [SerializeField]
    private CostUI costUI;
    
	//private const int defaultCost = 5;
    [SerializeField]
	private int _cost = Constants.START_COST;

    public int cost
    {
        get { return _cost; }
        set
        {
            _cost = value;
            CostUpdate();
        }
    }

    //コスト更新時の処理
    private bool isInit = true;//初回のみtrueフラグを渡す
    private void CostUpdate() {
        costUI.TextUpdate(this,isInit);
        isInit = false;
        Constants.BATTLE.AddStack(new CostChangeBlock(cost,this));
    }

    public BuffState Buff = new BuffState();

    [SerializeField]
    public DragEffet playerImage;

    public int Defence = 2;

    //攻撃力
    public int AttackPower{
        get { return UnitManager.GetAtack() + SupportManager.GetAtack() + Buff.GetAtk(); } }

    //現在の攻撃回数
    private int _atatckCount;
    public int attackCount { get { return _atatckCount; } set { _atatckCount = value; AttackCountUpdate(); } }
    //最大攻撃回数
    private int maxAttackCount { get { return Buff.GetMaxAtkCount(); } }
    
    //スキルダメージ
    public int SkillDamage {
        get { return UnitManager.GetSkill() + SupportManager.GetSkill() + Buff.GetSkill(); } }

    public string GetStatesText() {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append(UnitManager.GetAtkText());
        sb.Append(SupportManager.GetAtkText());
        sb.Append(UnitManager.GetSkillText());
        sb.Append(SupportManager.GetSkillText());
        sb.Append(Buff.GetBuffText());

        sb.Append("\n");
        sb.Append("攻撃力：" + AttackPower);
        sb.Append("\n");
        sb.Append("技ダメージ：" + SkillDamage);
        sb.Append("\n");
        sb.Append("防御力：" + Defence);

        return sb.ToString();
    }

    //手札ゾーン
    [SerializeField]
    private HandManager HandManager;
    public HandManager GetHandZone() { return HandManager; }

    //ユニットゾーン
    [SerializeField]
    private UnitManager UnitManager;
    public UnitManager GetUnitZone() { return UnitManager; }

    //マリガンゾーン
    [SerializeField]
    private MulliganManaegr MulliganManager;
    public MulliganManaegr GetMulliganManager() { return MulliganManager; }

    //ライフゾーン
    [SerializeField]
    private LifeManager LifeManager;
    public LifeManager GetLifeZone() { return LifeManager; }

    //支援ゾーン
    [SerializeField]
    private SupportManager SupportManager;
    public SupportManager GetSupportZone() { return SupportManager; }

    void Awake() {
        if (GetComponent<AIScript>())
            isAI = true;
    }
    
    //初期化
    public void Init() {
		cost = Constants.START_COST;
        HandManager.Init();
        UnitManager.Init();
        SupportManager.Init();
        LifeManager.Init();
    }

    //手札を追加する
    public void AddHand(List<Card> card) { HandManager.AddCard(card); }
    public void AddHand(Card card) { HandManager.AddCard(card); }
    public bool RemoveHand(Card card) { return HandManager.RemoveCard(card); }

    /// <summary>
    /// ターン開始処理
    /// </summary>
    public void TurnStart() {
        attackCount = 0;
        playerImage.SetIsActive(false);

        cost += cardAligment == CardAligment.SowrdMan ? Constants.S_COST : Constants.M_COST;

        if (GetComponent<AIScript>()) GetComponent<AIScript>().TurnStart();
        //Constants.BATTLE.CallTrigger(TriggerType.TurnStart,this);
    }

    /// <summary>
    /// ターン終了処理
    /// </summary>
    public void TurnEnd() {
        Buff.Tick();
        HandManager.Tick();
        UnitManager.Tick();
        SupportManager.Tick();
        //LifeManager.Refresh();//通常、ここに更新の必要は無いので動かないように
        //Constants.BATTLE.CallTrigger(TriggerType.TurnEnd, this);
    }

    /// <summary>
    /// 補助効果の更新　（一度の処理の間のみ働く効果等を消去）
    /// </summary>
    public void Refresh() {
        Buff.Refresh();
    }

    //カードの入手
    public void CardDraw(int count = 1) {
        AddHand(Constants.BATTLE.CardDraw(count));
    }

    //カードの使用
    public bool CardPlay(Card card,GameObject target = null) {
        if (!card.isPlay(cost,_cardAligment)) return false;
        //Card.Play()内で効果の処理をしているので、コスト変動効果の際はCard.Play()でコストが変更されます。
        int useCost = card.Play(target);
        cost -= useCost;
        return true;
    }

    public bool isCardPlay(Card card) {
        return card.isPlay(cost, _cardAligment);
    }

    public bool TrapOpen(Trap trap) {
        if (!trap.isOpen(cost)) return false;

        int useCost = trap.Open();
        cost -= useCost;
        return true;
    }

    /// <summary>
    /// 発動条件に適合する罠カードを全て返す
    /// </summary>
    /// <param name="trigger">指定する発動条件</param>
    /// <returns></returns>
    public List<Card> GetCanPlayTrapCards(TriggerType trigger) {
        return SupportManager.GetCanPlayTrapCards(trigger);
    }

    public IEnumerator SupportDestTriggerCheck(TriggerType type) {
        yield return StartCoroutine(SupportManager.SupportDestTriggerCheck(type));
    }

    

    /// <summary>
    /// 現在の攻撃回数が最大攻撃回数を上回っているか確認
    /// </summary>
    /// <returns></returns>
    public bool isCanAttack()
    {
        return (attackCount < maxAttackCount);
    }

    /// <summary>
    /// 攻撃回数が変更されたときに呼び出される更新処理
    /// </summary>
    /// <returns></returns>
    public void AttackCountUpdate() {
        playerImage.SetIsActive(isCanAttack() && Constants.BATTLE.GetActivePlayer() == this);
    }

    //攻撃を試みる
    public int Attack(GameObject target) {
        if (target == null) return 0;

        Constants.BATTLE.AddStack(new AttackBlock(target, this));
        attackCount++;
        AttackCountUpdate();

        return 0;
    }

    public int GetTargetPriority() { return Constants.TARGET_NOMAL; }

    public IEnumerator Damage(int damage) {
        //Card lastCard = null;
        int d = damage - Defence >= 0 ? damage - Defence : 0;
        DamageEffect.DamageInit(d, playerImage.gameObject);
        yield return StartCoroutine(LifeManager.GetDamage(d,this));
        //Constants.BATTLE.CallTrigger(TriggerType.TakeDamage,this);
    }

    public IEnumerator Heal(int value) {
        if (value < 0) value = 0;
        for (int i = 0; i < value; i++)
            LifeManager.AddCard(Constants.BATTLE.InitCard(CardList.NormalLifeCard()),true);
        //return value;
        DamageEffect.DamageInit(-value, playerImage.gameObject);
        yield return null;
    }

    //引数のカードが自分のものか確認
    public CardZone FindCard(Card card) {
        if (HandManager.FindCard(card)) return HandManager;
        if (UnitManager.FindCard(card)) return UnitManager;
        if (SupportManager.FindCard(card)) return SupportManager;
        if (LifeManager.FindCard(card)) return LifeManager;
        if (MulliganManager.FindCard(card)) return MulliganManager;
        return null;
    }

    //引数のカードゾーンが自分のものか確認
    public bool isHaveZone(CardZone zone) {
        if (zone == HandManager) return true;
        if (zone == UnitManager) return true;
        if (zone == SupportManager) return true;
        if (zone == LifeManager) return true;
        if (zone == MulliganManager) return true;
        return false;
    }
    
    //プレイヤーに適用されている補助効果
    [System.Serializable]
    public class BuffState{

        [System.Serializable]
        public class State
        {
            public int atk;
            public int skill;
            public int maxAtkCount = 1;
            public int deffence;
        }

        private State state = new State();

        public class BuffEffect{
            public BuffEffect(EffectState _effect) { EffectState = _effect; turn = _effect.turn; }
            public EffectState EffectState;
            public int? turn;
        }

        public List<BuffEffect> BuffEffects = new List<BuffEffect>();

        //効果を設置
        public void SetState(EffectState _effect)
        {
            BuffEffects.Add(new BuffEffect(_effect));
            StateUpdate();
        }

        /// <summary>
        /// 効果の除去
        /// </summary>
        /// <param name="num">効果の番号</param>
        private void RemoveState(int num){
            BuffEffects.RemoveAt(num);
            StateUpdate();
        }

        /// <summary>
        /// 効果の除去
        /// </summary>
        /// <param name="_effect">効果の情報</param>
        public void RemoveState(EffectState _effect) {
            for (int i = 0; i < BuffEffects.Count; i++)
                if (BuffEffects[i].EffectState == _effect) RemoveState(i--);
        }

        /// <summary>
        /// ターン経過
        /// </summary>
        public void Tick()
        {
            for (int i = 0; i < BuffEffects.Count; i++)
                if (--BuffEffects[i].turn <= 0) RemoveState(i--);
        }

        /// <summary>
        /// 継続しない補助効果を削除する
        /// </summary>
        public void Refresh() {
            for (int i = 0; i < BuffEffects.Count; i++)
                if (BuffEffects[i].turn <= 0) RemoveState(i--);
        }

        /// <summary>
        /// 能力値を更新する
        /// </summary>
        public void StateUpdate(){
            state.atk = 0;
            state.skill = 0;
            state.maxAtkCount = Constants.ATTACK_COUNT;
            state.deffence = 0;

            foreach (BuffEffect effect in BuffEffects) {
                switch (effect.EffectState.effectType) {
                    case EffectType.AttackAdd://攻撃力操作
                        state.atk += effect.EffectState.value;
                        break;
                    case EffectType.SkillDamageAdd://技ダメージ操作
                        state.skill += effect.EffectState.value;
                        break;
                    case EffectType.AttackTime://攻撃回数操作
                        state.maxAtkCount = effect.EffectState.value;
                        break;
                    case EffectType.DamageAdd://被ダメージ操作
                        state.deffence += effect.EffectState.value;
                        break;
                    default://その他
                        break;
                }
            }
        }

        public int GetAtk() { return state.atk; }
        public int GetSkill() { return state.skill; }
        public int GetMaxAtkCount() { return state.maxAtkCount; }
        public int GetDeffence() { return state.deffence; }
        public string GetBuffText() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (BuffEffect _effect in BuffEffects) {
                switch (_effect.EffectState.effectType) {
                    case EffectType.AttackAdd://攻撃力操作
                        sb.Append("攻撃力の上昇");
                        break;
                    case EffectType.SkillDamageAdd://技ダメージ操作
                        sb.Append("技ダメージの上昇");
                        break;
                    case EffectType.AttackTime://攻撃回数操作
                        sb.Append("攻撃回数の変化");
                        break;
                    case EffectType.DamageAdd://被ダメージ操作
                        sb.Append("被ダメージの減少");
                        break;
                    default://その他
                        break;
                }
                sb.Append(" 効果量：" + _effect.EffectState.value);
                sb.Append(" 残りターン数：" + _effect.turn);
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}