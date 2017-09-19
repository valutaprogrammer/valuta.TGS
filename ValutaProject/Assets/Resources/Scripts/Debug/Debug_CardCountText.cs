using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FixedLayer))]
[RequireComponent(typeof(Text))]
public class Debug_CardCountText : MonoBehaviour {
    [SerializeField]
    private CardZone cardZone;
    private Text text;
    void Awake() {
        if (cardZone == null) {
            cardZone = transform.parent.GetComponent<CardZone>();
            if (cardZone == null) Destroy(this);
        } 
        //最前面で固定する
        GetComponent<FixedLayer>().layer = 100;
        text = GetComponent<Text>();
    }

    //テキストの更新確認
    private int oldCards;
    void Update() {
        text.text = "";
        if (oldCards != cardZone.GetCardsCount()) {
            oldCards = cardZone.GetCardsCount();
            text.text = "";//oldCards.ToString();
        }
    }
}
