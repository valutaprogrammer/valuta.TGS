using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTosButton : MonoBehaviour {
    public static CoinTosButton CTos;
    [SerializeField]
    int shuffleCount;
    [SerializeField]
    float shuffleTime;
    [SerializeField]
    float waitTime;

    [SerializeField]
    public GameObject[] Buttons,ButtonsBack;

    private bool isSelectTime = false;
    
    private void Awake()
    {
        CTos = this;
        gameObject.SetActive(false);
        foreach (GameObject back in ButtonsBack)
            back.SetActive(false);
    }

    public void Shuffle() {
        StartCoroutine(ShuffleEnum());
        MessageUI.Message.AddMessage("", 0);
    }

    private IEnumerator ShuffleEnum() {
        yield return new WaitForSeconds(waitTime);
        foreach (GameObject back in ButtonsBack)
            back.SetActive(true);

        yield return new WaitForSeconds(waitTime / 2);

        Vector3 aButton = Buttons[0].transform.position;
        Vector3 bButton = Buttons[1].transform.position;

        int count = 0;
        while (count < shuffleCount) {
            Constants.Sound.CallSE(Constants.Sound.card_Move);
            StartCoroutine(Move(Buttons[0], Buttons[1].transform.position, shuffleTime));
            StartCoroutine(Move(Buttons[1], Buttons[0].transform.position, shuffleTime));
            yield return new WaitForSeconds(shuffleTime);
            count++;
        }
        yield return null;
        isSelectTime = true;

        int rand = Random.Range(0, 2);
        if (rand == 0) {
            Vector3 pos = Buttons[0].transform.position;
            Buttons[0].transform.position = Buttons[1].transform.position;
            Buttons[1].transform.position = pos;
        }

        MessageUI.Message.AddMessage("どちらかを選択してください", 1.5f);
    }

    private IEnumerator Move(GameObject obj,Vector3 position,float time) {
        float t = 0;

        Vector3 defPos = obj.transform.position;
        Vector3 moveDistance = defPos - position;
        while (t < time) {
            yield return null;
            t += Time.deltaTime;
            obj.transform.position = defPos - (moveDistance * t / time);
        }

        obj.transform.position = position;
    }

    bool isPushed = false;
    public void Onclick(bool isFirst) {
        if (isPushed) return;
        if (!isSelectTime) return;
        isPushed = true;
        foreach (GameObject back in ButtonsBack)
            back.SetActive(false);
        Player fP = isFirst ? Constants.BATTLE.Players[0] : Constants.BATTLE.Players[1];
        //Constants.BATTLE.CoinTos(fP);
        GameObject obj;
        if (isFirst) {
            obj = Buttons[0];
            Buttons[1].SetActive(false);
        }
        else {
            obj = Buttons[1];
            Buttons[0].SetActive(false);
        }
        Vector2 pos = (Buttons[0].transform.position  + Buttons[1].transform.position) / 2;
        StartCoroutine(ClickEnum(fP, pos, obj));
    }

    private IEnumerator ClickEnum(Player firstPlayer,Vector2 pos,GameObject obj) {
        yield return StartCoroutine(Move(obj, pos, waitTime / 2));
        yield return new WaitForSeconds(waitTime / 2);
        Constants.BATTLE.CoinTos(firstPlayer);
    }

    public void SelectStart() {
        gameObject.SetActive(true);
        Shuffle();
    }

    public void SelectEnd() {
        gameObject.SetActive(false);
    }
}
