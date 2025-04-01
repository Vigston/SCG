using UnityEngine;
using battleTypes;
using System;

public class Test_User : MonoBehaviour
{
    // ===変数===
    // 側
    [SerializeField]
    Side m_Side;
	// ネットワーク固有番号
	[SerializeField, ReadOnly]
	int m_ID;

	[SerializeField, ReadOnly]
	// 選択したカードエリア
	CardArea m_SelectedCardArea;

	// フェイズ情報
	[SerializeField, ReadOnly]
	PhaseManager.PhaseType m_PhaseType;
	// フェイズ同期待ちフラグ
	[SerializeField, ReadOnly]
	bool m_PhaseReadyFlag;

	// ステート情報
	[SerializeField, ReadOnly]
	Enum m_PhaseState;
	// ステート同期待ちフラグ
	[SerializeField, ReadOnly]
	bool m_PhaseStateReadyFlag;

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

    // --ユーザーID--
    public int GetSetID
    {
        get { return m_ID; }
        set { m_ID = value; }
    }

    // 選択したカードエリア
    public CardArea GetSetSelectedCardArea
    {
        get { return m_SelectedCardArea; }
        set { m_SelectedCardArea = value; }
    }

	// フェイズ情報取得
    public PhaseManager.PhaseType GetSetPhaseType
    {
        get { return m_PhaseType; }
        set { m_PhaseType = value; }
    }

	// --フェイズ同期待ち--
	public bool GetSetPhaseReadyFlag
    {
        get { return m_PhaseReadyFlag; }
        set { m_PhaseReadyFlag = value; }
    }

	// ステート情報取得
	public Enum GetSetPhaseState
	{
		get { return m_PhaseState; }
		set { m_PhaseState = value; }
	}

	// --ステート同期待ち--
	public bool GetSetPhaseStateReadyFlag
	{
		get { return m_PhaseStateReadyFlag; }
		set { m_PhaseStateReadyFlag = value; }
	}
}
