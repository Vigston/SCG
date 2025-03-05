using UnityEngine;
using battleTypes;

[System.Serializable]
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

	[SerializeField, ReadOnly]
	// フェイズ同期待ちフラグ
	bool m_PhaseReadyFlag;

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

    // --フェイズ同期待ち--
    public bool GetSetPhaseReadyFlag
    {
        get { return m_PhaseReadyFlag; }
        set { m_PhaseReadyFlag = value; }
    }
}
