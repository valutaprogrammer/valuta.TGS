using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour {
    [SerializeField]
    private Text text;

    public static void DamageInit(int damage,GameObject target,float t = 0.5f) {
        GameObject d = Instantiate(Resources.Load("Prefabs/DamageEffect")) as GameObject;
        d.GetComponent<DamageEffect>().Init(damage, target, t);
    }

    void Init(int damage,GameObject target,float t = 1.5f) {
        if (damage >= 0)
        {
            text.color = Color.red;
            text.text = damage.ToString();
        } //(Color.red + Color.black) / 2;
        else {
            text.color = Color.green;
            text.text = (-damage).ToString();
        } 

        text.transform.SetParent(target.transform);
        text.transform.position = target.transform.position;


        /*
        if (damage > 0)
            StartCoroutine(Vive(t));
        else StartCoroutine(Vive(t, 0));*/
        StartCoroutine(Move(t));
    }

    Vector2 move = new Vector2(0, 50);
    IEnumerator Move(float t) {
        float nowT = 0;
        Vector2 pos = transform.position;
        while (nowT < 0.1f){
            nowT += Time.deltaTime;
            transform.position = pos + (move * (nowT / 0.1f));
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        int tenmetu = 5;
        while (tenmetu-- > 0) {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        //yield return new WaitForSeconds(t);

        Destroy(gameObject);
    }

    IEnumerator Vive(float t,float vive = 8.5f) {
        float nowT = 0;
        Vector2 pos = text.transform.position;
        while (nowT < t) {
            nowT += Time.deltaTime;
            text.transform.position = pos + new Vector2(GetRandVive(vive), GetRandVive(vive));
            yield return null;
        }
        nowT = 0;
        while (nowT < 0.5f) {
            nowT += Time.deltaTime;
            text.transform.position = pos;
            yield return null;
        }
        nowT = 0;
        Vector3 downPos = pos;
        Color color = text.color;
        while (nowT < 1) {
            nowT += Time.deltaTime;
            downPos.y -= down * (Time.deltaTime);
            text.transform.position = downPos;
            color.a -= Time.deltaTime;
            text.color = color;
            yield return null;
        }
        Destroy(text.gameObject);
        Destroy(this);
    }

    private const float down = 10.0f;
    private float GetRandVive(float vive) {
        return vive * Random.Range(-1.0f, 1.0f);
    }
}
