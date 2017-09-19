using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftButton : MonoBehaviour {
    [SerializeField]
    private Vector2 defPos;
    [SerializeField]
    List<Vector3> movePos;
    [SerializeField]
    public static float moveTime;
    [SerializeField]
    private int moveCount = 1;
    [SerializeField]
    GameObject draft;

    public static DraftButton dButton;

    void Awake() {
        dButton = this;
        //OnClick(true, 0);
        StartCoroutine(Move(defPos, 0));
    }

    public void OnClick() {
        OnClick(null);
    }

    public void OnClick(float? t = null) {
        if (!t.HasValue) t = moveTime;
        if (corMove != null) StopCoroutine(corMove);
        if (moveCount++ % 2 == 0)
        {
            for (int i = 0; i < Constants.BATTLE.Players.Length; i++)
                if (Constants.BATTLE.Players[i] == Constants.BATTLE.GetActivePlayer()) {
                    if (t <= 0)
                        draft.transform.localPosition = movePos[i];
                    else corMove = StartCoroutine(Move(movePos[i], t.Value));
                }
        }
        else if (t <= 0) draft.transform.localPosition = defPos;
        else corMove = StartCoroutine(Move(defPos, t.Value));
    }

    //引数のboolで表示・非表示切替
    public void OnClick(bool isActive, float? t = null) {
        if (isActive == (moveCount % 2 == 0)){
            if (!t .HasValue) t = moveTime;
            OnClick(t.Value);
        }
    }

    Coroutine corMove;
    private IEnumerator Move(Vector3 pos,float t) {
        Vector3 nowPos = draft.transform.localPosition;
        Vector3 dis = pos - nowPos;
        float nowT = 0;
        while (nowT < t) {
            nowT += Time.deltaTime;
            draft.transform.localPosition = nowPos + (dis * (nowT / t));
            yield return null;
        }
        draft.transform.localPosition = pos;
    }

}
