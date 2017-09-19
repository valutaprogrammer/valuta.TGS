using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AligmentButton : MonoBehaviour {
    [SerializeField]
    private Button sowd_Button;
    [SerializeField]
    private Button magick_Button;

    public static AligmentButton aligment;

    void Awake() {
        aligment = this;
        gameObject.SetActive(false);
        try {
            sowd_Button.GetComponent<Image>().sprite = Resources.Load<Sprite>(Constants.PLAYER_ALIGMENT_SOWRD);
            magick_Button.GetComponent<Image>().sprite = Resources.Load<Sprite>(Constants.PLAYER_ALIGMENT_MAGICK);
            sowd_Button.transform.FindChild("Text").GetComponent<Text>().text = Constants.S_NAME;
            magick_Button.transform.FindChild("Text").GetComponent<Text>().text = Constants.M_NAME;
        } catch {
            Debug.LogAssertion("陣営選択ボタンの表示初期化に失敗しました");
            //Debug.Log("陣営選択ボタンの表示初期化に失敗しました");
        }
    }

    //剣士・太陽・契約者の陣営
    public void S_Button() {
        Constants.BATTLE.AligmentEnter(CardAligment.SowrdMan);
    }

    //魔術師・月の陣営
    public void M_Button() {
        Constants.BATTLE.AligmentEnter(CardAligment.Magician);
    }

    public void SelectStart() {
        gameObject.SetActive(true);
    }

    public void AligmentEnter() {
        gameObject.SetActive(false);
    }
}
