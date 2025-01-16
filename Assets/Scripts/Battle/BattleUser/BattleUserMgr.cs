using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleUserMgr : MonoBehaviour
{
    // =================変数=================
    // インスタンス
    public static BattleUserMgr instance;

    [SerializeField]
    private BattleUser[] m_BattleUsers = new BattleUser[(int)Side.eSide_Max];

    [SerializeField]
    private Side m_OperateUserSide;
    private void Awake()
    {
        // インスタンス生成
        CreateInstance();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // インスタンスを作成
    public bool CreateInstance()
    {
        // 既にインスタンスが作成されていなければ作成する
        if (instance == null)
        {
            // 作成
            instance = this;
        }

        // インスタンスが作成済みなら終了
        if (instance != null) { return true; }

        Debug.LogError("BattleUserMgrのインスタンスが生成できませんでした");
        return false;
    }

    // ===関数===
    // ---ユーザー---
    // プレイヤーユーザー
    public BattleUser GetSetPlayerUser
    {
        get { return m_BattleUsers[(int)Side.eSide_Player]; }
        set { m_BattleUsers[(int)Side.eSide_Player] = value; }
    }
	// 敵ユーザー
	public BattleUser GetSetEnemyUser
	{
		get { return m_BattleUsers[(int)Side.eSide_Enemy]; }
		set { m_BattleUsers[(int)Side.eSide_Enemy] = value; }
	}
    // 指定側のユーザー取得
    public BattleUser GetUser(Side Side)
    {
        return m_BattleUsers[(int)Side];
    }

    // ---操作側---
    // 操作ユーザー側
    public Side GetSetOperateSide
    {
        get { return m_OperateUserSide; }
        set { m_OperateUserSide = value; }
    }
    // 操作ユーザー側の切り替え
    public void ChangeOperateSide()
    {
		GetSetOperateSide = Common.GetRevSide(GetSetOperateSide);
    }
    // 操作側のユーザー
    public BattleUser GetSetOperateUser
    {
        get { return m_BattleUsers[(int)m_OperateUserSide]; }
        set { m_BattleUsers[(int)m_OperateUserSide] = value; }
    }
    // 指定の操作側か
    public bool IsOperateSide(Side _side)
    {
        return m_OperateUserSide == _side;
	}
}
