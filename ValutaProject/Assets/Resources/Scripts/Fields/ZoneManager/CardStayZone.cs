using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStayZone : CardZone {
    public virtual int GetAtack() {
        int atk = 0;
        List<Card> cards = GetCards();
        foreach (Card card in cards)
            atk += card.State.atk;

        return atk;
    }

    public virtual string GetAtkText() {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (Card c in Cards)
            if (c.State.atk != 0)
                sb.Append(c.State.cardName + "：攻撃力" + (c.State.atk > 0 ? "+" : "")  + c.State.atk);

        return sb.ToString();
    }

    public virtual string GetSkillText() {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (Card c in Cards)
            if (c.State.skill != 0)
                sb.Append(c.State.cardName + "：攻撃力" + (c.State.skill > 0 ? "+" : "") + c.State.skill);

        return sb.ToString();
    }

    public virtual int GetSkill() {
        int skill = 0;
        List<Card> cards = GetCards();
        foreach (Card card in cards)
            skill += card.State.skill;

        return skill;
    }
}
