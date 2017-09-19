using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLayer : MonoBehaviour {
    [SerializeField]
    public int layer;

	// Update is called once per frame
	void Update () {
		if (transform.GetSiblingIndex() != layer)
			transform.SetSiblingIndex (layer);
	}
}
