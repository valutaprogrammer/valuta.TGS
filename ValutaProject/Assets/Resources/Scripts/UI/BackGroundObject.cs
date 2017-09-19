using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundObject : MonoBehaviour {
    [SerializeField]
    private Transform parent;

	// Use this for initialization
	void Start () {
        if (parent == null) Destroy(this);
	}

    private bool isNoneActive;
	void Update () {
        if (!parent.gameObject.activeSelf) {
            gameObject.transform.SetParent(parent);
            isNoneActive = true;
            return;
        }

        if (isNoneActive) gameObject.transform.SetParent(parent.parent);
    }

    void LateUpdate() {
        transform.SetSiblingIndex(parent.GetSiblingIndex() - 1);
    }
}
