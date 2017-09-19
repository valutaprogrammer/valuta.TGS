using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ターンのフェイズ
public enum GamePhase {
    start,          //開始
    draft,          //ドラフト
    main,           //行動
    end,            //終了
    handDest,       //手札を捨てる
    wait,           //その他処理待機中
    //以下一度のみのフェイズ
    aligment,       //陣営選択
    coinTos,        //順番決め 
    mulligan,       //マリガン
    gameEnd,        //終了
}

//実行中の処理
public enum Proces {
    none,           //無し 行動選択可能
    stack,          //スタック処理
    effect,         //効果の発動
    trigger,        //誘発強制処理待機
    TrapSelect,     //罠カード発動選択待機
}

//EventBlock効果解決時用コールバック
public delegate void Resolve();

//バトルシーンのManagerClass
public class BattleManager : MonoBehaviour
{
    [SerializeField]
    public Player[] Players = new Player[2];
    public Player GetActivePlayer() { return ActivePlayer; }
    public Player GetOtherPlayer(Player p) { return p == Players[0] ? Players[1] : Players[0]; }
    [SerializeField]
    private Player ActivePlayer;

    [SerializeField]
    public Field Field;

    [SerializeField]
    public Canvas EffectCanvas;
    [SerializeField]
    private GameObject GameEndButton;

    [SerializeField]
    public Stack<EventBlock> EventStack = new Stack<EventBlock>();
    public EventBlock ResolvedBlock;

    //スタックの追加
    public void AddStack(EventBlock block) {
        if (EventStack.Count <= 0) EventStack.Push(new StackEndBlock(block.GetCaster()));
        EventStack.Push(block);
        isStackResolved = false;
        StartCoroutine(CallTrigger(block));
        //Debug.Log(block);
    }

    //スタックの解決
    public void ResolveStack(EventBlock block = null) {
        if (EventStack.Count == 0) return;
        if (block != null && EventStack.Peek() != block) {
            //Debug.LogAssertion("スタック順が正しく処理されていません");
        }
        ResolvedBlock = EventStack.Peek();
        StartCoroutine(EventStack.Pop().ResolveBlock());
        if (EventStack.Count <= 0) Refresh();
        //isStackResolved = false;
    }

    private bool isStackResolved;//前回追加されたスタックの処理が終わったか否か（罠等、他カードの処理を待機する場合のフラグ）
    public bool RefreshStack() {
        isStackResolved = true;
        if (EventStack.Count != 0 && EventStack.Peek().GetType() == typeof(StackEndBlock))
            EventStack.Pop();
        else return false;
        //Debug.Log("stackRefresh");
        Refresh();
        return true;
    }

    //スタックの消去
    public void DeleteStack() {
        if (EventStack.Count == 0) return;
        EventStack.Pop();
        if (EventStack.Count <= 0) Refresh();
    }

    /// <summary>
    /// 補助効果の更新
    /// </summary>
    private void Refresh() {
        foreach (Player p in Players) Players[0].Refresh();
    }

    //public static BattleManager BM;//Constantsに移動しました。
    void Awake()
    {
        if (Constants.BATTLE == null) Constants.BATTLE = this;
        else Destroy(this);
        GameEndButton.SetActive(false);
    }

