using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIConstants {
    public const int CARD_POINT_REVERS = 3;//裏側のカード（不明なカード）の評価値
    public const int COST_POINT = 1;//１コストごとの評価値
    public const int ATTACK_POINT = 3;//1攻撃力ごとの評価値
    public const int SKILL_POINT = 3;//1スキルダメージごとの評価値
    public const int LIFE_POINT = 3;//1ライフごとの評価値
}
