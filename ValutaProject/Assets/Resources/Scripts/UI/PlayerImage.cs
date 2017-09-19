using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerImage : MonoBehaviour {
    [SerializeField]
    Player player;
    [SerializeField]
    Image image;
    [SerializeField]
    Text text;

    private CardAligment oldAl;

    void Awake() {
        image.color = new Color(0, 0, 0, 0);
    }

    void Update() {
        if(player.cardAligment != oldAl) {
            image.color = new Color(1, 1, 1, 1);
            oldAl = player.cardAligment;
            if (oldAl == CardAligment.Magician)
            {
                image.sprite = Resources.Load<Sprite>(Constants.PLAYERSTATE_ALIGMENT_MAGICK);
                text.text = Constants.M_NAME;
            }
            else if (oldAl == CardAligment.SowrdMan) {
                image.sprite = Resources.Load<Sprite>(Constants.PLAYERSTATE_ALIGMENT_SOWRD);
                text.text = Constants.S_NAME;
            }
        }
    }
}
