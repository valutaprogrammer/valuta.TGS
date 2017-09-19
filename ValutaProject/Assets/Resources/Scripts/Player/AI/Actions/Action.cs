using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType {
    attack,//攻撃
    cardPlay,//カード使用
    activeEffect,//起動効果
    cardDisp,//カード撤退
    turnEnd,//ターン終了
}

public class Action {

    public Action(Player player) {
        this.player = player;
    }

    public int point;
    protected Player player;
    public int cost = 0;

    /// <summary>
    /// 行動の種別を取得
    /// </summary>
    /// <returns></returns>
    public virtual ActionType GetActionType() {
        return ActionType.turnEnd;
    }

    public virtual bool PointUpdate(AIScript.PlayerState[] PlayerStates) {
        return false;
    }

    /// <summary>
    /// 行動を実行
    /// </summary>
    /// <returns></returns>
    public virtual bool ActionEnter() {
        return false;
    }

    public virtual string GetActionName() {
        return point + "点 行動";
    }
}
