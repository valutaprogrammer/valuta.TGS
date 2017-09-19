using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource getDamage;
    public AudioSource attack_Sowrd, attack_Magick;
    public AudioSource card_Move;
    public AudioSource normalBGM, WinBGM, loseBGM;

    public List<AudioSource> SE = new List<AudioSource>();
    public AudioSource BGM;

    void Awake() {
        Constants.Sound = this;
    }

    void Start() {
        //CallSE(attack_Magick);
    }

    void Update() {
        for (int i = 0; i < SE.Count; i++) {
            if (SE[i] == null || !SE[i].isPlaying) {
                Destroy(SE[i]);
                SE.Remove(SE[i--]);
            } 
        }
    }

    public AudioSource CallSE(AudioSource se) {
        se.gameObject.SetActive(true);
        if (!se.isPlaying)
            se.Play();
        else {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = se.clip;
            audio.volume = se.volume;
            audio.pitch = se.pitch;
            se = audio;
            SE.Add(se);
            audio.Play();
        }

        if (se.pitch < 0) {
            se.timeSamples = se.clip.samples - 1;
        }
        else se.timeSamples = 0;

        return se;
    }

    public void ChangeBGM(AudioSource bgm) {
        BGM.Stop();
        BGM = bgm;
        BGM.Play();
    }
}
