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
    // ---思考---
    // 指定側のユーザーがカードエリア選択思考を行っているか
    public bool IsThinkCardAreaSelectFromSide(Side _side)
    {
        BattleUser battleUser = GetUser(_side);

        return battleUser.IsSelectCardAreaFlag();
    }

    // ---ユーザー---
    // プレイヤーユーザー設定
    public void SetPlayerUser(BattleUser playerUser)
    {
        m_BattleUsers[(int)Side.eSide_Player] = playerUser;
    }
    // 敵ユーザー設定
    public void SetEnemyUser(BattleUser enemyUser)
    {
        m_BattleUsers[(int)Side.eSide_Enemy] = enemyUser;
    }
    // プレイヤーユーザー取得
    public BattleUser GetPlayerUser()
    {
        return m_BattleUsers[(int)Side.eSide_Player];
    }
    // 敵ユーザー取得
    public BattleUser GetEnemyUser()
    {
        return m_BattleUsers[(int)Side.eSide_Enemy];
    }
    // 指定側のユーザー取得
    public BattleUser GetUser(Side side)
    {
        return m_BattleUsers[(int)side];
    }

    // ---操作側---
    // 操作ユーザー側設定
    public void SetOperateUserSide(Side _side)
    {
        m_OperateUserSide = _side;
    }
    // 操作ユーザー側取得
    public Side GetOperateUserSide()
    {
        return m_OperateUserSide;
    }
    // 操作ユーザー側の切り替え
    public void ChangeOperateUserSide()
    {
        SetOperateUserSide(Common.GetRevSide(GetOperateUserSide()));
    }
    // 操作側のユーザーを取得
    public BattleUser GetOperateUser()
    {
        return m_BattleUsers[(int)m_OperateUserSide];
    }
}
