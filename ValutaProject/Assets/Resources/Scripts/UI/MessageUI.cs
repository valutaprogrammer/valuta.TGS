using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour {
    public static MessageUI Message;
    [SerializeField]
    private Text text;
    private Color tColor;
    [SerializeField]
    public Image messageImage,imageBack,back,back2;
    private Color bColor,b2Color;
    [SerializeField]
    public Sprite Yourturn, EnemyTurn,MainPhase,Win_S,Win_M,Los;

    private void Awake() {
        Message = this;
        //text.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        tColor = text.color;
        bColor = back.color;
        b2Color = back2.color;
        messageImage.gameObject.SetActive(false);
        imageBack.gameObject.SetActive(false);
    }


    public const float SpriteFadeTime = 1.0f;
    public void AddMessage(string message,float t = 0.5f) {
        text.text = message;
        if (CorFade != null) StopCoroutine(CorFade);
        CorFade = StartCoroutine(MessageFade(t));
    }

    public void AddSpriteFade(Sprite sprite,float t = 1.0f) {
        if (CorFade != null) StopCoroutine(CorFade);
        CorFade = StartCoroutine(SpriteFade(sprite, t));
    }

    const float fadeTime = 0.3f;//画像の表示・消去にかける時間
    IEnumerator SpriteFade(Sprite sprite,float t) {
        if (text.text == "")
            t = 0;
        back.gameObject.SetActive(false);
        imageBack.gameObject.SetActive(true);
        messageImage.gameObject.SetActive(true);
        messageImage.sprite = sprite;
        messageImage.SetNativeSize();
        float time = 0;
        Color color = messageImage.color;
        while (fadeTime >= time) {
            time += Time.deltaTime;
            color.a = (time / fadeTime);
            messageImage.color = color;
            yield return null;
        }
        time = 0;
        while (t >= time) {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        while (fadeTime >= time) {
            time += Time.deltaTime;
            color.a = 1 - (time / fadeTime);
            messageImage.color = color;
            yield return null;
        }
        imageBack.gameObject.SetActive(false);
        messageImage.gameObject.SetActive(false);
        CorFade = null;
    }

    public static Coroutine CorFade;
    IEnumerator MessageFade(float t) {
        imageBack.gameObject.SetActive(false);
        messageImage.gameObject.SetActive(false);
        text.color = tColor;
        back.gameObject.SetActive(true);
        back.color = bColor;
        back2.color = b2Color;
        float fadeOutTime = t / 4;
        while (t > fadeOutTime) {
            t -= Time.deltaTime;
            yield return null;
        }

        while (t > 0) {
            t -= Time.deltaTime;
            text.color = new Color(tColor.r, tColor.g, tColor.b,(t / fadeOutTime));
            back.color = new Color(bColor.r, bColor.g, bColor.b,(t / fadeOutTime));
            back2.color = new Color(b2Color.r, b2Color.g, b2Color.b, (t / fadeOutTime));
            yield return null;
        }

        text.color = tColor;
        back.color = bColor;
        back2.color = b2Color;
        back.gameObject.SetActive(false);
        CorFade = null;
    }
}
