using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class BattleUser : MonoBehaviour
{
    // ===変数===
    // 側
    [SerializeField, ReadOnly]
    private Side m_Side;
	// ネットワーク固有番号
	[SerializeField, ReadOnly]
	private int m_NetWorkActorNumber;

	// 選択したカードエリア
	CardArea m_SelectedCardArea;

	// --側--
	public Side GetSetSide
    {
        get { return m_Side; }
        set { m_Side = value; }
    }

    // --ネットワーク固有番号--
    public int GetSetNetWorkActorNumber
    {
        get { return m_NetWorkActorNumber; }
        set { m_NetWorkActorNumber = value; }
    }

    // 選択したカードエリア
    public CardArea GetSetSelectedCardArea
    {
        get { return m_SelectedCardArea; }
        set { m_SelectedCardArea = value; }
    }
}
