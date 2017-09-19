using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDataUI : MonoBehaviour {
    [SerializeField]
    private Text dataTEXT;
    public Card card;
    private Vector2 size = new Vector2(1, 1);
    public Vector2 pos;

    void Awake() {
        Constants.CARD_DATA_UI = this;
        gameObject.SetActive(false);
    }

    //詳細表示
    public void DataActive(Card card) {
        if (this.card != null) DataNoneActive();
        switch (card.State.cardType) {
            case ObjectType.Unit:
                dataTEXT.text = "【ユニット】\n";
                break;
            case ObjectType.Support:
                dataTEXT.text = "【支援】\n";
                break;
            case ObjectType.Trap:
                dataTEXT.text = "【罠】\n";
                break;
            case ObjectType.Skill:
                dataTEXT.text = "【技】\n";
                break;
            case ObjectType.Life:
                dataTEXT.text = "【ライフ】\n";
                break;
        }
        bool set = card.GetIsSet();
        if (Constants.BATTLE.FindCardHolder(card) != null && !Constants.BATTLE.FindCardHolder(card).isAI) {
            card.SetIsSet(false);
        }
        this.card = Instantiate(card);
        if (Constants.BATTLE.FindCardHolder(card) != null && !Constants.BATTLE.FindCardHolder(card).isAI) {
            card.SetIsSet(set);
        }
        if (!set || (Constants.BATTLE.FindCardHolder(card) != null && !Constants.BATTLE.FindCardHolder(card).isAI))
            dataTEXT.text += card.State.text;
        this.card.gameObject.transform.SetParent(gameObject.transform);
        this.card.transform.localPosition = pos;
        this.card.GetComponent<EventTrigger>().enabled = false;
        //if (!Constants.BATTLE.FindCardHolder(card).isAI)
        /*Vector2 cSize = this.card.gameObject.transform.localScale;
        if (cSize.y < 0) cSize.y = -cSize.y;
        if (cSize.x < 0) cSize.x = -cSize.x;*/
        this.card.gameObject.transform.localScale = size;//new Vector2(size.x * cSize.x, size.y * cSize.y);
        gameObject.SetActive(true);
    }

    //詳細消去
    public void DataNoneActive() {
        gameObject.SetActive(false);
        if (card != null) Destroy(card.gameObject);
        dataTEXT.text = "";
    }

}
