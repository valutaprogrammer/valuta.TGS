using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    friend,//味方
    enemy,//敵
    all,//全体
}

[System.Serializable]
public class Target
{
    //対象の陣営
    public TargetType targetType;
    //対象となる場
    public List<CardZoneType> targetZone = new List<CardZoneType>();
    //対象の種類
    public List<ObjectType> targetObjectType = new List<ObjectType>();
    //
    public CardAligment targetAligment = CardAligment.All;
    //対象全てに効果を適用するか否か
    public bool isAll;
    //効果対象を選択するか
    public bool isSelect() { return (isAll); }

    //初期化子無し
    public Target() {

    }
    //Listでない初期化子のみ
    public Target(TargetType targetType, CardZoneType targetZone, ObjectType targetObjectType, bool isAll = true,CardAligment targetAligment = CardAligment.All) {
        this.targetType = targetType; this.targetZone.Add(targetZone); this.targetObjectType.Add(targetObjectType); this.isAll = isAll;this.targetAligment = targetAligment;
    }
    //Listの初期化子あり
    public Target(TargetType targetType, List<CardZoneType> targetZone, List<ObjectType> targetObjectType, bool isAll = true, CardAligment targetAligment = CardAligment.All) {
        this.targetType = targetType;this.targetZone = targetZone; this.targetObjectType = targetObjectType;this.isAll = isAll; this.targetAligment = targetAligment;
    }

    public Target(TargetType targetType, CardZoneType targetZone, List<ObjectType> targetObjectType, bool isAll)
    {
        this.targetType = targetType; this.targetZone.Add(targetZone); this.targetObjectType = targetObjectType; this.isAll = isAll;
    }

    public Target(TargetType targetType, List<CardZoneType> targetZone, ObjectType targetObjectType, bool isAll)
    {
        this.targetType = targetType; this.targetZone = targetZone; this.targetObjectType.Add(targetObjectType); this.isAll = isAll;
    }

    /// <summary>
    /// 引数のZoneTypeが対象に含まれるか確認
    /// </summary>
    /// <param name="value">確認する場の種類</param>
    /// <returns></returns>
    private bool isTargetZone(CardZoneType value)
    {
        foreach (CardZoneType zone in targetZone)
        {
            if (zone == value) return true;
        }
        return false;
    }

    /// <summary>
    /// 引数のObjectTypeが対象に含まれるか確認
    /// </summary>
    /// <param name="value">確認するObjectType</param>
    /// <returns></returns>
    private bool isTargetObjectType(ObjectType value)
    {
        foreach (ObjectType objType in targetObjectType)
        {
            if (value == ObjectType.All || objType == value) return true;
        }
        return false;
    }

    //効果対象の取得
    public List<GameObject> GetTargets(Player caster,int count = 0)
    {
        List<GameObject> targtes = new List<GameObject>();

        //デッキから対象を取得する場合、ドローしたものとして扱う
        if (targetZone.Count == 1 && targetZone[0] == CardZoneType.Deck) {
            List<Card> cards = Constants.BATTLE.CardDraw(count);
            List<GameObject> ret = new List<GameObject>();
            foreach (Card c in cards) {
                Debug.Log(c.gameObject);
                ret.Add(c.gameObject);
            }
            return ret;
        }

        switch (targetType)
        {
            case TargetType.friend:
                FindTargets(caster, targtes, targetAligment);
                break;
            case TargetType.enemy:
                FindTargets(Constants.BATTLE.GetOtherPlayer(caster), targtes, targetAligment);
                break;
            case TargetType.all:
            default:
                FindTargets(caster, targtes, targetAligment);
                FindTargets(Constants.BATTLE.GetOtherPlayer(caster), targtes, targetAligment);
                break;
        }

        return targtes;
    }

    //渡されたList<GameObject>tに<Player>pの陣営の対象となりうるオブジェクトを格納して返す
    private void FindTargets(Player p, List<GameObject> targets,CardAligment aligment = CardAligment.All)
    {
        foreach (CardZoneType zone in targetZone)
        {
            CardZone zoneObj = Constants.BATTLE.GetCardZone(zone, p);
            if (zone == CardZoneType.Deck) {
                continue;
            }
            foreach (ObjectType objT in targetObjectType) {
                zoneObj.FindCardType(objT, targets,aligment);
            }
        }
        foreach (ObjectType t in targetObjectType) if (t == ObjectType.Player) targets.Add(p.gameObject);
    }

    /// <summary>
    /// 引数のGameObjectが対象として正しいか確認　casterも入れるとより詳細に確認
    /// </summary>
    /// <param name="target">確認する効果対象</param>
    /// <param name="caster">このカードを使用するPlayer</param>
    /// <returns></returns>
    public bool isTarget(GameObject target,Player caster = null,Player targetHolder = null) {
        ObjectType objType;
        CardZoneType? zoneType = null;
        
        if (isAll) return true;

        if (caster && targetHolder) {
            bool isOK = false;
            switch (targetType){
                case TargetType.all:
                    isOK = true;
                    break;
                case TargetType.enemy:
                    isOK = (caster != targetHolder);
                    break;
                case TargetType.friend:
                    isOK = (caster == targetHolder);
                    break;
                default: return false;
            }
            if (!isOK) return false;
        }
        if (!target) return false;

        Player p = target.GetComponent<Player>();
        Card c = target.GetComponent<Card>();
        if (p)
        {
            objType = ObjectType.Player;
        }
        else if (c)
        {
            objType = c.State.cardType;
            CardZone holdZone = Constants.BATTLE.FindCardStayZone(c);
            zoneType = holdZone.GetCardZoneType();
        }
        else return false;

        bool isObj = false;
        foreach (ObjectType o in targetObjectType) {
            if (o == objType) isObj = true;
        }
        bool isZone = (p);
        foreach (CardZoneType z in targetZone) {
            if (z == zoneType) isZone = true;
        }

        return (isObj && isZone);
    }
}