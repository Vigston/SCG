using UnityEngine;
using Photon.Pun;

public class Test_NetWorkMgr : MonoBehaviourPun
{
	// インスタンス
	public static Test_NetWorkMgr instance;

	private void Awake()
	{
		// インスタンス生成
		CreateInstance();
	}


	void Start()
	{

	}


	void Update()
	{

	}

	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (!instance)
		{
			// 作成
			instance = this;
		}

		// インスタンスが作成済みなら終了
		if (instance) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	//////////////////////////
	// ===== フェイズ ===== //
	//////////////////////////
	// フェイズオブジェクトの通信同期処理
	[PunRPC]
	public void RPC_SyncPhases_MC(int[] phaseViewIDs)
	{
		// マスタークライアントならはじく
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogError($"{nameof(RPC_SyncPhases_MC)}がマスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}

		PhaseManager phaseManager = PhaseManager.instance;

		phaseManager.GetSetPhases = new Phase[phaseViewIDs.Length];

		for (int i = 0; i < phaseViewIDs.Length; i++)
		{
			PhotonView photonView = PhotonView.Find(phaseViewIDs[i]);
			if (!photonView)
			{
				Debug.LogError($"PhotonViewID {phaseViewIDs[i]} のオブジェクトが見つかりませんでした。");
				continue;
			}

			phaseManager.GetSetPhases[i] = photonView.GetComponent<Phase>();
			phaseManager.GetSetPhases[i].transform.SetParent(phaseManager.GetSetPhaseParentObj.transform);
		}
	}

	//////////////////////////
	// ===== ユーザー ===== //
	//////////////////////////
	// ユーザー情報の通信同期
	[PunRPC]
    public void RPC_SyncUser_MC(string _playerUserjsonData, string _enemyUserjsonData)
    {
        // マスタークライアントならはじく
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogError($"{nameof(RPC_SyncUser_MC)}がマスタークライアントで呼ばれているため処理を行わず終了しました");
            return;
        }

		Test_UserMgr test_UserMgr = Test_UserMgr.instance;

		// 受け取ったユーザー情報をこちらの環境に設定
		test_UserMgr.GetSetPlayerUser = JsonUtility.FromJson<Test_User>(_playerUserjsonData);
		test_UserMgr.GetSetEnemyUser = JsonUtility.FromJson<Test_User>(_enemyUserjsonData);
	}

    // ユーザー情報の送信
    [PunRPC]
    public void RPC_PushUser_CM()
    {
        // マスタークライアントじゃないならはじく
        if(!PhotonNetwork.IsMasterClient)
        {
			Debug.LogError($"{nameof(RPC_PushUser_CM)}非マスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}
    }
}