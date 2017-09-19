using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour {
    [SerializeField]
    //詳細ボタン
    private GameObject dataButton;

    [SerializeField]
    //効果起動ボタン
    private GameObject effectButton;

    [SerializeField]
    //撤退ボタン
    private GameObject dispButton;

    private Card card;

    void Awake() {
        Constants.CARD_BUTTONS = this;
        gameObject.SetActive(false);
        dataButton.SetActive(false);
        effectButton.SetActive(false);
        dispButton.SetActive(false);
    }

    private void Update()
    {
        if (card == null) gameObject.SetActive(false);
        transform.position = card.gameObject.transform.position;
    }

    /// <summary>
    /// カードがクリックされた際に呼び出される、ボタンを表示する処理
    /// </summary>
    public void CardClick(Card card) {
        CardRemove();
        /*gameObject.SetActive(true);
        gameObject.transform.position = card.transform.position;
        dataButton.SetActive(true);*/
        this.card = card;
        effectButton.SetActive(false);
        dispButton.SetActive(false);
        if (Constants.BATTLE.FindCardHolder(card) != null && Constants.BATTLE.FindCardHolder(card).isAI){

        }
        else {
            effectButton.SetActive(card.isActiveEffectPlay());
            CardZone zone = Constants.BATTLE.FindCardStayZone(card);
            if (zone != null) {
                System.Type type = zone.GetType();
                if (type == typeof(SupportManager) || type == typeof(UnitManager))
                    dispButton.SetActive(true);
            }
        }
        DataButtonClick();
    }
    
    public void CardRemove() {
        gameObject.SetActive(false);
        //dataButton.SetActive(false);
        //effectButton.SetActive(false);
        //dispButton.SetActive(false);
        Constants.CARD_DATA_UI.DataNoneActive();
        card = null;
    }

    public void DataButtonClick() {
        //詳細画面表示
        Constants.CARD_DATA_UI.DataActive(card);
        //CardRemove();
    }

    public void DispButtonClick() {
        //カード撤退
        if (card != null) card.Disp();
        CardRemove();
    }

    public void EffectButtonClick() {
        //起動効果発動
        card.ActiveEffectPlay();
        CardRemove();
    }
}
