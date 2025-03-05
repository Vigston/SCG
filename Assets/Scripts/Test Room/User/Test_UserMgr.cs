using UnityEngine;
using battleTypes;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;

public class Test_UserMgr : MonoBehaviour
{
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
    private async void Start()
    {
        // マスタークライアントじゃないならはじく
        if (!PhotonNetwork.IsMasterClient) return;

        // ユーザー作成
        CreatePlayerUser();     // プレイヤー
        CreateEnemyUser();      // 敵

		// ユーザーの生成が完了するまで待機
		await UniTask.WaitUntil(() => GetSetPlayerUser != null && GetSetEnemyUser != null);

        /////通信同期/////
        Test_NetWorkMgr test_NetWorkMgr = Test_NetWorkMgr.instance;
        // Userクラスをjson形式に変換して送信
        string playerUserjsonData = JsonUtility.ToJson(GetSetPlayerUser);
		string enemyUserjsonData = JsonUtility.ToJson(GetSetEnemyUser);
        test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncUser_MC), RpcTarget.OthersBuffered, playerUserjsonData, enemyUserjsonData);
	}

    // Update is called once per frame
    private void Update()
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
		foreach (var user in GetSetUsers)
		{
			if (!user) continue;

            user.Init_PhaseInfo();
		}
	}

    // ---ユーザー---
    // プレイヤーユーザー作成
	public void CreatePlayerUser()
	{
		// プレイヤーユーザーがないならはじく
		if (!GetSetPlayerUser) { return; }

		// == ユーザー設定 ==
		GetSetPlayerUser.GetSetSide   = Side.eSide_Player;                        // 側
		GetSetPlayerUser.GetSetID     = PhotonNetwork.LocalPlayer.ActorNumber;    // ID
	}

    // 敵ユーザー作成
	public void CreateEnemyUser()
	{
		// 敵ユーザーがないならはじく
		if (!GetSetEnemyUser) { return; }

		// 敵ユーザー設定
		GetSetEnemyUser.GetSetSide = Side.eSide_Enemy;          // 側

        // 相手プレイヤーのActorNumberをIDとして取得
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // 自分以外のプレイヤーなら敵なのでIDを設定
            if(player != PhotonNetwork.LocalPlayer)
            {
				GetSetEnemyUser.GetSetID = player.ActorNumber;  // ID
                break;
            }
        }
	}

    // ユーザーリスト
    public Test_User[] GetSetUsers
    {
        get { return m_Users; }
        set { m_Users = value; }
    }
	// プレイヤーユーザー
	public Test_User GetSetPlayerUser
    {
		get { return GetSetUsers[(int)Side.eSide_Player]; }
        set { GetSetUsers[(int)Side.eSide_Player] = value; }
    }
	// 敵ユーザー
	public Test_User GetSetEnemyUser
	{
		get { return GetSetUsers[(int)Side.eSide_Enemy]; }
		set { GetSetUsers[(int)Side.eSide_Enemy] = value; }
	}
    // 指定側のユーザー取得
    public Test_User GetUser(Side Side)
    {
        return GetSetUsers[(int)Side];
    }
    // ユーザーIDからユーザーを取得
    public Test_User GetUserFromUserID(int _userID)
    {
		return GetSetUsers.FirstOrDefault(user => user != null && user.GetSetID == _userID);
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
        get { return GetSetUsers[(int)m_OperateUserSide]; }
        set { GetSetUsers[(int)m_OperateUserSide] = value; }
    }
    // 指定の操作側か
    public bool IsOperateSide(Side _side)
    {
        return m_OperateUserSide == _side;
	}
}
