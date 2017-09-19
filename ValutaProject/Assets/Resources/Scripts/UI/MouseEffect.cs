using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseEffect : MonoBehaviour {
    public GameObject Prefab;
    public Canvas TargetCanvas;
    private RectTransform effectObj;

    void Awake() {
        try
        {
            GameObject obj = Instantiate(Resources.Load("Prefabs/Effect" /*+ Prefab.name*/)) as GameObject;
            effectObj = obj.GetComponent<RectTransform>();
            effectObj.GetComponent<Image>().raycastTarget = false;
            effectObj.transform.SetParent(TargetCanvas.transform);
        }
        catch {
            //Debug.Log("マウスエフェクトのプレファブの参照に失敗しました");
            Destroy(this);
        }
    }

    private Vector3 oldPos;
	void Update () {
        effectObj.gameObject.SetActive(Input.GetMouseButton(0));
        if (effectObj.gameObject.activeSelf) {
            effectObj.position = oldPos;
        }
        else {
            effectObj.gameObject.SetActive(false);
        }

        oldPos = Input.mousePosition;
    }
}
