using UnityEngine;
using battleTypes;
using System.Linq;

public class Test_UserMgr : MonoBehaviour
{
    // =================変数=================
    // インスタンス
    public static Test_UserMgr instance;

    [SerializeField]
    private Test_User[] m_Users = new Test_User[(int)Side.eSide_Max];

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

        Debug.LogError($"{this}のインスタンスが生成できませんでした");
        return false;
    }

    // === 初期化 ===
    // ユーザーのフェイズ情報初期化
    public void Init_User_PhaseInfo()
    {
        // 各ユーザーのフェイズ情報初期化
		foreach (var user in m_Users)
		{
			if (!user) continue;

            user.Init_PhaseInfo();
		}
	}

    // ---ユーザー---
    // プレイヤーユーザー
    public Test_User GetSetPlayerUser
    {
		get { return m_Users[(int)Side.eSide_Player]; }
        set { m_Users[(int)Side.eSide_Player] = value; }
    }
	// 敵ユーザー
	public Test_User GetSetEnemyUser
	{
		get { return m_Users[(int)Side.eSide_Enemy]; }
		set { m_Users[(int)Side.eSide_Enemy] = value; }
	}
    // 指定側のユーザー取得
    public Test_User GetUser(Side Side)
    {
        return m_Users[(int)Side];
    }
    // ユーザーIDからユーザーを取得
    public Test_User GetUserFromUserID(int _userID)
    {
		return m_Users.FirstOrDefault(user => user != null && user.GetSetID == _userID);
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
    public Test_User GetSetOperateUser
    {
        get { return m_Users[(int)m_OperateUserSide]; }
        set { m_Users[(int)m_OperateUserSide] = value; }
    }
    // 指定の操作側か
    public bool IsOperateSide(Side _side)
    {
        return m_OperateUserSide == _side;
	}
}
