using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectTextBox : MonoBehaviour {
    public enum BoxPos {
        Up,
        Down,
    }
    public BoxPos pos;
    public Text cardName, effectText;
    const float t = 3.0f;

    void Awake() {
        if (pos == BoxPos.Up)
            Constants.TextBox_Up = this;
        else Constants.TextBox_Down = this;
        gameObject.SetActive(false);
    }

    public IEnumerator SetEffectText(string name,string text,float time = t) {
        cardName.text = name;
        effectText.text = text;
        gameObject.SetActive(true);

        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
