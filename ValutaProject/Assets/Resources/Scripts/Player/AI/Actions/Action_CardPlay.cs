using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CardPlay : Action_Card
{
    public Action_CardPlay(Player player, Card card,GameObject target = null) : base(player, card)
    {
        this.target = target;
    }

    public GameObject target;

    public override bool PointUpdate(AIScript.PlayerState[] PlayerStates)
    {
        return base.PointUpdate(PlayerStates);
    }

    public override bool ActionEnter()
    {
        if (card.State.cardType == ObjectType.Unit) {
            if (!player.GetUnitZone().isAddCard()) {
                AIScript.PlayerState state = player.GetComponent<AIScript>().PlayerStates[0];
                CardPoint dispCard = null;
                foreach (CardPoint unitCard in state.Unit)
                    if (point > unitCard.point && (dispCard == null || dispCard.point > unitCard.point))
                        dispCard = unitCard;
                if (dispCard != null)
                    dispCard.card.Disp();
            }
        }
        else if (card.State.cardType == ObjectType.Support || card.State.cardType == ObjectType.Trap) {
            if (!player.GetSupportZone().isAddCard()) {
                AIScript.PlayerState state = player.GetComponent<AIScript>().PlayerStates[0];
                CardPoint dispCard = null;
                foreach (CardPoint supportCard in state.Support)
                    if (point > supportCard.point && (dispCard == null || dispCard.point > supportCard.point))
                        dispCard = supportCard;
                if (dispCard != null)
                    dispCard.card.Disp();
            }
        }

        if (card.State.isTargetSelect()) {
            player.GetComponent<AIScript>().GetPoint(player, card, CardZoneType.Hand, ref target);
            Debug.Log(card.State.cardName + " 行動対象 " + target);
        }
            
        return player.CardPlay(card, target);
    }

    public override string GetActionName()
    {
        return base.GetActionName() + "使用" + (target == null ? "" : "：対象は" +target.name);
    }
}
