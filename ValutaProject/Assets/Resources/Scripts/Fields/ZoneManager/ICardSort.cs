using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSort {
    //ソート後の位置を計算
    List<Vector2> GetCardSort(List<Card> card);

    //引数のカードをソートする
    void CardSort(List<Card> card);

    //この場のカードをソートする
    void SortUpdate();
}
