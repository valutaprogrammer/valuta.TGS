using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCardSpace : CardZone
{
    [SerializeField]
    List<Card> Dest;
    [SerializeField]
    GameObject defaultPage;
    GameObject[] Page = new GameObject[10];

    GameObject PageSet;
    GameObject CreateCard;

    DisCardManager DestZone;

    [SerializeField]
    public int x, y;//横数・縦数
    int page { get { return x * y; } }

    [SerializeField]
    Vector2 defaultsize,cardOffset;
    [SerializeField]
    public float leftSide = 0,UpSide = 0;
    int Open = 0;
    int ListPage = 0;
    int OpenPage = 0;
    // Use this for initialization
    void Start()
    {
        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        Vector2 scale = transform.localScale;
        defaultsize = new Vector2(size.x * scale.x, size.y * scale.y);
        if (x > 1) {
            leftSide = -(defaultsize.x / 2) + 50;
            cardOffset.x = (leftSide * -2) / (x - 1);
        }
        if (y > 1) {
            UpSide = defaultsize.y / 2 - 50;
            cardOffset.y = UpSide * 2 / (y - 1);
        }
        Dest = new List<Card>();
        gameObject.SetActive(false);
    }

    public void OpenCardList()
    {
        StartCardList();
    }

    void StartCardList()
    {
        Dest = Constants.BATTLE.Field.GetDisCardZone().GetCards();
        if (Dest.Count > 0) {
            gameObject.SetActive(true);
            CreatePage();
            CreateCardList();
        }
    }

    void CreateCardList()
    {
        ListPage = 0;
        float yPos,xPos = leftSide;

        for (int i = 0; i < Dest.Count; i++) {
            CreateCard = Instantiate(Dest[Open++].gameObject);
            CreateCard.transform.SetParent(Page[ListPage].transform);
            Vector2 size = CreateCard.GetComponent<RectTransform>().sizeDelta;
            Vector3 scale = CreateCard.transform.localScale;
            Vector2 CardSize = new Vector3(size.x * scale.x,size.y * scale.y);

            int line = i % page / y;//行数を確認
            xPos = leftSide + line * cardOffset.x;
            yPos = UpSide - (i % y) * cardOffset.y;

            CreateCard.transform.localPosition = new Vector2(xPos, yPos);
            if (Open % page == 0) ListPage++;
        }

        Page[0].SetActive(true);
    }
    public void NextList()
    {
        if (ListPage != OpenPage)
        {
            if (Open % 6 != 0)
            {
                OpenPage += 1;
                Page[OpenPage - 1].SetActive(false);
                Page[OpenPage].SetActive(true);
            }
        }
    }
    public void PrevList()
    {
        if (0 != OpenPage)
        {
            OpenPage -= 1;
            Page[OpenPage + 1].SetActive(false);
            Page[OpenPage].SetActive(true);
        }
    }
    void CreatePage()
    {
        for (int i = 0; i <= 9; i++)
        {
            PageSet = Instantiate(defaultPage);
            PageSet.transform.SetParent(gameObject.transform);
            PageSet.transform.localPosition = new Vector2(0, 0);
            PageSet.transform.localScale = new Vector2(1, 1);
            Page[i] = PageSet;
            Page[i].SetActive(false);
        }
    }
    public void PageClose()
    {
        for (int i = 0; i <= 9; i++)
        {
            Destroy(Page[i]);
        }
        ListPage = 0; OpenPage = 0; Open = 0;
        gameObject.SetActive(false);
    }
}
