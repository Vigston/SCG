using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleArea : MonoBehaviour
{
    [SerializeField]
    Side m_Side;

    /////////関数//////////
    // ===側===
    // 側設定
    public void SetSide(Side _side)
    {
        m_Side = _side;
    }
    // 側取得
    public Side GetSide()
    {
        return m_Side;
    }
}