    [SerializeField]
    private GamePhase phase;
    public GamePhase GetTurnPhase() { return phase; }
    /// <summary>
    /// Phaseの変更処理　引数のPhaseを開始する
    /// </summary>
    /// <param name="p">どの種類のPhaseか</param>
    /// <param name="player">どちらのPlayerが主となるか</param>
    private void SetTurnPhase(GamePhase p,Player player = null) {
        if (phase == GamePhase.gameEnd) return;
        string message = "";
        float mTime = 2.5f;
        switch (p) {
            case GamePhase.aligment:
                message = "陣営を選択してください";
                break;
            case GamePhase.coinTos:
                //message = "順番を選択してください";
                break;
            case GamePhase.mulligan:
                //message = "マリガン開始";
                break;
            case GamePhase.draft:
                //message = "Draft Phase";
                if (ActivePlayer.isAI)
                    StartCoroutine(ActivePlayer.GetComponent<Scond_AIScript>().Draft());
                break;
            case GamePhase.end:
                //message = "End Phase";
                break;
            case GamePhase.gameEnd:
                /*if (player)
                    message = (player.cardAligment == CardAligment.SowrdMan ? "剣装士" : "契約者") + "の勝利";
                else message = "Game Over";*/
                //gameObject.AddComponent<TuochSceneMove>().SceneNumber = 0;
                Sprite sprite = null;
                mTime = 10000;
                if (player)
                    sprite = (player.cardAligment == CardAligment.SowrdMan ? MessageUI.Message.Win_S : MessageUI.Message.Win_M);
                else message = "Game Over";
                MessageUI.Message.AddSpriteFade(sprite, mTime);
                GameEndButton.SetActive(true);
                mTime = 10000;
                foreach (Player P in Players)
                    if (P.isAI)
                        P.GetComponent<Scond_AIScript>().StopAllCoroutines();
                break;
            case GamePhase.handDest:
                message = "手札上限を超えています\n手札を捨ててください";
                if (ActivePlayer.isAI)
                    StartCoroutine(ActivePlayer.GetComponent<AIScript>().HandDestPhase());
                break;
            case GamePhase.main:
                //message = "Main Phase";
                MessageUI.Message.AddSpriteFade(MessageUI.Message.MainPhase, MessageUI.SpriteFadeTime);
                if (ActivePlayer.isAI)
                    StartCoroutine(ActivePlayer.GetComponent<Scond_AIScript>().Think());
                break;
            case GamePhase.start:
                //message = "バトル開始";
                break;
            default:
                message = p.ToString();
                return;
        }
        if (message != "")
            MessageUI.Message.AddMessage(message, mTime);
        phase = p;
    }

    [SerializeField]
    private Proces proces;
    public Proces GetProces() { return proces; }

    //現在実行中のEffect数 1以上なら各処理をEffect終了まで待機
    //private int effectCount = 0;
    //public void SetEffect() { effectCount++; }
    //void RemoveEffect() { effectCount--; }

    void Start() {
        GameInit();
    }

    //ゲーム開始時の初期化
    public void GameInit() {
        Field.Init();
        foreach (Player p in Players) { p.Init(); }
        AligmentSelect();
    }

	//陣営の選択開始
    private void AligmentSelect() {
        SetTurnPhase(GamePhase.aligment);
        AligmentButton.aligment.SelectStart();
    }

    //プレイヤーの陣営決定　*a版用に1pだけ任意決定、2pは選ばれなかった方になる
    public void AligmentEnter(CardAligment aligment) {
        Players[0].cardAligment = aligment;
        if (aligment == CardAligment.Magician)
            Players[1].cardAligment = CardAligment.SowrdMan;
        else Players[1].cardAligment = CardAligment.Magician;

        AligmentButton.aligment.AligmentEnter();
        CoinTosStart();
    }

	//先攻後攻の選択開始
    private void CoinTosStart() {
        CoinTosButton.CTos.SelectStart();
        SetTurnPhase(GamePhase.coinTos);
    }

    //先攻プレイヤーの決定
    public void CoinTos(Player p)
    {
        if (Players[0] == p) { ActivePlayer = Players[0]; }
        else { ActivePlayer = Players[1]; }
        CoinTosButton.CTos.SelectEnd();
        StartCoroutine(TurnStart());
    }

    //マリガンの開始
    public void MulliganStart()
    {
        SetTurnPhase(GamePhase.mulligan);
    }

    /// <summary>
    /// マリガンの開始と終了まで待機
    /// </summary>
    /// <returns></returns>
    public IEnumerator MulliganWait() {
        MulliganStart();
        while (phase == GamePhase.mulligan) {
            yield return null;
        }
    }

    /// <summary>
    /// マリガンでデッキに戻すカードを両者が決定した際の処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator MulliganEnter() {
        Coroutine[] draws = new Coroutine[Players.Length];
        for (int i = 0; i < Players.Length; i++) {
            draws[i] = StartCoroutine(Players[i].GetMulliganManager().CardDraw());
            yield return new WaitForSeconds(MulliganManaegr.wait / 2);
        }

        //全員のドロー終了まで待機
        foreach (Coroutine draw in draws) {
            yield return draw;
        }

        //終了した後、少し待機する
        yield return new WaitForSeconds(MulliganManaegr.wait);

