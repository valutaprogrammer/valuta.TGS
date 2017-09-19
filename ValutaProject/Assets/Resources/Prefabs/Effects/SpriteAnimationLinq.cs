using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnimationAutoDestroy))]
public class SpriteAnimationLinq : MonoBehaviour {
    private SpriteRenderer SR;
    private Image image;

    void Awake(){
        SR = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        if (SR == null || image == null) Destroy(this);
    }

	// Update is called once per frame
	void Update () {
        image.sprite = SR.sprite;
	}
}
