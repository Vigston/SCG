using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
	public enum PhaseType
	{
		Start,
		Join,
		Main,
		End
	}

	// インスタンス
	public static PhaseManager instance;

	[SerializeField]
	private Phase[] m_Phases;
	[SerializeField]
	private PhaseType m_PhaseType;
	[SerializeField]
	private GameObject m_PhaseParent; // 親オブジェクト（===Phase===）
	[SerializeField]
	private int m_SyncDelay;    // 同期待機時間

	private void Awake()
	{
		// インスタンス生成
		CreateInstance();
	}

	private async void Start()
	{
		Debug.Log($"PhotonNetwork.IsMasterClient：{PhotonNetwork.IsMasterClient}");

		if (!GetSetPhaseParentObj)
		{
			Debug.LogError($"親フェイズオブジェクト「{nameof(GetSetPhaseParentObj)}」が見つかりません！{nameof(PhaseManager)}にアタッチされているか確認してください。");
			return;
		}

		if(PhotonNetwork.IsMasterClient)
		{
			// 全フェイズゲームオブジェクト生成
			GameObject startPhaseObj = CreatePhasePhotonObject("TestStartPhasePrefab");
			GameObject joinPhaseObj = CreatePhasePhotonObject("TestJoinPhasePrefab");
			GameObject mainPhaseObj = CreatePhasePhotonObject("TestMainPhasePrefab");
			GameObject endPhaseObj = CreatePhasePhotonObject("TestEndPhasePrefab");

			// フェイズ取得
			GetSetPhases = new Phase[]
			{
				GetPhase(startPhaseObj),
				GetPhase(joinPhaseObj),
				GetPhase(mainPhaseObj),
				GetPhase(endPhaseObj),
			};

			// 各フェイズオブジェクトのPhotonViewIdを取得
			int[] phaseViewIDs = new int[GetSetPhases.Length];

			for (int i = 0; i < GetSetPhases.Length; i++)
			{
				PhotonView photonView = GetSetPhases[i]?.GetComponent<PhotonView>();

				if (!photonView)
				{
					Debug.LogWarning($"PhotonViewIdが取得できませんでした。|| m_Phases.Length：{GetSetPhases.Length}");
					continue;
				}

				// PhotonViewId取得
				phaseViewIDs[i] = photonView.ViewID;
			}

			// フェイズオブジェクトの通信同期を行う
			Test_NetWorkMgr test_NetWorkMgr = Test_NetWorkMgr.instance;
			test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncPhases_MC), RpcTarget.OthersBuffered, phaseViewIDs);
		}

		// フェイズオブジェクトの生成、通信同期が正常に終了するまで待機
		await WaitSyncCreatePhase();

		// ターンループ処理
		RunTurnCycle().Forget();
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

	// ターンループ処理
	private async UniTask RunTurnCycle()
	{
		Debug.Log("ターンループ処理開始");

		// 全フェイズ初期化
		foreach (var phase in GetSetPhases)
		{
			// フェイズ初期化
			phase.InitPhase();
		}

		while (true)
		{
			if(GetSetPhases == null)
			{
				Debug.LogError($" m_Phases がNULLなので {(double)(GetSetSyncDelay / 1000)} 秒同期待ちを行います。");
				await UniTask.Delay(GetSetSyncDelay);
				Debug.Log($"同期待ちが終了したので処理を再開します");
				continue;
			}

			if(GetSetPhases.Length < 0 || GetSetPhases.Length <= (int)GetSetPhaseType)
			{
				Debug.LogWarning($"範囲外Index参照エラー || m_Phases.Length：{GetSetPhases.Length} m_PhaseType：{GetSetPhaseType}");
				await UniTask.Delay(GetSetSyncDelay);
				continue;
			}

			// フェイズ処理
			await GetSetPhases[(int)GetSetPhaseType].RunPhase();

			////////////////////////
			///// フェイズ遷移 /////
			////////////////////////
			// 次のフェイズに遷移可能な状態になるまで待機
			await UniTask.WaitUntil(() => IsSwitchPhase());

			PhaseType nextPhaseType = GetSetPhaseType + 1;
			Debug.Log($"フェイズ移行：{GetSetPhaseType}→{nextPhaseType}");
			// フェイズ初期化
			GetSetPhases[(int)GetSetPhaseType].InitPhase();
			GetSetPhaseType++;

			// 通信同期


			if ((int)GetSetPhaseType >= GetSetPhases.Length)
			{
				GetSetPhaseType = PhaseType.Start; // 次のターンへ
				Debug.Log("Next Turn");
			}

			await UniTask.Yield();
		}
	}

	//////////////////////////
	// ===== フェイズ ===== //
	//////////////////////////
	// フェイズオブジェクトの生成
	private GameObject CreatePhasePhotonObject(string prefabName)
	{
		// Photonでフェーズオブジェクトを生成
		GameObject phaseObject = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);

		// 親オブジェクト
		if (!GetSetPhaseParentObj)
		{
			Debug.LogWarning($"親フェイズオブジェクト「{nameof(GetSetPhaseParentObj)}」が見つかりませんでした");
			return phaseObject;
		}

		// 生成されたオブジェクトの親を設定
		phaseObject.transform.SetParent(GetSetPhaseParentObj.transform);

		return phaseObject;
	}
	// フェイズオブジェクトの生成、通信同期が正常に行われるまで待機
	private async UniTask WaitSyncCreatePhase()
	{
		Debug.Log($"{nameof(WaitSyncCreatePhase)}開始");
		// フェイズオブジェクトの生成が行われるまで待機
		await UniTask.WaitUntil(() => GetSetPhases != null);
		// Indexに異常値が入っている場合は通信同期が正しく行われていないので待機
		await UniTask.WaitUntil(() => GetSetPhases.Length > 0 && GetSetPhases.Length > (int)GetSetPhaseType);

		Debug.Log($"{nameof(WaitSyncCreatePhase)}終了");
	}
	// フェイズ取得
	private Phase GetPhase(GameObject phaseObject)
	{

		if (!phaseObject) { return null; }
		// フェイズ取得
		Phase phase = phaseObject.GetComponent<Phase>();

		return phase;
	}
	// 次のフェイズへ遷移可能な状態か
	private bool IsSwitchPhase()
	{
		Test_User playerUser = Test_UserMgr.instance.GetSetPlayerUser;
		Test_User enemyUser = Test_UserMgr.instance.GetSetEnemyUser;

		// ユーザーが取得できていないなら不可能
		if (!playerUser || !enemyUser)
		{
			Debug.LogError($"{nameof(IsSwitchPhase)}でユーザー取得ができていなかったのでフェイズ遷移を行えませんでした。" +
						   $"PlayerUser：{playerUser} || EnemyUser：{enemyUser}");
			return false;
		}

		// マスタークライアント
		if(PhotonNetwork.IsMasterClient)
		{
			// 自分と相手のユーザーフェイズが一致しないので不可能
			if (playerUser.GetSetPhaseType != enemyUser.GetSetPhaseType)
			{
				Debug.LogError($"自分と相手のフェイズが一致しないのでフェイズ遷移を行えませんでした。通信同期が正しく行えているのか確認をお願いします。" +
							   $"PlayerUser：{playerUser.GetSetPhaseType} || EnemyUser：{enemyUser.GetSetPhaseType}");
				return false;
			}

			// 自分と相手のユーザーフェイズ同期待ちフラグが両方立っていないので不可能
			if (!playerUser.GetSetPhaseReadyFlag || !enemyUser.GetSetPhaseReadyFlag)
			{
				Debug.LogError($"自分と相手のフェイズ同期待ちフラグが両方立っていないのでフェイズ遷移を行えませんでした。通信同期が正しく行えているのか確認をお願いします。" +
							   $"PlayerUser：{playerUser.GetSetPhaseReadyFlag} || EnemyUser：{enemyUser.GetSetPhaseReadyFlag}");
				return false;
			}
		}

		// 自分と相手がユーザーフェイズ遷移可能じゃないので不可能
		if (!playerUser.GetSetPhaseSwitchFlag || !enemyUser.GetSetPhaseSwitchFlag)
		{
			Debug.LogError($"自分と相手がフェイズ遷移可能じゃないのでフェイズ遷移を行えません。通信同期が正しく行われているのか確認をお願いします。" +
						   $"PlayerUser：{playerUser.GetSetPhaseSwitchFlag} || EnemyUser：{enemyUser.GetSetPhaseSwitchFlag}");
			return false;
		}

		// 遷移可能
		return true;
	}

	//////////////////////////////////
	// ===== GetSetプロパティ ===== //
	//////////////////////////////////
	public Phase[] GetSetPhases
	{
		get { return m_Phases; }
		set { m_Phases = value; }
	}

	public PhaseType GetSetPhaseType
	{
		get { return m_PhaseType; }
		set { m_PhaseType = value; }
	}

	public GameObject GetSetPhaseParentObj
	{
		get { return m_PhaseParent; }
		set { m_PhaseParent = value; }
	}

	public int GetSetSyncDelay
	{
		get { return m_SyncDelay; }
		set { m_SyncDelay = value; }
	}
}