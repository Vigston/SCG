﻿using UnityEngine;
using battleTypes;

public class Test_User : MonoBehaviour
{
    [SerializeField]
    private ResourceMgr resourceMgr; // ユーザーごとの資源管理
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

    [SerializeField, ReadOnly]
    PhaseType m_PhaseType;

	[SerializeField, ReadOnly]
	// フェイズ同期待ちフラグ
	bool m_PhaseReadyFlag;

    // 対戦開始しているか
    [SerializeField, ReadOnly]
    bool m_GameStartFlag;

	private void Awake()
	{
		// 資源管理の初期化
		resourceMgr = new ResourceMgr();
	}

	// フェイズ情報の初期化
	public void Init_PhaseInfo()
    {
		GetSetPhaseReadyFlag = false;
    }

	// 資源管理へのアクセス
	public ResourceMgr GetResourceMgr()
	{
		return resourceMgr;
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

    public PhaseType GetSetPhaseType
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

	// --対戦開始しているか--
	public bool GetSetGameStartFlag
	{
		get { return m_GameStartFlag; }
		set { m_GameStartFlag = value; }
	}
}
