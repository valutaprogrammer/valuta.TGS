using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardListUI : MonoBehaviour {

    [SerializeField]
    private Text pageText;
    [SerializeField]
    private Text effectText;

    private void Update() {
        pageText.text = (CardListSceneManager.LIST.page + 1) + " / " + (CardListSceneManager.LIST.pageMax + 1);
        if (CardListSceneManager.LIST.SelectCard != null && CardListSceneManager.LIST.SelectCard.GetComponent<Card>())
            effectText.text = CardListSceneManager.LIST.SelectCard.GetComponent<Card>().State.text;
    }
}
