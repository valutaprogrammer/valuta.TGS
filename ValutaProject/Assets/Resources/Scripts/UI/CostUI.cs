using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostUI : MonoBehaviour
{
    private float dilay = 0.1f;
    //private float _dilay = 0;

    public Text costText;
    private CardAligment aligment;
    private Image parentImage;
    public static GameObject UpEffect, DownEffect;
    public GameObject effectPos;

    void Awake() {
        try {
            parentImage = transform.parent.GetComponent<Image>();
            if (parentImage == null) Destroy(this);
            gameObject.transform.parent.gameObject.SetActive(false);
        }
        catch {
            Destroy(this);
        }
    }


    public void TextUpdate(Player p,bool isInit = false) {
        if (!UpEffect || !DownEffect) {
            UpEffect = Resources.Load<GameObject>(Constants.CARD_EFFECT_STATE_UP);
            DownEffect = Resources.Load<GameObject>(Constants.CARD_EFFECT_STATE_DOWN);
        }

        //Debug.Log("costUpdate");
        switch (p.cardAligment) {
            case CardAligment.SowrdMan:
                gameObject.transform.parent.gameObject.SetActive(true);
                parentImage.sprite = Resources.Load<Sprite>(Constants.COST_ALIGMENT_SOWRD);
                parentImage.SetNativeSize();
                break;
            case CardAligment.Magician:
                gameObject.transform.parent.gameObject.SetActive(true);
                parentImage.sprite = Resources.Load<Sprite>(Constants.COST_ALIGMENT_MAGICK);
                parentImage.SetNativeSize();
                break;
            default:
                gameObject.transform.parent.gameObject.SetActive(false);
                break;
        }

        if (dilaydUpdate != null) StopCoroutine(dilaydUpdate);
        int num = 0;
        int.TryParse(costText.text,out num);
        if (num < p.cost)
        {
            dilaydUpdate = StartCoroutine(DilaydUpdate(p.cost));
            if (!isInit) StartCoroutine(Constants.ANIMATION_EFFECT.EffectStart(UpEffect, effectPos));
        }
        else if(num > p.cost) {
            if (!isInit) StartCoroutine(Constants.ANIMATION_EFFECT.EffectStart(DownEffect, effectPos));
            costText.text = p.cost.ToString();
        }
        isInit = false;
    }

    Coroutine dilaydUpdate = null;
    private IEnumerator DilaydUpdate(int value) {
        int now;
        int.TryParse(costText.text,out now);
        while (now < value) {
            costText.text = (++now).ToString();
            yield return new WaitForSeconds(dilay);
        }
    }
}
