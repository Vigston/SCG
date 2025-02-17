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
    Side m_Side;
	// ネットワーク固有番号
	[SerializeField, ReadOnly]
	int m_NetWorkNumber;

	[SerializeField, ReadOnly]
	// 選択したカードエリア
	CardArea m_SelectedCardArea;

	[SerializeField, ReadOnly]
	// フェイズ同期待ちフラグ
	bool m_PhaseReadyFlag;

    // --初期化--
    // フェイズ情報の初期化
    public void Init_PhaseInfo()
    {
		GetSetPhaseReadyFlag = false;
    }

	// --側--
	public Side GetSetSide
    {
        get { return m_Side; }
        set { m_Side = value; }
    }

    // --ネットワーク固有番号--
    public int GetSetNetWorkNumber
    {
        get { return m_NetWorkNumber; }
        set { m_NetWorkNumber = value; }
    }

    // 選択したカードエリア
    public CardArea GetSetSelectedCardArea
    {
        get { return m_SelectedCardArea; }
        set { m_SelectedCardArea = value; }
    }

    // --フェイズ同期待ち--
    public bool GetSetPhaseReadyFlag
    {
        get { return m_PhaseReadyFlag; }
        set { m_PhaseReadyFlag = value; }
    }
}
