using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Attack : Action
{
    public Action_Attack(Player player,GameObject target) : base(player) {
        this.target = target;
    }

    public GameObject target;


    public override bool PointUpdate(AIScript.PlayerState[] PlayerStates) {
        int p = GetAttackPoint(PlayerStates);
        if (point != p) {
            point = p;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 攻撃評価値の更新
    /// </summary>
    /// <param name="PlayerStates">プレイヤーの情報</param>
    /// <param name="attackCount">攻撃回数</param>
    /// <param name="attackPower">攻撃力</param>
    /// <returns></returns>
    public int GetAttackPoint(AIScript.PlayerState[] PlayerStates,int attackCount = 1,int attackPower = 0) {
        if (attackCount == 0) return 0;
        if (attackPower == 0) attackPower = player.AttackPower;
        List<int> points = new List<int>();
        int point = 0;
        foreach (CardPoint c in PlayerStates[1].Unit) {
            //攻撃対象が限定されている場合、その対象に対して必ず攻撃する
            if (c.card.GetComponent<IAttackTarget>().GetTargetPriority() > Constants.TARGET_NOMAL) {
                if (player.AttackPower >= c.card.State.defence) {
                    attackCount--;
                    point += c.point;
                }
                else attackCount = 0;
            }
            //if (Constants.BATTLE.IsAttackTarget(c.card.gameObject))
            else if(player.AttackPower >= c.card.State.defence)
                points.Add(c.point);
        }

        if (Constants.BATTLE.IsAttackTarget(Constants.BATTLE.GetOtherPlayer(player).gameObject)) {
            if (player.AttackPower - Constants.BATTLE.GetOtherPlayer(player).Defence > 0 && (player.AttackPower - Constants.BATTLE.GetOtherPlayer(player).Defence) * AIConstants.LIFE_POINT > point) {
                int p = (player.AttackPower - Constants.BATTLE.GetOtherPlayer(player).Defence) * AIConstants.LIFE_POINT;
                for (int i = 0; i < attackCount; i++)
                    points.Add(p);

            }
        }
        points.Sort();
        points.Reverse();
        for (int i = 0; i < attackCount && i < points.Count; i++)
            point += points[i];

        return point;
    }

    public override bool ActionEnter() {
        player.Attack(target);
        return true;
    }

    public override string GetActionName()
    {
        return base.GetActionName() + ":" + target.name + "への攻撃";
    }
}
