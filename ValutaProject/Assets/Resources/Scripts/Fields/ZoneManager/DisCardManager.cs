using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//捨て札ゾーンClass
public class DisCardManager : CardZone{
    public override CardZoneType GetCardZoneType()
    {
        return CardZoneType.Dest;
    }
}
