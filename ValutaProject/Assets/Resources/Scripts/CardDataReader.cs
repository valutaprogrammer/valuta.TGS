using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

enum dataType
{
    name,
    aligment,
    type,
    cost,
    text,
    deffence,
    count,
}

public static class CardDataReader {
    private const string DataPath = "Data/CardList";

    

    public static List<cardCount> ReadCardList() {
        TextAsset cardList = Resources.Load<TextAsset>(DataPath);

        StringReader sr = new StringReader(cardList.text);

        //1　カード名、2　所属、3　種類、4　マナ、5　能力、6　耐久、7　枚数
        //他に欲しいもの…　効果ID（効果量）　読み仮名　カード画像名
        List<string[]> cardData = new List<string[]>();
        //int i = 0;
        while (sr.Peek() > -1) {
            string[] line = sr.ReadLine().Split(',');
            for (int i = 0; i < line.Length; i++)
                line[i] = line[i].Replace("\"", "");
            cardData.Add(line);
        }
        cardData.Remove(cardData[0]);//一番上は表の目次なので削除

        List<cardCount> cards = new List<cardCount>();
        foreach (string[] line in cardData) {
            CardState state = new CardState();
            cardCount ccount = new cardCount();
            ccount.card = state;
            for (int i = 0; i < line.Length; i++) {
                if (line[i] == null || line[i] == "") continue;
                dataType dt = (dataType)Enum.ToObject(typeof(dataType),i);
                switch (dt) {
                    case dataType.name:
                        state.cardName = line[i];
                        break;
                    case dataType.aligment:
                        if (line[i].IndexOf("剣") >= 0) state.cardAligment = CardAligment.SowrdMan;
                        else if (line[i].IndexOf("契約") >= 0 || line[i].IndexOf("魔") >= 0) state.cardAligment = CardAligment.Magician;
                        else state.cardAligment = CardAligment.All;
                        break;
                    case dataType.type:
                        switch (line[i]) {
                            case "ユニット":
                                state.cardType = ObjectType.Unit;
                                break;
                            case "技":
                                state.cardType = ObjectType.Skill;
                                break;
                            case "支援":
                                state.cardType = ObjectType.Support;
                                break;
                            case "罠":
                                state.cardType = ObjectType.Trap;
                                break;
                            case "ライフ":
                                state.cardType = ObjectType.Life;
                                break;
                            default:
                                state.cardType = ObjectType.Skill;
                                break;
                        }
                        break;
                    case dataType.cost:
                        state.Cost = int.Parse(line[i]);
                        break;
                    case dataType.text:
                        state.text = line[i];
                        break;
                    case dataType.deffence:
                        state.defence = int.Parse(line[i]);
                        break;
                    case dataType.count:
                        ccount.count = int.Parse(line[i]);
                        break;
                }
            }
            cards.Add(ccount);
        }
        return cards;
    }
}
