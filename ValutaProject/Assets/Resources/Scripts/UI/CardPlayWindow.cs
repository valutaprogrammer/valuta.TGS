using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPlayWindow : MonoBehaviour {
    public static CardPlayWindow CPW;

    [SerializeField]
    private Text text;
    [SerializeField]
    private GameObject Window;

    private void Awake() {
        if (CPW == null) CPW = this;
        else Destroy(this);
        Window.SetActive(false);
    }

    /// <summary>
    /// ウィンドウの呼び出し
    /// </summary>
    public IEnumerator CallWindow(string text,List<Card> targets,CallBack _call) {

        ChengeText(text);

        Window.SetActive(true);

        //選択されるまで待機
        while (Window.activeSelf) {
            Debug.Log("発動確認中…");
            yield return null;
        }

        if (button)
            yield return StartCoroutine(CardSelectManager.CS.CallCardSelect(targets, _call));

        //yield return StartCoroutine(_call(true));
    }

    private void ChengeText(string t) {
        text.text = t;
    }

    private bool button = false;
    public void ButtonPush(bool button) {
        if (button) {
            Debug.Log("カード効果発動が認可されました");
        }
        else {
            Debug.Log("カード効果発動が拒否されました");
        }

        this.button = button;

        Window.SetActive(false);
    }
}
