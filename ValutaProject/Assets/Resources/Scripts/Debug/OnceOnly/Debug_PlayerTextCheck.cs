using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_PlayerTextCheck : MonoBehaviour {
    [SerializeField]
    private Text text;
    [SerializeField]
    private Player target;
	
	void Update () {
        if (text != null && target != null)
            text.text = target.GetStatesText();
	}
}
