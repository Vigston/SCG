﻿using UnityEngine;
using battleTypes;

public class Test_StageArea : MonoBehaviour
{
    [SerializeField]
    Side m_Side;

    /////////関数//////////
    // ===側===
    // 側設定
    public void SetSide(Side _Side)
    {
        m_Side = _Side;
    }
    // 側取得
    public Side GetSide()
    {
        return m_Side;
    }
}
