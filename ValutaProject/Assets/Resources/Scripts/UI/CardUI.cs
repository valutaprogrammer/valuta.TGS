using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {
    private Card card;
    private Image image;
    private Text text;

    public void Init() {
        card = GetComponent<Card>();
        image = GetComponent<Image>();
        try { image.sprite = card.State.cardSprite; }
        catch { Debug.LogAssertion("カード画像を設定できません"); }
        try { text = transform.FindChild("Text").GetComponent<Text>(); }
        catch { Debug.LogAssertion(name + "の子要素にTextが存在しません"); }
        UIUpdate();
    }

    public void UIUpdate() {
        if (card && image && text) {
            image.color = GetCardTypeColor(card);
            text.text = card.State.cardType.ToString();

            if (card.GetIsSet()) text.gameObject.SetActive(false);
            else text.gameObject.SetActive(true);
        }
    }

    private Color GetCardTypeColor(Card card) {
        if (card == null) return DefaultColor;
        if (card.GetIsSet()) return SetColor;
        switch (card.State.cardType) {
            case ObjectType.Unit:return UnitColor;
            case ObjectType.Support:return SupportColor;
            case ObjectType.Trap:return TrapColor;
            case ObjectType.Skill:return MagickColor;
            case ObjectType.Life:return LifeColor;
            default: return DefaultColor;
        }
    }



    private Color UnitColor = Color.red;
    private Color SupportColor = Color.blue;
    private Color TrapColor = Color.yellow;
    private Color MagickColor = Color.cyan;
    private Color LifeColor = Color.green;
    private Color DefaultColor = Color.white;
    private Color SetColor = Color.black;
}
