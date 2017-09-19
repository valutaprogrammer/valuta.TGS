using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardPoint{
    public Card card;
    public int point;
    public CardZoneType zone;
}

public class AIScript : MonoBehaviour {

    private Player player;
    public int cost_value = AIConstants.COST_POINT;
    public Scond_AIScript second_AI;//テスト用AI

    [System.Serializable]
    public class PlayerState
    {
        public List<CardPoint> hand = new List<CardPoint>(), Unit = new List<CardPoint>(), Support = new List<CardPoint>();//それぞれの場のカード
        public int atk, skill;
        public int atkMod, skillMod;//場のカード以外から得ている攻撃力、技ダメージの値
        public int cost;
        public Player player;

        public PlayerState(Player player)
        {
            this.player = player;
            foreach (Card c in player.GetHandZone().GetCards())
                hand.Add(new CardPoint() { card = c, point = 0, zone = CardZoneType.Hand });
            foreach (Card c in player.GetUnitZone().GetCards())
                Unit.Add(new CardPoint() { card = c, point = 0, zone = CardZoneType.Unit });
            foreach (Card c in player.GetSupportZone().GetCards())
                Support.Add(new CardPoint() { card = c, point = 0, zone = CardZoneType.Support });
            atk = player.AttackPower;
            skill = player.SkillDamage;
            atkMod = player.AttackPower - (player.GetUnitZone().GetAtack() + player.GetSupportZone().GetAtack());
            skillMod = player.SkillDamage - (player.GetUnitZone().GetSkill() + player.GetSupportZone().GetSkill());
            cost = player.cost;
        }

        public List<CardPoint> GetAllCard()
        {
            List<CardPoint> allcard = new List<CardPoint>();
            foreach (CardPoint cp in Unit)
                allcard.Add(cp);
            foreach (CardPoint cp in Support)
                allcard.Add(cp);
            foreach (CardPoint cp in hand)
                allcard.Add(cp);
            return allcard;
        }
    }

    void Awake() {
        player = GetComponent<Player>();
        second_AI = gameObject.AddComponent<Scond_AIScript>();//テスト用AI
    }

    /// <summary>
    /// マリガンの処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator Mulligun() {
        List<Card> cards = new List<Card>(player.GetHandZone().GetCards());
        foreach (Card c in cards)
            if (c.State.cardAligment == (player.cardAligment == CardAligment.SowrdMan ? CardAligment.Magician : CardAligment.SowrdMan))
                player.GetMulliganManager().AddCard(c);

