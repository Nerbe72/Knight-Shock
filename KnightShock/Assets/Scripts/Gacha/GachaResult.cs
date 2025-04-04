using NUnit.Framework;
using System;
using System.Collections.Generic;

[Serializable]
public class GachaResult
{
    public int CharacterId;
    public int TotalCount;
    /// <summary>
    /// SR등장 이후 쌓인 스택
    /// </summary>
    public int SRCurrentCount;
    /// <summary>
    /// SSR등장 이후 쌓인 스택
    /// </summary>
    public int SSRCurrentCount;
    /// <summary>
    /// 천장 체크
    /// </summary>
    public bool PickupForce;
    public string Time;

    public GachaResult()
    {
        CharacterId = 0;
        TotalCount = 0;
        SRCurrentCount = 0;
        SSRCurrentCount = 0;
        PickupForce = false;
        Time = "";
    }

    public GachaResult(int _characterId, int _totalCount, int _sRCurrentCount, int _sSRCurrentCount, bool _pickupForce, string _time)
    {
        CharacterId = _characterId;
        TotalCount = _totalCount;
        SRCurrentCount = _sRCurrentCount;
        SSRCurrentCount = _sSRCurrentCount;
        PickupForce = _pickupForce;
        Time = _time;
    }
}

[Serializable]
public class GachaResultWrapper
{
    public List<GachaResult> GachaResultList;

    public GachaResultWrapper()
    {
        GachaResultList = new List<GachaResult>();
    }

    public GachaResultWrapper(List<GachaResult> _list)
    {
        GachaResultList = _list;
    }
}
