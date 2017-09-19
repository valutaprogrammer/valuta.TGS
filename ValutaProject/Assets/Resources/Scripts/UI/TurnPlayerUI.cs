using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPlayerUI : MonoBehaviour {
    public Text pText;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update () {
        Player p = Constants.BATTLE.GetActivePlayer();
        if (p) {
            pText.text = p.cardAligment.ToString();
            pText.gameObject.SetActive(true);
            image.color = new Color(image.color.a, image.color.b, image.color.g, 175.0f);
        }
        else {
            pText.text = "";
            pText.gameObject.SetActive(false);
            image.color = new Color(image.color.a, image.color.b, image.color.g, 0.0f);
        }
	}
}
