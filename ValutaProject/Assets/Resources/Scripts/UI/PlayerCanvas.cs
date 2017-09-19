using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour {
    [SerializeField]
    List<Player> players;
    [SerializeField]
    List<Canvas> pCanvases;

    [SerializeField]
    private int activeCanvasNum;
    [SerializeField]
    private int noneActiveCanvasNum;

    Player oldActive;
	void Update () {
        if (oldActive != Constants.BATTLE.GetActivePlayer()) {
            for (int i = 0; i < players.Count && i < pCanvases.Count; i++) {
                if (players[i] == Constants.BATTLE.GetActivePlayer()) pCanvases[i].sortingOrder = activeCanvasNum;
                else pCanvases[i].sortingOrder = noneActiveCanvasNum;
            }
            oldActive = Constants.BATTLE.GetActivePlayer();
        }
	}
}