        yield return new WaitForSeconds(2.0f);
        player.GetMulliganManager().OnEnterButton();
        while (Constants.BATTLE.GetTurnPhase() == GamePhase.mulligan)
            yield return null;
    }

    /// <summary>
    /// 指定されたカードの中からカードを一枚選択する処理
    /// </summary>
    /// <param name="cards">選択候補のカード</param>
    /// <param name="isMaxSelect">評価点の高いものを選択するか、低いものを選択するか</param>
    /// <returns></returns>
    public IEnumerator CardSelect(List<Card> cards,bool isMaxSelect = true) {
        yield return new WaitForSeconds(1.5f);
        foreach (Card c in cards){
            Constants.CARD_SELECT_EFFECT.CardSelect(c);
            break;
        }
    }

    /// <summary>
    /// ターン開始
    /// </summary>
    public void TurnStart() {
        //StartCoroutine(Turn());
    }

    /*
    /// <summary>
    /// 行動処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator Turn() {

        //二人のプレイヤーの情報を取得
        PlayerStates = new PlayerState[2] {
            new PlayerState(player),
            new PlayerState(Constants.BATTLE.GetOtherPlayer(player))
        };

        while (Constants.BATTLE.GetActivePlayer() == player) {
            switch (Constants.BATTLE.GetTurnPhase()) {
                case GamePhase.draft:
                    yield return StartCoroutine(DraftPhase());
                    break;
                case GamePhase.main:
                    //yield return StartCoroutine(MainPhase());
                    yield return new WaitForSeconds(1.0f);
                    yield return StartCoroutine(second_AI.Think());//テスト用AI
                    Constants.BATTLE.TurnEnd();
                    while (Constants.BATTLE.GetTurnPhase() == GamePhase.main)
                        yield return null;
                    break;
                case GamePhase.handDest:
                    yield return StartCoroutine(HandDestPhase());
                    break;
                default:
                    yield return null;
                    break;
            }
            yield return null;
        }
        Debug.Log("TurnEnd:" + gameObject);
    }
    */
    /// <summary>
    /// ドラフトの処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator DraftPhase() {
        while (Constants.BATTLE.Field.GetDraftZone().GetCards().Count == 0) yield return null;
        yield return new WaitForSeconds(1.5f);
        CardPoint selectCard = null;

        List<CardPoint> cards = new List<CardPoint>();
        foreach (Card c in Constants.BATTLE.Field.GetDraftZone().GetCards())
            cards.Add(new CardPoint() { card = c, point = GetPoint(PlayerStates[0].player, c, CardZoneType.Hand), zone = CardZoneType.Draft });
        
        foreach (CardPoint cp in cards)
            if (selectCard == null ||(player.AligmentCheck(cp.card.State.cardAligment) && cp.point > selectCard.point)) selectCard = cp;
        //if (selectCard == null) selectCard = draft[0];
        Constants.BATTLE.DraftDraw(selectCard.card);

        while (Constants.BATTLE.GetTurnPhase() == GamePhase.draft) {
            yield return null;
        }
    }

    /// <summary>
    /// メインフェイズの処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator MainPhase() {
        yield return new WaitForSeconds(2.0f);
        bool continueFlag;
        do {
            continueFlag = false;

            //各行動の評価値を取得する
            yield return StartCoroutine(PointUpdate());
            PlayListSort_CardPoint(PlayerStates[0].hand);

            List<Action> actions = GetAllAction();

            //
            foreach (Action a in actions) {
                if (a.point > 0) {
                    //Debug.Log(a.point);
                    a.ActionEnter();
                    Debug.Log(a.GetActionName());
                    yield return new WaitForSeconds(0.5f);
                    continueFlag = true;
                    break;
                }
            }
            //処理終了まで待機
            while (Constants.BATTLE.EventStack.Count > 0)
                yield return null;
            yield return new WaitForSeconds(1.0f);
        } while (continueFlag);

        Constants.BATTLE.TurnEnd();
        while (Constants.BATTLE.GetTurnPhase() == GamePhase.main) {
            yield return null;
        }
    }

    /// <summary>
    /// 手札破棄の処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator HandDestPhase() {
        yield return new WaitForSeconds(1.0f);
        //Debug.Log(player.GetHandZone().GetCards().Count);
        while (Constants.BATTLE.GetTurnPhase() == GamePhase.handDest && player.GetHandZone().isHandOver()) {
            foreach (Card c in player.GetHandZone().GetCards()){
                //Debug.Log(c.State.cardName + ":" + !player.AligmentCheck(c.State.cardAligment));
                if (!player.AligmentCheck(c.State.cardAligment))
                    if (Constants.BATTLE.HandDest(c)) yield break;
                //break;
            }
            foreach (Card c in player.GetHandZone().GetCards()) {
                if (Constants.BATTLE.HandDest(c)) yield break;
                break;
            }
            //フェイズ移行する前にもう一枚捨てないよう、待機する
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// 可能な行動を全て取得する処理
    /// </summary>
    /// <returns></returns>
    private List<Action> GetAllAction() {
        List<Action> actions = new List<Action>();

        //手札使用行動を取得
        foreach (CardPoint cp in PlayerStates[0].hand) {
            if (player.isCardPlay(cp.card)) {
                Action_CardPlay action = new Action_CardPlay(player, cp.card);
                action.point = cp.point;
                actions.Add(action);
            }
        }

        //攻撃行動を取得
        if (player.isCanAttack() && GetAttackPoint() > 0) {
            Action_Attack attack = new Action_Attack(player, GetAttackTarget());
            attack.point = GetAttackPoint();
            actions.Add(attack);
        }


        //攻撃後に行うべき行動の順序を遅らせる
        List<Action> rate = new List<Action>();
        foreach (Action action in actions) {
            if (action.GetType() == typeof(Action_CardPlay) && (action as Action_CardPlay).card.State.atk < 0)
                rate.Add(action);
        }
        //最後尾に送る
        foreach (Action action in rate) {
            actions.Remove(action);
            actions.Add(action);
        }

        return actions;
    }

    /// <summary>
    /// 攻撃対象を取得する処理
    /// </summary>
    /// <returns></returns>
    public GameObject GetAttackTarget() {
        List<CardPoint> points = new List<CardPoint>();
        CardPoint point = null;
        GameObject target = null;
        foreach (CardPoint c in PlayerStates[1].Unit)
        {
            if (c.card.State.defence <= player.AttackPower) {
                /*if (c.card.GetComponent<IAttackTarget>().GetTargetPriority() > Constants.TARGET_NOMAL)
                    points.Add(c);*/
                if (Constants.BATTLE.IsAttackTarget(c.card.gameObject))
                    points.Add(c);
            }
        }

        for (int i = 0; i < points.Count; i++) {
            if (point == null)
                point = points[i];
            else if (point.point < points[i].point)
                point = points[i];
        }
        if (point != null)
            target = point.card.gameObject;
        //if (Constants.BATTLE.IsAttackTarget(Constants.BATTLE.GetOtherPlayer(player).gameObject))
        if (player.AttackPower - Constants.BATTLE.GetOtherPlayer(player).Defence > 0 &&
            (target == null || (player.AttackPower - Constants.BATTLE.GetOtherPlayer(player).Defence) * AIConstants.LIFE_POINT > point.point))
            target = Constants.BATTLE.GetOtherPlayer(player).gameObject;
        
        return target;
    }

    class AttackTargetPoint {
        public int point;
        public GameObject obj;

        public AttackTargetPoint(GameObject obj, int point) {
            this.point = point;
            this.obj = obj;
        }
    }

    /// <summary>
    /// 攻撃時の最大評価点を取得する処理
    /// </summary>
    /// <param name="attackCount">攻撃回数</param>
    /// <returns></returns>
    public int GetAttackPoint(int attackCount = 1, int attackPower = -100)
    {
        if (attackCount == 0) return 0;
        if (attackPower == -100) attackPower = player.AttackPower;
        List<AttackTargetPoint> points = new List<AttackTargetPoint>();

        int point = 0;

        foreach (CardPoint c in PlayerStates[1].Unit) {
            //攻撃対象が限定されている場合、その対象に対して必ず攻撃する
            if (c.card.GetComponent<IAttackTarget>().GetTargetPriority() > Constants.TARGET_NOMAL) {
                if (attackPower >= c.card.State.defence) {
                    attackCount--;
                    point += c.point;
                }
                else attackCount = 0;
            }
            else if (attackPower >= c.card.State.defence) {
                points.Add(new AttackTargetPoint(c.card.gameObject, c.point));
            }
        }

        int playerDamage = (attackPower - Constants.BATTLE.GetOtherPlayer(player).Defence);// * AIConstants.LIFE_POINT;
        if (Constants.BATTLE.IsAttackTarget(Constants.BATTLE.GetOtherPlayer(player).gameObject))
        {
            if (playerDamage > 0 /*&& (player.AttackPower - Constants.BATTLE.GetOtherPlayer(player).Defence) * AIConstants.LIFE_POINT > point*/)
            {
                for (int i = 0; i < attackCount; i++) {
                    //points.Add(p);
                    //target.Add(player.gameObject);
                    points.Add(new AttackTargetPoint(player.gameObject, playerDamage * AIConstants.LIFE_POINT));
                }
            }
        }

        points.Sort((a, b) => a.point - b.point);
        points.Reverse();

        int pAttack = 0;
        for (int i = 0; i < attackCount && i < points.Count; i++) {
            point += points[i].point;
            if (points[i].obj == player.gameObject)
                pAttack++;
        }

        //勝利できそうなら評価点を最大にする
        if (pAttack * playerDamage >= Constants.BATTLE.GetOtherPlayer(player).GetLifeZone().GetCardsCount())
            point = 10000;

        return point;
    }

    /// <summary>
    /// 行動の評価点を更新する処理
    /// </summary>
    /// <param name="actions">行動のリスト</param>
    private void ActionPointUpdate(List<Action> actions) {
        bool isContinue;
        int x = 0;
        do {
            isContinue = false;
            foreach (Action action in actions)
                if (action.PointUpdate(PlayerStates)) isContinue = true;
        } while (isContinue && x++ < 100);
    }

    public PlayerState[] PlayerStates;
    /// <summary>
    /// 全ての行動を取得し、評価点を計算する処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator PointUpdate() {
        //二人のプレイヤーの情報を取得
        PlayerStates = new PlayerState[2] {
            new PlayerState(player),
            new PlayerState(Constants.BATTLE.GetOtherPlayer(player))
        };

        //各カードの評価値取得

        //場のカードの評価値取得
        for (int i = 0; i < PlayerStates.Length; i++) {
            foreach (CardPoint cp in PlayerStates[i].Unit)
                cp.point = GetPoint(PlayerStates[i].player, cp.card, cp.zone);
            foreach (CardPoint cp in PlayerStates[i].Support)
                cp.point = GetPoint(PlayerStates[i].player, cp.card, cp.zone);
            //yield return null;
        }
        //手札のカードの評価値更新
        bool isContinue;
        int x = 0;
        int newPoint;
        do {
            isContinue = false;
            for (int i = 0; i < PlayerStates.Length; i++) {
                foreach (CardPoint cp in PlayerStates[i].hand) {
                    newPoint = GetPoint(PlayerStates[i].player, cp.card, cp.zone);
                    if (cp.point != newPoint) {
                        isContinue = true;
                        cp.point = newPoint;
                    }
                }
            }
            yield return null;
        } while (isContinue && x++ < 100);
    }

    /// <summary>
    /// カードの評価点を計算する処理
    /// </summary>
    /// <param name="player">対象カードを保持するプレイヤー</param>
    /// <param name="card">対象のカード</param>
    /// <param name="zone">対象のカードが存在する場</param>
    /// <returns></returns>
    public int GetPoint(Player player, Card card, CardZoneType zone) {
        GameObject damy = null;
        return GetPoint(player, card, zone,ref damy);
    }

    public int GetPoint(Player player,Card card,CardZoneType zone,ref GameObject target) {
        int p = 0;

        //カードのコストによる評価値の取得
        if (zone == CardZoneType.Hand)
            p -= card.State.Cost * cost_value;

        //カード効果による評価値の取得
        foreach (CardEffect effect in card.State.Effect) {
            switch (effect.GetEffectPlayType()) {
                case EffectPlayType.playEffect:
                    if (zone != CardZoneType.Unit && zone != CardZoneType.Support) {//既に場に出ているカードは発動時効果の評価値を計算しない
                        foreach (EffectState estate in effect.effectState)
                        {
                            switch (estate.effectType)
                            {
                                case EffectType.AttackTime:
                                    if (player.Buff.GetMaxAtkCount() < estate.value)
                                        p += GetAttackPoint(estate.value) - GetAttackPoint(player.Buff.GetMaxAtkCount());//(estate.value - 1) * (playerState.atk * AIConstants.ATTACK_POINT);
                                    break;
                                case EffectType.CardDraw:
                                    p += estate.value * AIConstants.CARD_POINT_REVERS;
                                    break;
                                case EffectType.CardDraft:
                                    if (estate.Target.targetZone[0] == CardZoneType.Deck)//山札を参照するとドローまで呼んでしまうので参照しない
                                        p += AIConstants.CARD_POINT_REVERS;
                                    else {
                                        List<GameObject> targets = estate.GetCanSelectTargets(player);
                                        int max = 0;
                                        int now = 0;
                                        foreach (GameObject go in targets) {
                                            if (go.GetComponent<Card>()) {
                                                now = GetPoint(player,go.GetComponent<Card>(), CardZoneType.Hand);
                                                if (now < max)
                                                    max = now;
                                            }
                                        } 
                                        p += max;
                                    }
                                    break;
                                case EffectType.CardUnDraft:
                                    p += (estate.value - 1) * AIConstants.CARD_POINT_REVERS;
                                    break;
                                case EffectType.Damage:
                                    int damageScore = 0,totalScore = 0;
                                    int damageValue = (card.State.cardType == ObjectType.Skill ? estate.value + player.SkillDamage : estate.value);

                                    target = GetMostAttackTarget(estate.GetCanSelectTargets(player), damageValue, ref damageScore, ref totalScore);
                                    if (estate.Target.isAll) p += totalScore;
                                    else p += damageScore;
                                    break;
                                case EffectType.CostAdd:
                                    p += estate.value * AIConstants.COST_POINT;
                                    break;
                            }
                        }
                    }
                    break;
                case EffectPlayType.staticEffect:
                    foreach (EffectState estate in effect.effectState) {
                        switch (estate.effectType) {
                            case EffectType.Guardian:break;
                        }
                    }
                    break;
                case EffectPlayType.activeEffect:
                    foreach (EffectState estate in effect.effectState) {
                        switch (estate.effectType) {
                            case EffectType.AttackAdd:
                                p += estate.value;
                                break;
                            case EffectType.CardDraft:
                                if (estate.Target.targetZone[0] == CardZoneType.Deck)//山札を参照するとドローまで呼んでしまうので参照しない
                                    p += AIConstants.CARD_POINT_REVERS;
                                else
                                {
                                    List<GameObject> targets = estate.GetCanSelectTargets(player);
                                    int max = 0;
                                    int now = 0;
                                    foreach (GameObject go in targets)
                                    {
                                        if (go.GetComponent<Card>())
                                        {
                                            now = GetPoint(player, go.GetComponent<Card>(), CardZoneType.Hand);
                                            if (now < max)
                                                max = now;
                                        }
                                    }
                                    p += max;
                                }
                                break;
                        }
                    }
                    break;
                case EffectPlayType.triggerEffect:
                    foreach (EffectState estate in effect.effectState) {
                        switch (estate.effectType) {
                            case EffectType.AttackAdd:
                                p += GetAttackPoint(player.Buff.GetMaxAtkCount(), player.AttackPower) - GetAttackPoint(player.Buff.GetMaxAtkCount(), player.AttackPower + estate.value);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        if (card.State.atk > 0) {
            p += GetAttackPoint(player.Buff.GetMaxAtkCount(), player.AttackPower + card.State.atk) - GetAttackPoint(player.Buff.GetMaxAtkCount(), player.AttackPower);
        }

        if (card.State.skill > 0) {
            p += card.State.skill * AIConstants.SKILL_POINT;
        }
            
        
        return p;
    }

    public GameObject GetMostAttackTarget(List<GameObject> targets,int damage,ref int max,ref int total) {
        max = 0;
        total = 0;
        int x;
        GameObject mostT = null;
        Player p;
        Card c;
        foreach (GameObject t in targets) {
            x = 0;
            if ((c = t.GetComponent<Card>())) {
                if (c.State.defence <= damage)
                    x = GetPoint(PlayerStates[0].player, c, CardZoneType.Unit);
            }
            else if ((p = t.GetComponent<Player>())) {
                x = damage - p.Defence;
                if (x >= p.Life)
                    x = 100000;
            }
            if (x > max || (mostT != null && mostT.GetComponent<IAttackTarget>() != null && mostT.GetComponent<IAttackTarget>().GetTargetPriority() > Constants.TARGET_NOMAL)) {
                max = x;
                mostT = t;
            }
            if (x > 0)
                total += x;
        }
        return mostT;
    }

    /*
    /// <summary>
    /// カードの使用順序並び替え
    /// </summary>
    /// <returns></returns>
    private List<Card> PlayListSort(List<Card> cards) {
        List<Card> sort = new List<Card>(cards);

        bool isContinue = false;
        do {
            isContinue = false;
            for (int i = 0; i < sort.Count; i++)
            {
                for (int j = i + 1; j < sort.Count; j++)
                {
                    if (isPriority(sort[i], sort[j]) > 0) {
                        Debug.Log("CardChange to " + sort[i] + ":" + sort[j]);
                        Card c = sort[i];
                        sort[i] = sort[j];
                        sort[j] = c;
                        isContinue = true;
                    }
                }
            }
        } while (isContinue);

        card = sort;

        return sort;
    }*/

    /// <summary>
    /// カードを評価点順に並び替える処理
    /// </summary>
    /// <param name="cards"></param>
    private void PlayListSort_CardPoint(List<CardPoint> cards)
    {
        bool isContinue = false;
        do
        {
            isContinue = false;
            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = i + 1; j < cards.Count; j++)
                {
                    if (isPriority(cards[i].card, cards[j].card) > 0)
                    {
                        //Debug.Log("CardChange to " + cards[i] + ":" + cards[j]);
                        CardPoint c = cards[i];
                        cards[i] = cards[j];
                        cards[j] = c;
                        isContinue = true;
                    }
                }
            }
        } while (isContinue);
    }

    /// <summary>
    /// 二つのカードがお互いのカードの発動を阻害する場合、阻害しない順番に変更する
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int isPriority(Card a,Card b){
        if (!player.AligmentCheck(a.State.cardAligment) || !player.AligmentCheck(b.State.cardAligment))
            return 0;

        switch (a.State.cardType) {
            case ObjectType.Support:
                TriggerType destTrigger = a.GetComponent<Support>().GetDestTrigger();
                switch (destTrigger)
                {
                    case TriggerType.PlayUnit:
                        if (b.State.cardType == ObjectType.Unit)
                            return 1;
                        break;
                    case TriggerType.PlaySkill:
                        if (b.State.cardType == ObjectType.Skill)
                            return 1;
                        break;
                    case TriggerType.PlayCard_4CostOrMore:
                        if (b.State.Cost >= 4 && b.State.cardType != ObjectType.Trap)
                            return 1;
                        break;
                    default:
                        break;
                }
                break;
            case ObjectType.Skill:
                if (a.State.GetEffect(EffectType.Damage) != null && b.State.skill > 0)
                    return 1;
                break;
        }
        

        return 0;
    }
}
