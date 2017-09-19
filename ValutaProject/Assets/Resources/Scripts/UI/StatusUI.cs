using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour {
    [SerializeField]
    private Player player;
    private int? life, atk, skill;
    private CardAligment aligment;

    [SerializeField]
    private Text atkText;
    public Transform atkEffectPos;

    [SerializeField]
    private Text skillText;
    public Transform skillEffectPos;

    [SerializeField]
    public Text lifeText_m, lifeText_s;
    private Text lifeText;

    public static GameObject UpEffect,DownEffect;

    void Start() {
        if (!UpEffect || !DownEffect) {
            UpEffect = Resources.Load<GameObject>(Constants.CARD_EFFECT_STATE_UP);
            DownEffect = Resources.Load<GameObject>(Constants.CARD_EFFECT_STATE_DOWN);
        }

        lifeText_s.text = "";
        lifeText_m.text = "";
        atkText.text = "";
        skillText.text = "";
    }
	
	void Update () {
        if (player.cardAligment == CardAligment.None || player.cardAligment == CardAligment.All) return;

        if (atk != player.AttackPower) {
            if (player.AttackPower > atk)
                StartCoroutine(Constants.ANIMATION_EFFECT.EffectStart(UpEffect, atkEffectPos.gameObject));
            else if(player.AttackPower < atk) StartCoroutine(Constants.ANIMATION_EFFECT.EffectStart(DownEffect, atkEffectPos.gameObject));
            atkText.text = (atk = player.AttackPower).ToString();
        }
        if (skill != player.SkillDamage) {
            if (player.SkillDamage > skill) StartCoroutine(Constants.ANIMATION_EFFECT.EffectStart(UpEffect, skillEffectPos.gameObject));
            else if(player.SkillDamage < skill) StartCoroutine(Constants.ANIMATION_EFFECT.EffectStart(DownEffect, skillEffectPos.gameObject));
            skillText.text = (skill = player.SkillDamage).ToString();
        }
        if (life != player.Life && lifeText != null) {
            lifeText.text = (life = player.GetLifeZone().GetCardsCount()).ToString();
        }

        if (player.cardAligment != aligment) {
            aligment = player.cardAligment;
            if (player.cardAligment == CardAligment.SowrdMan) {
                lifeText = lifeText_s;
                lifeText_m.text = "";
            }
            else if (player.cardAligment == CardAligment.Magician) {
                lifeText = lifeText_m;
                lifeText_s.text = "";
            }
        }
	}
}
