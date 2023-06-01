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
    private BattleUser m_UserPlayer;
    [SerializeField]
    private BattleUser m_UserEnemy;

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
    // プレイヤーユーザー設定
    public void SetPlayerUser(BattleUser playerUser)
    {
        m_UserPlayer = playerUser;
    }
    // 敵ユーザー設定
    public void SetEnemyUser(BattleUser enemyUser)
    {
        m_UserEnemy = enemyUser;
    }
    // プレイヤーユーザー取得
    public BattleUser GetPlayerUser()
    {
        return m_UserPlayer;
    }
    // 敵ユーザー取得
    public BattleUser GetEnemyUser()
    {
        return m_UserEnemy;
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
        if(m_OperateUserSide == Side.eSide_Player)
        {
            return m_UserPlayer;
        }
        else if(m_OperateUserSide == Side.eSide_Enemy)
        {
            return m_UserEnemy;
        }

        return null;
    }
}
