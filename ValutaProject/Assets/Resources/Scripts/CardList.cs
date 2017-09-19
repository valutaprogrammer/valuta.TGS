using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードの種類と数
public struct cardCount
{
    public CardState card;
    public int count;

    public void Init(CardState state, int count = 0)
    {
        card = state;
        this.count = count;
    }
}

public class CardList : MonoBehaviour {

    private static List<CardState> _CardList = null;//読み込みが一度で済むように保存しておく
    public static List<CardState> GetCardList() {
        if (_CardList != null) return _CardList;

        //直に打ち込み用
        //List<cardCount> cards = BataDeck();
        List<cardCount> cards = GatiDeck();

        //データ読み込み用
        //List<cardCount> cards = CardDataReader.ReadCardList();

        _CardList = new List<CardState>();
        foreach (cardCount c in cards) {
            for (int i = 0; i < c.count; i++)
                _CardList.Add(c.card);
        }
        return _CardList;
    }

    public static List<CardState> GetLifeCards() {
        List<cardCount> cards = new List<cardCount>();
        
        cardCount ccount = new cardCount();
        /*
        ccount.card = NormalLifeCard();
        //ライフカード効果確認用、カードドロー効果設定
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.CardDraw, 1);
        ccount.count = 1;
        cards.Add(ccount);*/

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "9";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.CardDraw, 1);
        ccount.count = 1;
        ccount.card.text = "カードを1枚引く";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "8";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.CostAdd, 3, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));
        ccount.count = 1;
        ccount.card.text = "コスト+3";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "7";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.AttackAdd, 2, 1, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));
        ccount.count = 1;
        ccount.card.text = "攻撃力+2";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "6";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.RandCardDest, 1, 0, new Target(TargetType.enemy, CardZoneType.Support, ObjectType.Support, true));
        ccount.count = 1;
        ccount.card.text = "相手の場の支援カードを\nランダムに1枚破壊する";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);
        
        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "5";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.CostAdd, 5,0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));
        ccount.count = 1;
        ccount.card.text = "コスト+5";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "4";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.Heal, 1, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));
        ccount.count = 1;
        ccount.card.text = "ライフを1回復する";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "3";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.CardDraw, 2, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));
        ccount.card.text = "カードを2枚引く";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        ccount.count = 1;
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "2";
        List<ObjectType> targtes = new List<ObjectType>() { ObjectType.Player, ObjectType.Unit };
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.Damage, 3, 0, new Target(TargetType.enemy, CardZoneType.Unit, targtes, true));
        ccount.count = 1;
        ccount.card.text = "相手ユニット全てとプレイヤーに3ダメージ";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "1";
        ccount.card.SetEffect(EffectPlayType.playEffect, EffectType.CardDraft, 3, 0, new Target(TargetType.friend, CardZoneType.Deck, ObjectType.All, true));
        ccount.count = 1;
        ccount.card.text = "山札からカードを3枚引き、\nその内1枚を選び手札に加える\n残りのカードは捨て札に送る";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.card = NormalLifeCard();
        ccount.card.cardName += "0";
        ccount.count = 1;
        ccount.card.text = "ゲームに敗北する";
        ccount.card.cardSprite = Resources.Load<Sprite>(CardSprits.Sinigami);
        cards.Add(ccount);

        List<CardState> cardList = new List<CardState>();
        foreach (cardCount c in cards)
        {
            for (int i = 0; i < c.count; i++)
                cardList.Add(c.card);
        }
        cardList.Reverse();

        return cardList;
    }

    //通常のＬカード、効果無し
    public static CardState NormalLifeCard() {
        CardState card = new CardState();
        card.cardName = "　ライフ";
        card.cardType = ObjectType.Life;
        card.cardAligment = CardAligment.All;
        card.Cost = 0;
        card.text = "（効果無し）";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Life);

        return card;
    }

    //"被攻撃時、相手の攻撃力を-3する"罠
    private static CardState AttackDebuffTrap() {
        CardState card = new CardState();
        card.cardName = "竜の咆哮";
        card.cardName_Read = "りゅうのほうこう";
        card.cardType = ObjectType.Trap;
        card.cardAligment = CardAligment.All;
        card.Cost = 3;
        card.text = "被攻撃時、相手の攻撃力を-3する";
        card.trigger = TriggerType.TakeAttack;
        //card.Effect = null;
        card.cardSprite = Resources.Load<Sprite>(CardSprits.RyuuNoHoukou);

        EffectState estate = new EffectState(EffectType.AttackAdd, -3, 1, new Target(TargetType.enemy, CardZoneType.Unit, ObjectType.Player, true));
        CardEffect e = new PlayEffect();
        e.effectState.Add(estate);
        //
        card.Effect.Add(e);

        return card;
    }

    //"被攻撃時、相手の攻撃力を-1する"罠
    private static CardState Sikabanenotate()
    {
        CardState card = new CardState();
        card.cardName = "屍の盾";
        card.cardName_Read = "しかばねのたて";
        card.cardType = ObjectType.Trap;
        card.cardAligment = CardAligment.All;
        card.Cost = 0;
        card.text = "被攻撃時、相手の攻撃力を-1する";
        card.trigger = TriggerType.TakeAttack;
        //card.Effect = null;
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Sikabanenotate);

        EffectState estate = new EffectState(EffectType.AttackAdd, -1, 1, new Target(TargetType.enemy, CardZoneType.Unit, ObjectType.Player, true));
        CardEffect e = new PlayEffect();
        e.effectState.Add(estate);
        //
        card.Effect.Add(e);

        return card;
    }

    private static CardState Eruhu() {
        CardState card = new CardState();
        card.cardName = "エルフの迎撃法";
        card.cardName_Read = "えるふのげいげきほう";
        card.cardType = ObjectType.Trap;
        card.cardAligment = CardAligment.All;
        card.Cost = 1;
        card.text = "相手のターン開始時、\nドラフトゾーンのカードを全て捨て札に送る";
        card.trigger = TriggerType.DraftStart;
        //card.Effect = null;
        card.cardSprite = null;// Resources.Load<Sprite>(CardSprits.Devil);

        EffectState estate = new EffectState(EffectType.CardDest, 0, 0, new Target(TargetType.enemy, CardZoneType.Draft, ObjectType.All, true));
        CardEffect e = new PlayEffect();
        e.effectState.Add(estate);
        //
        card.Effect.Add(e);

        return card;
    }
    /*
    //"自身の技ダメージを+2する。自身がユニットを召喚した時、このカードを破棄する。"支援
    private static CardState SkillPowerSupport() {
        CardState card = new CardState();
        card.cardName = "三頭竜の呪い";
        card.cardType = ObjectType.Support;
        card.cardAligment = CardAligment.All;
        card.Cost = 6;
        card.skill = 2;
        card.text = "自身の技ダメージを+2する\n自身がユニットを召喚した時、このカードを破棄する";
        card.trigger = TriggerType.PlayUnit;
        //card.Effect = null;
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Devil);

        return card;
    }*/

    //"技カードの発動を無効にする"
    private static CardState SkillCounterTrap() {
        CardState card = new CardState();
        card.cardName = "威圧";
        card.cardName_Read = "いあつ";
        card.cardType = ObjectType.Trap;
        card.cardAligment = CardAligment.All;
        card.Cost = 2;
        card.trigger = TriggerType.TakeSkill;
        card.text = "技カードの発動を無効にする\nその後山札からカードを1枚引く";
        //card.Effect = null;
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Iatu);

        card.SetEffect(EffectPlayType.playEffect, EffectType.Counter, 0, 0, null);

        return card;
    }

    private static CardState OverPowerTrap() {
        CardState card = SkillCounterTrap();
        card.SetEffect(EffectPlayType.playEffect, EffectType.CardDraw, 1, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));
        return card;
    }

    private static CardState BoosterDagar()
    {
        CardState card = new CardState();
        card.cardName = "パワーインストール";//"ブースターダガー";
        card.cardName_Read = "ぱわーいんすとーる";
        card.cardType = ObjectType.Unit;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 5;
        card.atk = 1;
        card.defence = 3;
        card.text = "攻撃力+1\nマナを2点支払うことで、1ターンの間攻撃力を+3する";
        //card.Effect = null;
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Powerinstall);

        card.SetEffect(EffectPlayType.activeEffect, EffectType.AttackAdd, 3, 1, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true), 2);

        return card;
    }

    private static CardState Present() {
        CardState card = new CardState();
        card.cardName = "もらいもの";
        card.cardType = ObjectType.Trap;
        card.cardAligment = CardAligment.All;
        card.Cost = 3;
        card.trigger = TriggerType.TakeDispCard;
        card.text = "相手がカードを破棄した時、そのカードを手札に加える";
        //card.Effect = null;
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Moraimono);

        card.SetEffect(EffectPlayType.playEffect, EffectType.GetDispCard, 0);

        return card;
    }


    /// //////////////////////////β版用
    /// 
    private static List<cardCount> BataDeck() {
        List<cardCount> cards = new List<cardCount>();

        int i = 3;

        cardCount ccount = new cardCount();
        ccount.Init(kurausorasu(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(SupportFairy(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(FrameDogg(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DevilContraction(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DestZoneSkillCardChoisSupport(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DevilsGift(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(SolHand(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(ThreeHedDragonsCurs(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(YerowDragonsArmer(), i);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DualAttackSkill(), i);
        //cards.Add(ccount);

        return cards;
    }

    private static List<cardCount> GatiDeck()
    {
        List<cardCount> cards = new List<cardCount>();

        int i = 1;

        cardCount ccount = new cardCount();
        ccount.Init(kurausorasu(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(SupportFairy(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(FrameDogg(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DevilContraction(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DestZoneSkillCardChoisSupport(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DevilsGift(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(SolHand(), i * 3);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(ThreeHedDragonsCurs(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(YerowDragonsArmer(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DualAttackSkill(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(BeginnerSowrdmanAira(), i * 3);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(IronKnight(), i * 3);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(GuardianShield(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(DarkMagicianRejehu(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(IndifferentBattleField(), i * 3);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(GodBreath(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(IgunisRanth(), i * 3);
        cards.Add(ccount);

        
        ccount = new cardCount();
        ccount.Init(DemonsGate(), i * 3);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(BoosterDagar(), i * 2);
        cards.Add(ccount);

        //罠
        ccount = new cardCount();
        ccount.Init(AttackDebuffTrap(), i * 2);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(OverPowerTrap(), i * 3);
        cards.Add(ccount);

        ccount = new cardCount();
        ccount.Init(Sikabanenotate(), i * 3);
        cards.Add(ccount);

        /*画像が無いので排除
        ccount = new cardCount();
        ccount.Init(Eruhu(), i * 3);
        cards.Add(ccount);*/

        ccount = new cardCount();
        ccount.Init(Present(), i * 3);
        cards.Add(ccount);

        return cards;
    }

    private static CardState kurausorasu() {
        CardState card = new CardState();
        card.cardName = "クラウ＝ソラス";
        card.cardName_Read = "くらうそらす";
        card.cardType = ObjectType.Unit;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 9;
        card.atk = 6;
        card.defence = 1;
        card.text = "攻撃力+6";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Swordman);
        card.isSpriteOnFrame = true;

        return card;
    }

    private static CardState SupportFairy() {
        CardState card = new CardState();
        card.cardName = "支援妖精";
        card.cardName_Read = "しえんようせい";
        card.cardType = ObjectType.Support;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 3;
        card.atk = 1;
        card.trigger = TriggerType.UnitDest;
        card.text = "攻撃力+1\n自身のユニットが破壊された時、このカードを破壊する";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Support_Fairy);
        card.isSpriteOnFrame = true;

        return card;
    }

    private static CardState FrameDogg()
    {
        CardState card = new CardState();
        card.cardName = "炎の番犬";
        card.cardName_Read = "ほのおのばんけん";
        card.cardType = ObjectType.Unit;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 2;
        card.atk = 1;
        card.skill = 1;
        card.defence = 4;
        card.text = "攻撃力+1\n技ダメージ+1";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Watchdog);
        card.isSpriteOnFrame = true;

        return card;
    }

    private static CardState DevilContraction()
    {
        CardState card = new CardState();
        card.cardName = "悪魔の契約";
        card.cardName_Read = "あくまのけいやく";
        card.cardType = ObjectType.Support;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 4;
        card.skill = 1;
        card.text = "技ダメージ+1\nユニットを場に出した時、このカードを破壊する";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Akumanokeiyaku);

        return card;
    }

    private static CardState DestZoneSkillCardChoisSupport()
    {
        CardState card = new CardState();
        card.cardName = "幽霊界の案内人";
        card.cardName_Read = "ゆうれいかいのあんないにん";
        card.cardType = ObjectType.Support;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 6;
        card.trigger = TriggerType.PlayUnit;
        card.text = "1ターンに1度、捨て札から技カードを1枚選び手札に加える\n自身がユニットを場に出した時、\nこのカードを破壊する";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Yurekai_No_Annainin);
        card.isSpriteOnFrame = true;

        card.SetEffect(EffectPlayType.activeEffect, EffectType.GetDisZoneCard, 0, 0, new Target(TargetType.friend, CardZoneType.Dest, ObjectType.Skill, true, CardAligment.Magician));

        return card;
    }

    private static CardState DevilsGift()
    {
        CardState card = new CardState();
        card.cardName = "悪魔くんの贈り物";
        card.cardName_Read = "あくまくんのおくりもの";
        card.cardType = ObjectType.Skill;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 3;
        card.text = "カードを3枚裏向きで引く\n相手はその中から1枚を選び、\n捨て札に送る\n残りは自分の手札に加える";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.DevilPresenst);
        card.isSpriteOnFrame = true;

        card.SetEffect(EffectPlayType.playEffect, EffectType.CardUnDraft, 3, 0, new Target(TargetType.friend, CardZoneType.Deck, ObjectType.All, true));

        return card;
    }

    private static CardState SolHand(){
        CardState card = new CardState();
        card.cardName = "ソウルハンド";
        card.cardName_Read = "そうるはんど";
        card.cardType = ObjectType.Skill;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 3;
        card.text = "相手プレイヤーに3ダメージ";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.HandSoul);

        card.SetEffect(EffectPlayType.playEffect, EffectType.Damage, 3, 0, new Target(TargetType.enemy, CardZoneType.Unit, ObjectType.Player, true));

        return card;
    }

    private static CardState ThreeHedDragonsCurs()
    {
        CardState card = new CardState();
        card.cardName = "三頭竜の呪い";
        card.cardName_Read = "さんとうりゅうののろい";
        card.cardType = ObjectType.Support;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 6;
        card.skill = 2;
        card.trigger = TriggerType.PlayUnit;
        card.text = "技ダメージ+2\n自身がユニットを場に出した時、このカードを破壊する";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.ThreeHedDragon);
        card.isSpriteOnFrame = true;

        return card;
    }

    private static CardState YerowDragonsArmer()
    {
        CardState card = new CardState();
        card.cardName = "黄龍の鎧";
        card.cardName_Read = "こうりゅうのよろい";
        card.cardType = ObjectType.Support;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 6;
        card.trigger = TriggerType.TurnStart;
        card.atk = -3;
        card.text = "攻撃力-3\nターン終了時にこのカードを破壊し、次のターン攻撃力+4";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.YerowArmer);
        card.isSpriteOnFrame = true;

        TriggerEffect effect = new TriggerEffect();
        effect.TriggerType = TriggerType.OnDest;
        effect.effectState.Add(new EffectState(EffectType.AttackAdd, 4, 1, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true)));
        card.Effect.Add(effect);

        return card;
    }

    private static CardState DualAttackSkill()
    {
        CardState card = new CardState();
        card.cardName = "蒼龍乱舞";//"双剣乱舞";
        card.cardName_Read = "そうりゅうらんぶ";//"そうけんらんぶ";
        card.cardType = ObjectType.Skill;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 6;
        card.text = "このターン自分は2回攻撃できる";
        //card.Effect = null;
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Souryuurannbu);

        card.SetEffect(EffectPlayType.playEffect, EffectType.AttackTime, 2, 1, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));

        return card;
    }

    //新米剣士アイラ
    private static CardState BeginnerSowrdmanAira() {
        CardState card = new CardState();
        card.cardName = "妖精剣士";//"新米剣士アイラ";
        card.cardName_Read = "ようせいけんし";//"しんまいけんしあいら";
        card.cardType = ObjectType.Unit;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 1;
        card.text = "攻撃力+1\n使用時、マナ+2";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Youseikennsi);

        card.atk = 1;
        card.defence = 3;
        card.SetEffect(EffectPlayType.playEffect, EffectType.CostAdd, 2, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));

        return card;
    }

    //アイアンナイト
    private static CardState IronKnight(){
        CardState card = new CardState();
        card.cardName = "アイアンナイト";
        card.cardName_Read = "あいあんないと";
        card.cardType = ObjectType.Unit;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 3;
        card.text = "攻撃力+2\n使用時、山札からカードを1枚引く";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Aiannaito);

        card.atk = 2;
        card.defence = 2;
        card.SetEffect(EffectPlayType.playEffect, EffectType.CardDraw, 1, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));

        return card;
    }

    //守護者の盾
    private static CardState GuardianShield(){
        CardState card = new CardState();
        card.cardName = "番人の盾";
        card.cardName_Read = "ばんにんのたて";
        card.cardType = ObjectType.Unit;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 4;
        card.text = "このカードが場にある限り、相手はこのカード以外を攻撃・カード効果の対象に選択できない";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.BanninNoTate);
        card.isSpriteOnFrame = true;

        card.defence = 3;
        card.SetEffect(EffectPlayType.staticEffect, EffectType.Guardian, 1, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));

        return card;
    }

    //闇魔導師レジェフ
    private static CardState DarkMagicianRejehu() {
        CardState card = new CardState();
        card.cardName = "闇魔導師レジェフ";
        card.cardName_Read = "やみまどうしれじぇふ";
        card.cardType = ObjectType.Unit;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 5;
        card.text = "攻撃力+2\n技威力+1";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.YamiMadousiRejehu);
        card.isSpriteOnFrame = true;

        card.atk = 2;
        card.skill = 1;
        card.defence = 5;

        return card;
    }
    
    //無窮の戦場
    private static CardState IndifferentBattleField() {
        CardState card = new CardState();
        card.cardName = "神父";
        card.cardName_Read = "しんぷ";
        card.cardType = ObjectType.Support;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 5;
        card.text = "自分のユニットが破壊された時、マナ+そのユニットのマナ\n自分が技カードを使用した時、このカードを破壊する";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Sinpu);

        card.trigger = TriggerType.PlaySkill;
        card.SetEffect(EffectPlayType.triggerEffect, EffectType.UnitCostAdd, 0, 0, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true), 0, TriggerType.UnitDest);

        return card;
    }

    //神光の加護
    private static CardState GodBreath(){
        CardState card = new CardState();
        card.cardName = "神光の加護";
        card.cardName_Read = "しんこうのかご";
        card.cardType = ObjectType.Skill;
        card.cardAligment = CardAligment.SowrdMan;
        card.Cost = 4;
        card.text = "攻撃力+2";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Jinnkounokago);
        
        card.SetEffect(EffectPlayType.playEffect, EffectType.AttackAdd, 2, 1, new Target(TargetType.friend, CardZoneType.Unit, ObjectType.Player, true));

        return card;
    }

    //イグニスランス
    private static CardState IgunisRanth(){
        CardState card = new CardState();
        card.cardName = "魔女の悪戯";
        card.cardName_Read = "まじょのいたずら";
        card.cardType = ObjectType.Skill;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 2;
        card.text = "相手のユニット一体に3ダメージ";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Majonoitazura);

        card.SetEffect(EffectPlayType.playEffect, EffectType.Damage, 3, 0, new Target(TargetType.enemy, CardZoneType.Unit, ObjectType.Unit, false));

        return card;
    }

    //デモンズゲート
    private static CardState DemonsGate() {
        CardState card = new CardState();
        card.cardName = "デモンズゲート";
        card.cardName_Read = "でもんずげーと";
        card.cardType = ObjectType.Skill;
        card.cardAligment = CardAligment.Magician;
        card.Cost = 4;
        card.text = "相手のプレイヤーかユニット一体に3ダメージ";
        card.cardSprite = Resources.Load<Sprite>(CardSprits.Demonsgate);

        List<ObjectType> targets = new List<ObjectType>() { ObjectType.Unit, ObjectType.Player };
        card.SetEffect(EffectPlayType.playEffect, EffectType.Damage, 3, 0, new Target(TargetType.enemy, CardZoneType.Unit, targets, false));

        return card;
    }
}
