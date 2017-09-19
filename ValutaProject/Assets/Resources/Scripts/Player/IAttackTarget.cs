using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackTarget {

    //被攻撃
    IEnumerator Damage(int damage);

    //被攻撃対象優先順位 高いほど高く、他より低い対象は選択できない
    int GetTargetPriority();
}
