using UnityEngine;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using battleTypes;
using static Common;

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
		Debug.Log($"{nameof(RPC_SyncPhases_MC)}");
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
    public async void RPC_SyncUser_MC(int _userSide, int _id, int _phaseType, bool _phaseReadyFlag)
    {
		Debug.Log($"{nameof(RPC_SyncUser_MC)}" +
				  $"_userSide：{_userSide}, _id：{_id}, _phaseType：{_phaseType}, _phaseReadyFlag：{_phaseReadyFlag}");
		// マスタークライアントならはじく
		if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogError($"{nameof(RPC_SyncUser_MC)}がマスタークライアントで呼ばれているため処理を行わず終了しました");
            return;
        }

		// 後々選択中のカードエリアも同期してください n_oishi 2025/3/6
		// 自分と相手のユーザー情報の側は逆なのでここで逆にする
		Side		userSide	=	GetRevSide((Side)_userSide);	// ユーザー側
		PhaseType	phaseType	=	(PhaseType)_phaseType;			// 現在のフェイズ

		Test_UserMgr test_UserMgr = Test_UserMgr.instance;
		Test_User test_User = test_UserMgr.GetUser(userSide);

		// ユーザーの生成が完了するまで待機
		await UniTask.WaitUntil(() => test_User != null);

		// ユーザー設定
		if(test_User.GetSetID == 0)
		{
			test_User.GetSetID = _id;
		}
		test_User.GetSetPhaseType = phaseType;
		test_User.GetSetPhaseReadyFlag = _phaseReadyFlag;
	}

    // ユーザー情報の送信
    [PunRPC]
    public async void RPC_PushUser_CM(int _userSide, int _phaseType, bool _phaseReadyFlag)
    {
		Debug.Log($"{nameof(RPC_PushUser_CM)}" +
				  $"_userSide：{_userSide}, _phaseType：{_phaseType}, _phaseReadyFlag：{_phaseReadyFlag}");
        // 非マスタークライアントならはじく
        if(!PhotonNetwork.IsMasterClient)
        {
			Debug.LogError($"{nameof(RPC_PushUser_CM)}非マスタークライアントで呼ばれているため処理を行わず終了しました");
			return;
		}

		// 自分と相手のユーザー情報の側は逆なのでここで逆にする
		Side userSide = GetRevSide((Side)_userSide);    // ユーザー側
		PhaseType phaseType = (PhaseType)_phaseType;            // 現在のフェイズ

		Test_UserMgr test_UserMgr = Test_UserMgr.instance;
		Test_User test_User = test_UserMgr.GetUser(userSide);

		// ユーザーの生成が完了するまで待機
		await UniTask.WaitUntil(() => test_User != null);

		test_User.GetSetPhaseType = phaseType;
		test_User.GetSetPhaseReadyFlag = _phaseReadyFlag;
	}

	/////////////////////////////
	// ===== CardAbility ===== //
	/////////////////////////////
	
}