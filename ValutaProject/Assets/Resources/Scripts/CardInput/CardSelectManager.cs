using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerator CallBack(Card card);
public class CardSelectManager : MonoBehaviour {
    public static CardSelectManager CS;
    [SerializeField]
    private GameObject CancelButton;

    private void Awake() {
        if (CS == null) CS = this;
        else Destroy(this);

        CancelButton.SetActive(false);
    }

    private Card card;
    public static bool isWaiting = false;
    public IEnumerator CallCardSelect(List<Card> cards,CallBack _call) {
        isWaiting = true;
        if (cards == null || cards.Count == 0) yield break;

        foreach (Card c in cards) {
            //各対象カードに選択対象エフェクトをアタッチする
            c.gameObject.AddComponent<CardSelect>();
        }

        SelectStart();

        while (isWaiting)
            yield return null;

        foreach (Card c in cards) {
            //各対象カードにアタッチした選択対象エフェクトを削除する
            //Destroy(c.gameObject.AddComponent<CardSelect>()); 
            c.GetComponent<CardSelect>().SelectEnd();
        }

        SelectEnd();

        yield return StartCoroutine(_call(card));
    }

    private void SelectStart() {
        //キャンセルボタンを表示
        try {
            CancelButton.SetActive(true);
        }
        catch {
            Debug.Log("CancelButtonがありません");
        }
    }

    private void SelectEnd() {
        //キャンセルボタンを非表示
        try
        {
            CancelButton.SetActive(false);
        }
        catch
        {
            Debug.Log("CancelButtonがありません");
        }
    }

    public void SetSelectCard(Card card) {
        isWaiting = false;
        this.card = card;
    }

    public void OnCancelButton() {
        SetSelectCard(null);
    }
}