        //カードをデッキに戻す
        foreach (Player p in Players) {
            yield return StartCoroutine(p.GetMulliganManager().CardRestore());
        }
        MulliganEnd();
    }

    public void MulliganEnd()
    {
        SetTurnPhase(GamePhase.draft);
    }

    //ターンの終了
    public bool TurnEnd() {
        if (phase == GamePhase.gameEnd) return true;
        if (ActivePlayer.GetHandZone().isHandOver())
        {
            HandDestStart();
            return false;
        }
        else {
            SetTurnPhase(GamePhase.wait);
            new TurnEndBlock(ActivePlayer).AddBlock();//TurnChange();
            return true;
        } 
    }

	//手札破棄開始
    public void HandDestStart() {
        SetTurnPhase(GamePhase.handDest);
    }

    public bool HandDest(Card c) {
        CardDisp(c);
        return HandDestEnd();
    }

	//手札破棄終了
    public bool HandDestEnd() {
        return TurnEnd();
    }

    //ターン切替
    public void TurnChange() {
        ActivePlayer.TurnEnd();
		ActivePlayer = GetOtherPlayer (ActivePlayer);
        StartCoroutine(TurnStart());
    }
    
    private int turnCount = 0;
    //ターン開始
    private IEnumerator TurnStart() {
        Sprite sprite = ActivePlayer.isAI ? MessageUI.Message.EnemyTurn : MessageUI.Message.Yourturn;

        //初ターンのみ初期動作を行う
        if (turnCount++ == 0) {
            yield return StartCoroutine(GameInitHandDraw());
            yield return StartCoroutine(MulliganWait());
            yield return new WaitForSeconds(0.5f);
            MessageUI.Message.AddSpriteFade(sprite, MessageUI.SpriteFadeTime);
            while (MessageUI.CorFade != null)
                yield return null;
            new DraftStartBlock(ActivePlayer).AddBlock();
        }
        else {
            MessageUI.Message.AddSpriteFade(sprite, MessageUI.SpriteFadeTime);
            while (MessageUI.CorFade != null)
                yield return null;
            new DraftStartBlock(ActivePlayer).AddBlock();
        }

        ActivePlayer.TurnStart();
    }

    //ドラフトフェイズの開始
    public IEnumerator DraftStart() {
        DraftButton.dButton.OnClick(true);
        SetTurnPhase(GamePhase.draft);
        if (Field.GetDraftZone().isCharge()) {
            yield return new WaitForSeconds(DraftButton.moveTime);
            Field.DraftCharge();
        }
    }

    //山札からカードを引く
    public List<Card> CardDraw(int count = 1) {
		return Field.DeckGetCard(count);
    }

    /// <summary>
    /// ゲーム開始時のカードドロー
    /// </summary>
    private IEnumerator GameInitHandDraw() {
        foreach (Player p in Players)
        {
            for (int i = 0; i < Constants.START_HAND; i++)
            {
                p.CardDraw();
                yield return new WaitForSeconds(0.2f);
            }
        }
        
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    /// <param name="winPlayer">勝利したPlayer</param>
    public void GameEnd(Player winPlayer) {
        SetTurnPhase(GamePhase.gameEnd,winPlayer);
        foreach (Player p in Players) {
            if (p.GetComponent<AIScript>()) p.GetComponent<AIScript>().StopAllCoroutines();
        }
    }

    public void GameEnd()
    {
        foreach (Player p in Players) {
            if (p.GetLifeZone().GetCardsCount() <= 0) {
                GameEnd(GetOtherPlayer(p));
            }
        }
    }

    //---------ゲームフェイズここまで


    //---------カード操作ここから

    /// <summary>
    /// ドラフトカードを手札に加える処理
    /// </summary>
    /// <param name="card">対象のカード</param>
    public void DraftDraw(Card card) {
        if (ActivePlayer == null) return;
        ActivePlayer.AddHand(card);
        if (Field.isDraftCharge()) Field.DraftCharge();
        DraftButton.dButton.OnClick(false);
        SetTurnPhase(GamePhase.main);
    }

    /// <summary>
    /// 手札から対象のカードを使用する処理
    /// </summary>
    /// <param name="card">使用するカード</param>
    /// <param name="target">カードの使用対象</param>
    /// <returns>使用が成功したか否か</returns>
    public bool CardPlay(Card card,GameObject target = null) {
        //カードがどちらのプレイヤーのものか判別
        Player caster = FindCardHolder(card);
        if (!caster) {
            //Debug.Log("カードを使用するプレイヤーが不明です");
            return false;
        }

        //カードの使用先を確認
        CardZone zone = FindCardPlayZone(card, caster);
        if (zone.isAddCard()) zone.AddCard(card);

        //カードの使用イベントをスタックに追加
        new CardPlayBlock(card, caster,target).AddBlock();

        //Debug.Log(card +  "が" + zone.name + "へ移動");
        return true;
    }

    public IEnumerator CardPlayEffectPlay(Card card,Player caster,GameObject target = null) {
        foreach (CardEffect e in card.State.Effect) {
            if (e.GetEffectPlayType() == EffectPlayType.playEffect)
                yield return StartCoroutine(e.EffectCall(caster, card, target));
        }
    }

    public void CardActiveEffecPlay(Card card) {
        Player caster = FindCardHolder(card);
        foreach (CardEffect e in card.State.Effect) {
            if (e.GetType() == typeof(ActiveEffect) && ((e) as ActiveEffect).cost <= caster.cost)
                StartCoroutine(e.EffectCall(caster, card));
            else if(e.GetType() == typeof(ActiveEffect)) Debug.Log(caster.cost + ":" + (e as ActiveEffect).cost);
        }
    }

    /// <summary>
    /// カードがターゲットの場に対して使用可能か確認
    /// </summary>
    /// <returns><c>true</c>, カードが使用可能な状態 <c>false</c> カードが使用できない状態.</returns>
    /// <param name="card">使用するカード.</param>
    /// <param name="targetZone">カードの使用先.</param>
    /// <param name="target">カードの対象　スキルカード等の効果対象</param>
    public bool isCardPlay(Card card,CardZone targetZone,GameObject target = null){
		Player caster = FindCardHolder(card);
        //if (targetZone == FindCardPlayZone(card, caster) && targetZone.isAddCard()) return true;
        if (isCardPlayZone(card, caster, targetZone)) {
            //スキルの場合、効果対象が正しいか確認する
            Skill sCard = card.GetComponent<Skill>();
            if (sCard) return sCard.isPlay(target, caster);
            return true;
        }
        return false;
	}

    /// <summary>
    /// カードの使用先CardZoneがそのCardに対応したものか確認
    /// </summary>
    /// <param name="card">使用するカード</param>
    /// <param name="caster">使用するカードの持ち主</param>
    /// <param name="zone">使用先のゾーン</param>
    /// <returns></returns>
    public bool isCardPlayZone(Card card,Player caster,CardZone zone)
    {
        //手札には使用できない
        if (zone != null && zone.GetType() == typeof(HandManager)) return false;
        CardZone canPlayZone = FindCardPlayZone(card, caster);
        if (zone == canPlayZone) return zone.isAddCard();
        //使用先が墓地（どこでもいい）ならtrue
        if (canPlayZone.GetType() == typeof(DisCardManager)) return true;
        return false;
    }

    /// <summary>
    /// カード効果がターゲットのカードに対して使用可能か確認
    /// </summary>
    /// <returns><c>true</c>, カードが使用可能な状態, <c>false</c> カードが使用できない状態.</returns>
    /// <param name="card">使用するカードm>
    /// <param name="targetCard">カードの使用先.</param>
    public bool isCardEffectPlay(CardEffect effect,Card targetCard){
		return true;
	}

    /// <summary>
    /// 各種イベント発生時に呼び出される誘発イベント確認処理
    /// </summary>
    /// <param name="type"></param>
    /// <param name="caster"></param>
    public IEnumerator CallTrigger(EventBlock block) {
        List<TriggerBlock> triggers = block.GetTrigger();
        //誘発効果を確認
        foreach (TriggerBlock t in triggers)
        {
            yield return StartCoroutine(TriggerCheck(t.trigger, t.caster, block));
            //Debug.Log(t + "確認");
        }
        //ResolveBlock();
        ResolveStack(block);
        //Debug.Log(block + "終了");
    }

    /// <summary>
    /// カードの誘発条件となり得るイベントが発生した時、それに対する誘発効果や罠の発動を呼び出す処理
    /// </summary>
    /// <param name="type">誘発イベントの種類</param>
    /// <param name="caster">誘発イベントの発動者</param>
    private IEnumerator TriggerCheck(TriggerType type,Player caster = null,EventBlock block = null) {
        //支援カードの破棄条件等、効果発動より先に発動するルール効果
        //Debug.Log("ルール効果確認");
        yield return StartCoroutine(SupportDestTriggerCheck(type, caster));

        //発動済みカードの誘発効果等、プレイヤーの承認を待たない誘発効果
        if (caster != null)
            yield return StartCoroutine(caster.GetSupportZone().EffectTriggerCheck(type, block));


        //Debug.Log("罠カード発動確認");
        //罠カードの発動等、プレイヤーの承認を待つ効果
        yield return StartCoroutine(TrapPlayTime(type, caster, block));

        //効果を解決
        //Debug.Log("トリガー確認終了");
    }

    /// <summary>
    /// 支援カードの破棄条件を確認し、満たしていたならば破棄する。確認と破棄が終わるまで待機する
    /// </summary>
    /// <param name="type">破棄条件の種類</param>
    /// <param name="caster">対象となるプレイヤー</param>
    /// <returns></returns>
    public IEnumerator SupportDestTriggerCheck(TriggerType type,Player caster) {
        if (caster == null) yield break;
        yield return StartCoroutine(caster.SupportDestTriggerCheck(type));
    }

    /// <summary>
    /// 罠カードの発動待機
    /// </summary>
    /// <param name="trigger">発動できる罠カードの発動条件</param>
    /// <param name="caster">カードの発動権を持つプレイヤー</param>
    /// <returns></returns>
    public IEnumerator TrapPlayTime(TriggerType trigger,Player caster,EventBlock block) {
        //Debug.Log("Trap wait... as " + trigger);
        List<Card> cards = caster.GetCanPlayTrapCards(trigger);
        string text = "";
        switch (trigger) {
            case TriggerType.TakeAttack:
                AttackBlock atkBlock = block as AttackBlock;
                string targetName = "";
                GameObject target = atkBlock.GetTarget();
                if (target.GetComponent<Player>()) {
                    targetName = "あなた";
                }
                else {
                    targetName = target.GetComponent<Card>().State.cardName;
                }
                text = "相手が" + targetName + "に攻撃しました";
                break;
            case TriggerType.TakeSkill:
                CardPlayBlock playBlock = block as CardPlayBlock;
                string skillName = playBlock.GetCards()[0].State.cardName;
                string skillTargetName = "";
                if (playBlock.GetTarget()) {
                    if (playBlock.GetTarget().GetComponent<Player>()) {
                        skillTargetName = "あなた";
                    }
                    else {
                        skillTargetName = playBlock.GetTarget().GetComponent<Card>().State.cardName;
                    }
                    skillTargetName += "に";
                }

                text = "相手が" + skillTargetName + skillName + "を発動しました";
                break;
            case TriggerType.DraftStart:
                text = "相手ターン開始時です";
                break;
            case TriggerType.TakeDispCard:
                CardDispBlock dispBlock = block as CardDispBlock;
                Card targetCard = dispBlock.GetCards()[0];
                foreach (Card c in cards) {
                    foreach (CardEffect ce in c.State.Effect) {
                        foreach (EffectState es in ce.effectState) {
                            es.SetTarget(targetCard.gameObject);
                        }
                    }
                }
                string dispCardName = dispBlock.GetCards()[0].State.cardName;
                text = "相手が" + dispCardName + "を捨てました";
                break;
        }
        text += "\n罠カードを発動しますか？";
        //発動可能なカードがあれば発動確認ウィンドウを出す
        if (cards.Count > 0) {
            if (!caster.isAI)
                yield return StartCoroutine(CardPlayWindow.CPW.CallWindow(text, cards, OnSelectedTrapCard));
            else {
                yield return StartCoroutine(caster.GetComponent<Scond_AIScript>().Trap(trigger, cards, block));
            }
        }
    }

    /// <summary>
    /// 選択された罠カードを発動するコールバック用関数
    /// </summary>
    /// <param name="card">選択されたカード</param>
    /// <returns></returns>
    public IEnumerator OnSelectedTrapCard(Card card) {
        if (card == null) yield break;
        //対象カードを発動する
        //int stack = EventStack.Count;
        if (CardOpen(card)) {

        }
        else {
            Debug.Log("発動に失敗");
        }
        //効果終了まで待機
        isStackResolved = false;
        while (!isStackResolved) {
            yield return null;
            Debug.Log("効果終了まで待機");
        }
        yield return new WaitForSeconds(2.0f);
    }

    /// <summary>
    /// 手札から罠を伏せる処理
    /// </summary>
    /// <param name="card">伏せるカード</param>
    /// <returns>処理が成功したか</returns>
    public bool CardSet(Card card) {
        //カードがどちらのプレイヤーのものか判別
        Player caster = FindCardHolder(card);
        if (!caster) {
            //Debug.LogAssertion("カードを使用するプレイヤーが不明です");
            return false;
        }
        //カードの使用先を確認
        CardZone zone = caster.GetSupportZone();
        if (zone.isAddCard() && zone.AddCard(card))
        {
            card.SetIsSet(true);
            //Debug.Log(card + "を" + zone.name + "にセット");
        }
        else {
            Debug.Log("罠の使用に失敗しました");
            caster.GetHandZone().SortUpdate();
        }
        return true;
    }

    /// <summary>
    /// 伏せた罠カードを発動させる処理
    /// </summary>
    /// <param name="card">発動する罠カード</param>
    /// <param name="target">罠カードの対象</param>
    /// <returns>発動が成功したか</returns>
    public bool CardOpen(Card card,GameObject target = null) {
        Player caster = FindCardHolder(card);
        if (caster.cost >= card.State.Cost)
            if (CardPlay(card)) {
                caster.cost -= card.State.Cost;
                card.SetIsSet(false);
                return true;
            }
        return false;
    }

    //カード破壊
    public void CardDest(Card card) {
        //Field.DestAddCard(card);
        new CardDestBlock(card, FindCardHolder(card)).AddBlock();
    }

    //カード破棄 isSingleはカードが単体か否か CardDisp(List<Card>)から呼ばれた場合はfalse
    public void CardDisp(Card card, bool isSingle = true){
        //Field.DestAddCard(card);
        new CardDispBlock(card, ActivePlayer).AddBlock();
    }

    //カード破棄
    public void CardDisp(List<Card> cards) {
        //foreach (Card c in cards) CardDisp(c);
        new CardDispBlock(cards, ActivePlayer).AddBlock();
    }

    //カード撤退
    public void CardPullOut(Card card) { Field.DestAddCard(card); }
    
    /// <summary>
    /// 引数のカードの所持プレイヤーを返す処理
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public Player FindCardHolder(Card card) {
        foreach (Player p in Players) {
            if (p.FindCard(card) != null) return p;
        } 
        return null;
    }

    /// <summary>
    /// 引数のオブジェクトを持っているPlayerを確認する処理
    /// </summary>
    /// <param name="obj">対象のオブジェクト</param>
    /// <returns>引数のオブジェクトを持つPlayer　いなければnull</returns>
    public Player FindObjectHolder(GameObject obj) {
        if (!obj) return null;
        Card c = obj.GetComponent<Card>();
        if (c) return FindCardHolder(c);
        return obj.GetComponent<Player>();
    }

    /// <summary>
    /// 引数のカードゾーンを持つプレイヤーを確認する処理
    /// </summary>
    /// <param name="zone">対象のカードゾーン</param>
    /// <returns>プレイヤー、見つからなければnull</returns>
    public Player FindCardZoneHolder(CardZone zone) {
        foreach (Player p in Players)
            if (p.isHaveZone(zone)) return p;

        return null;
    }

    /// <summary>
    /// カードが現在存在する場を返す処理
    /// </summary>
    /// <param name="card">対象のオブジェクト</param>
    /// <returns>カードが存在する場　存在しなければnull</returns>
    public CardZone FindCardStayZone(Card card) {
        CardZone zone = null;
        foreach (Player p in Players) {
            if ((zone = p.FindCard(card)) != null) return zone;
        }
        if (Constants.CARD_SELECT_EFFECT.Zone.FindCard(card)) return Constants.CARD_SELECT_EFFECT.Zone;
        return Field.FindCard(card);
    }

    /// <summary>
    /// カードの使用先を確認する処理
    /// </summary>
    /// <param name="card">対象のカード</param>
    /// <param name="caster">カードの保持Player</param>
    /// <returns>カード使用先の場　存在しないなら墓地</returns>
    public CardZone FindCardPlayZone(Card card, Player caster) {
        switch (card.State.cardType)
        {
            case ObjectType.Unit:
                return caster.GetUnitZone();
            case ObjectType.Support:
                return caster.GetSupportZone();
            case ObjectType.Trap://罠は使用したら墓地へ行くため墓地指定
            case ObjectType.Skill:
            case ObjectType.Life:
            default:
                return Field.GetDisCardZone();
        }
    }

    /// <summary>
    /// カードを生成する処理
    /// </summary>
    /// <param name="cardState">生成するカードのステータス</param>
    /// <returns>生成したカード</returns>
    public Card InitCard(CardState cardState) {
        GameObject cardObj = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
        Card card = SetCardType(cardObj,cardState.cardType);
        card.State = cardState;
        card.name = cardState.cardName;
        card.GetComponent<uicard>().Init();
        return card;
    }

    /// <summary>
    /// カードの型を判別し、Gameobjectに付与する処理
    /// </summary>
    /// <param name="card">対象のオブジェクト</param>
    /// <param name="type">生成したいカードのタイプ</param>
    /// <returns>付与したカード派生型</returns>
    private Card SetCardType(GameObject card, ObjectType type) {
        switch (type) {
            case ObjectType.Unit:     return card.AddComponent<Unit>();
            case ObjectType.Skill:    return card.AddComponent<Skill>();
            case ObjectType.Support:  return card.AddComponent<Support>();
            case ObjectType.Trap:     return card.AddComponent<Trap>();
            case ObjectType.Life:     return card.AddComponent<Life>();
            default:                return card.AddComponent<Card>();
        }
    }

    /// <summary>
    /// カードゾーンタイプに対応するCardZoneを返す
    /// </summary>
    /// <param name="type">探すCardZoneの種類</param>
    /// <param name="p">CardZoneを保持するプレイヤー</param>
    /// <returns>探したCardZone　存在しないならnull</returns>
    public CardZone GetCardZone(CardZoneType type,Player p = null) {
        if (p == null && isPlayerHaveZone(type)) return null;
        switch (type) {
            case CardZoneType.Deck:
                return Field.GetDeckZone();
            case CardZoneType.Draft:
                return Field.GetDraftZone();
            case CardZoneType.Dest:
                return Field.GetDisCardZone();
            case CardZoneType.Hand:
                return p.GetHandZone();
            case CardZoneType.Unit:
                return p.GetUnitZone();
            case CardZoneType.Support:
                return p.GetSupportZone();
            default:return null;
        }
    }

    /// <summary>
    /// 引数のタイプの場がプレイヤーごとに保持するものか確認
    /// </summary>
    /// <param name="type">確認する場の種類</param>
    /// <returns>その場がプレイヤーが保持するタイプか否か</returns>
    private bool isPlayerHaveZone(CardZoneType type) {
        if (type == CardZoneType.Hand || type == CardZoneType.Support || type == CardZoneType.Unit) return true;
        return false;
    }

    /// <summary>
    /// カードが起動効果を発動できるか確認
    /// </summary>
    /// <param name="card">起動効果を発動するカード</param>
    /// <returns></returns>
    public bool isActiveEffectPlay(Card card,bool isAI = false,int? haveCost = null) {
        if (card.RunTimeState.isActiveEffectPlayed) return false;
        switch (card.State.cardType) {
            case ObjectType.Unit:
            case ObjectType.Support:
                Player p = FindCardHolder(card);
                if (p == ActivePlayer && (isAI || FindCardStayZone(card) == FindCardPlayZone(card, p))) {
                    if (!haveCost.HasValue) haveCost = p.cost;
                    foreach (CardEffect e in card.State.Effect)
                        if (e.GetType() == typeof(ActiveEffect))
                            return ((e as ActiveEffect).isPlay(Constants.BATTLE.FindCardHolder(card), haveCost));
                }
                break;
            default: break;
        }
        Debug.Log("起動効果が発動できません");
        return false;
    }

    /// <summary>
    /// 引数のオブジェクトが攻撃対象として正しいか確認
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsAttackTarget(GameObject target) {
        if (target == null) return false;
        Player p = target.GetComponent<Player>();
        if (p == null) {
            Card c = target.GetComponent<Unit>();
            p = FindCardHolder(c);
            if (p == null) return false;
        }

        bool canAttack = true;
        int maxTargetPriority = Constants.TARGET_NOMAL;
        if (p.GetTargetPriority() > maxTargetPriority) {
            maxTargetPriority = p.GetTargetPriority();
            if (target != p.gameObject) canAttack = false;
        }
        IAttackTarget ia;
        foreach (Card c in p.GetUnitZone().GetCards()) {
            ia = c.GetComponent<IAttackTarget>();
            if (ia != null && ia.GetTargetPriority() > maxTargetPriority) {
                maxTargetPriority = ia.GetTargetPriority();
                if (target != c.gameObject) canAttack = false;
            } 
        }

        return canAttack;
    }
}