using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CardInput))]
public class uicard : MonoBehaviour
{
    [SerializeField]
    private Image Frame_Image;
    [SerializeField]
    private Image Card_Image;
    [SerializeField]
    private Text CardNameText;
    [SerializeField]
    private Text CardCostText;
    [SerializeField]
    private Text CardEffectText;
    [SerializeField]
    private Text CardTypeText;
    [SerializeField]
    private Text CardDeffenceText;
    [SerializeField]
    private GameObject CardTexts;

    [SerializeField]
    private string cardText;

    [SerializeField]
    private Card card;

    private bool isSet;//現在伏せられているか否か

    public void Init()
    {
        card = GetComponent<Card>();
        if (!card) { return; }

        Card_Image.sprite = card.State.cardSprite;

        ChangeSprite();
        CardNameText.text = "";
        CardCostText.text = "";
        CardTypeText.text = "";
        CardDeffenceText.text = "";

        //Debug.Log(Card_Image.sprite);
        //枠付きカード画像を使用することになったので、画像が入っていたら帰る
        if (Card_Image.sprite != null && card.State.isSpriteOnFrame) return;

        CardNameText.text = card.State.cardName;
        CardCostText.text = card.State.Cost.ToString();
        if (CardEffectText)
            CardEffectText.text = cardText;
        CardTypeText.text = card.State.cardType.ToString();

        if (card.State.defence > 0)
            CardDeffenceText.text = card.State.defence.ToString();
        else CardDeffenceText.text = "";
    }

    /// <summary>
    /// 伏せる処理
    /// </summary>
    public void CardSet() {
        if (!isSet) {
            isSet = true;
            ChangeSprite();
            /////
            UpdateActive(isSet);
        }
    }

    /// <summary>
    /// 表向きにする処理
    /// </summary>
    public void CardOpen() {
        if (isSet) {
            isSet = false;
            ChangeSprite();
            UpdateActive(isSet);
        }
    }

    /// <summary>
    /// カードが裏になった時、テキスト等を消す 表になったら表示する
    /// </summary>
    /// <param name="value"></param>
    private void UpdateActive(bool value) {
        if (Card_Image.sprite != null && card.State.isSpriteOnFrame) return;
        value = !value;
        CardTexts.SetActive(value);
        /*
        CardNameText.gameObject.SetActive(!value);
        CardCostText.gameObject.SetActive(!value);
        if (CardEffectText)
            CardEffectText.gameObject.SetActive(!value);
        CardTypeText.gameObject.SetActive(!value);
        CardDeffenceText.gameObject.SetActive(!value);*/
    }

    /// <summary>
    /// 画像の更新
    /// </summary>
    private void ChangeSprite() {
        Sprite sprite = null;
        //カードが伏せられているなら裏面描画
        if (isSet) {
            sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_BACK);
        } 
        else if(Card_Image.sprite == null || !card.State.isSpriteOnFrame)//カード画像がない場合の描画
            switch (card.State.cardType)
            {
                case ObjectType.Unit://ユニットカードの画像に変更
                    if (card.State.cardAligment == CardAligment.SowrdMan)
                        sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_SWORD_UNIT);
                    else
                        sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_MAGICK_UNIT);
                    break;
                case ObjectType.Support:
                case ObjectType.Skill://非ユニット画像に変更
                    if (card.State.cardAligment == CardAligment.SowrdMan)
                        sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_SWORD);
                    else
                        sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_MAGICK);
                    break;
                case ObjectType.Trap://罠画像に変更
                    sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_TRAP);
                    break;
                case ObjectType.Life://L画像に変更
                    sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_LIFE);
                    break;
                default://何でもないなら裏面描画
                    sprite = Resources.Load<Sprite>(Constants.CARD_FRAME_BACK);
                    break;
            }
        Frame_Image.sprite = sprite;
        Color color = Frame_Image.color;
        if (Frame_Image.sprite) color.a = 1;
        else color.a = 0;
        Frame_Image.color = color;
    }

}



