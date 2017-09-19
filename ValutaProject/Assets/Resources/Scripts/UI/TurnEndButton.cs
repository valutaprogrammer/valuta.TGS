using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnEndButton : MonoBehaviour {
    [SerializeField]
    private GameObject button;
    [SerializeField]
	private List<Image> image;

    private void Awake(){
        if (!button) Destroy(this);
    }

    public void OnClick() {
        Constants.BATTLE.TurnEnd();
        button.gameObject.SetActive(false);
    }

    private void Update() {
        AligmentUpdate();
        if (Constants.BATTLE.GetTurnPhase() == GamePhase.main && !Constants.BATTLE.GetActivePlayer().isAI && Constants.BATTLE.EventStack.Count == 0){
            button.gameObject.SetActive(true);
            if (Constants.BATTLE.GetActivePlayer() == Constants.BATTLE.Players[0]){
                image[0].gameObject.SetActive(true);
                image[1].gameObject.SetActive(false);
            }
            else {
                image[0].gameObject.SetActive(false);
                image[1].gameObject.SetActive(true);
            }
        }
        else{
            button.gameObject.SetActive(false);
            foreach (Image i in image)
                i.gameObject.SetActive(false);
        }
    }

    CardAligment p1;
    CardAligment p2;
    private void AligmentUpdate() {
        if (Constants.BATTLE.Players[0].cardAligment != p1 || Constants.BATTLE.Players[1].cardAligment != p2) {
            p1 = Constants.BATTLE.Players[0].cardAligment;
            p2 = Constants.BATTLE.Players[1].cardAligment;
            image[0].color = p1 == CardAligment.SowrdMan ? Constants.S_COLOR : Constants.M_COLOR;
            image[1].color = p2 == CardAligment.SowrdMan ? Constants.S_COLOR : Constants.M_COLOR;
        }
    }
}
