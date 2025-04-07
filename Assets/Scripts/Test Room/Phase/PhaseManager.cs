using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
	public enum PhaseType
	{
		Start,
		Join,
		Main,
		End,

		TurnEnd,
	}

	// インスタンス
	public static PhaseManager instance;

	[SerializeField]
	private Phase m_Phase;

	[SerializeField]
	private Phase[] m_Phases;
	[SerializeField]
	private PhaseType m_PhaseType;
	[SerializeField]
	private GameObject m_PhaseParent; // 親オブジェクト（===Phase===）
	[SerializeField]
	private int m_SyncDelay;    // 同期待機時間

	[SerializeField]
	private int m_TimeoutDuration; // タイムアウトまでの時間（秒）

	[SerializeField, ReadOnly]
	private bool m_IsWaitingForNetWork = false; // 通信同期状態

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
			if (GetSetPhases == null)
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

			// 参照取得
			Test_NetWorkMgr test_NetWorkMgr = Test_NetWorkMgr.instance;
			Test_User playerUser = Test_UserMgr.instance.GetSetPlayerUser;
			Test_User enemyUser = Test_UserMgr.instance.GetSetEnemyUser;

			// 対戦開始フラグを立てる
			playerUser.GetSetGameStartFlag = true;

			// 非マスタークライアントならマスタークライアントに自分のユーザー情報を送信
			if (!PhotonNetwork.IsMasterClient)
			{
				test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_PushUser_CM), RpcTarget.AllBuffered, playerUser.GetSetSide, playerUser.GetSetPhaseType, playerUser.GetSetPhaseReadyFlag, playerUser.GetSetGameStartFlag);
			}

			// フェイズ開始可能な状態になるまで待機
			try
			{
				IsWaitingForCommunication = true;
				await UniTask.WaitUntil(() => IsPhaseStartable()).Timeout(TimeSpan.FromSeconds(m_TimeoutDuration));
				break;
			}
			catch (TimeoutException)
			{
				Debug.LogError($"フェイズ開始可能な状態になるまでの時間が経過しました。タイムアウト処理を実行します。");
				OnTimeout();
				// 再試行のためにループを続ける
			}
			finally
			{
				IsWaitingForCommunication = false;
			}



			////////////////////
			///// 通信同期 /////
			////////////////////
			if (PhotonNetwork.IsMasterClient)
			{
				test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncUser_MC), RpcTarget.OthersBuffered, playerUser.GetSetSide, playerUser.GetSetID, playerUser.GetSetPhaseType, playerUser.GetSetPhaseReadyFlag, playerUser.GetSetGameStartFlag);
				test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncUser_MC), RpcTarget.OthersBuffered, enemyUser.GetSetSide, enemyUser.GetSetID, enemyUser.GetSetPhaseType, enemyUser.GetSetPhaseReadyFlag, enemyUser.GetSetGameStartFlag);
			}

			// フェイズ処理
			await GetSetPhases[(int)GetSetPhaseType].UpdatePhase();

			////////////////////////
			///// フェイズ終了 /////
			////////////////////////
			// フェイズ終了同期待ち状態に設定
			playerUser.GetSetPhaseReadyFlag = true;

			// 非マスタークライアントならマスタークライアントに自分のユーザー情報を送信
			if(!PhotonNetwork.IsMasterClient)
			{
				test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_PushUser_CM), RpcTarget.AllBuffered, playerUser.GetSetSide, playerUser.GetSetPhaseType, playerUser.GetSetPhaseReadyFlag, playerUser.GetSetGameStartFlag);
			}

			// 次のフェイズに遷移可能な状態になるまで待機
			try
			{
				IsWaitingForCommunication = true;
				await UniTask.WaitUntil(() => IsSwitchPhase()).Timeout(TimeSpan.FromSeconds(m_TimeoutDuration));
				break;
			}
			catch (TimeoutException)
			{
				Debug.LogError("次のフェイズに遷移可能な状態になるまでの待機がタイムアウトしました。");
				OnTimeout();
				// 再試行のためにループを続ける
			}
			finally
			{
				IsWaitingForCommunication = false;
			}

			////////////////////
			///// 通信同期 /////
			////////////////////
			if (PhotonNetwork.IsMasterClient)
			{
				test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncUser_MC), RpcTarget.OthersBuffered, playerUser.GetSetSide, playerUser.GetSetID, playerUser.GetSetPhaseType, playerUser.GetSetPhaseReadyFlag, playerUser.GetSetGameStartFlag);
				test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncUser_MC), RpcTarget.OthersBuffered, enemyUser.GetSetSide, enemyUser.GetSetID, enemyUser.GetSetPhaseType, enemyUser.GetSetPhaseReadyFlag, enemyUser.GetSetGameStartFlag);
			}

			////////////////////////
			///// フェイズ終了 /////
			////////////////////////
			// フェイズ終了時の処理
			OnPhaseEnd();

			//////////////////////
			///// ターン遷移 /////
			//////////////////////
			// 全てのフェイズが終了しているならターン遷移を行う
			if ((int)GetSetPhaseType >= GetSetPhases.Length - 1)
			{
				// ターン終了時の処理
				OnTurnEnd();
			}
			////////////////////////
			///// フェイズ遷移 /////
			////////////////////////
			else
			{
				// フェイズ遷移時の処理
				OnSwitchPhase();
			}

			await UniTask.Yield();
		}
	}

	// タイムアウト時の処理
	private void OnTimeout()
	{
		// ここにタイムアウト時の処理を実装します
		// 例: ゲームを一時停止し、再接続を試みる
		Debug.Log("タイムアウトが発生しました。ゲームを一時停止し、再接続を試みます...");
		// 再接続処理を実装
		PhotonNetwork.ReconnectAndRejoin();
	}

	////////////////////////
	// ===== ターン ===== //
	////////////////////////
	// ターン終了時
	private void OnTurnEnd()
	{
		GameMgr gameMgr = GameMgr.instance;

		// スタートフェイズに設定
		GetSetPhaseType = PhaseType.Start;

		// ターンカウント加算
		gameMgr.GetSetTurnCnt++;
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
	// フェイズ終了時
	private void OnPhaseEnd()
	{
		Test_User playerUser = Test_UserMgr.instance.GetSetPlayerUser;
		// フェイズ終了時処理
		GetSetPhases[(int)GetSetPhaseType].EndPhase();
		// ユーザーのフェイズ情報を初期化
		playerUser.Init_PhaseInfo();
	}
	// フェイズ遷移時
	private void OnSwitchPhase()
	{
		PhaseType nextPhaseType = GetSetPhaseType + 1;
		Debug.Log($"フェイズ遷移：{GetSetPhaseType}→{nextPhaseType}");

		// == 次のフェイズへ == //
		GetSetPhaseType++;
	}
	// フェイズ開始可能な状態か
	private bool IsPhaseStartable()
	{
		Test_User playerUser = Test_UserMgr.instance.GetSetPlayerUser;
		Test_User enemyUser = Test_UserMgr.instance.GetSetEnemyUser;

		// ユーザーが取得できていないなら不可能
		if (!playerUser || !enemyUser)
		{
			Debug.LogError($"{nameof(IsSwitchPhase)}でユーザー取得ができていなかったのでフェイズ処理を開始できませんでした。" +
						   $"PlayerUser：{playerUser} || EnemyUser：{enemyUser}");
			return false;
		}

		// ゲーム開始していないならはじく
		if (!playerUser.GetSetGameStartFlag || !enemyUser.GetSetGameStartFlag)
		{
			Debug.LogError($"自分と相手のゲームが開始されていないのでフェイズ処理を開始できませんでした。通信同期が正しく行えているのか確認をお願いします。" +
						   $"PlayerUser：{playerUser.GetSetGameStartFlag} || EnemyUser：{enemyUser.GetSetGameStartFlag}");
			return false;
		}

		// 自分と相手のユーザーフェイズが一致しないので不可能
		if (playerUser.GetSetPhaseType != enemyUser.GetSetPhaseType)
		{
			Debug.LogError($"自分と相手のフェイズが一致しないのでフェイズ処理を開始できませんでした。通信同期が正しく行えているのか確認をお願いします。" +
						   $"PlayerUser：{playerUser.GetSetPhaseType} || EnemyUser：{enemyUser.GetSetPhaseType}");
			return false;
		}

		// 自分と相手のユーザーフェイズ同期待ちフラグがどちらか立っているので不可能
		if (!playerUser.GetSetPhaseReadyFlag || !enemyUser.GetSetPhaseReadyFlag)
		{
			Debug.LogError($"自分と相手のフェイズ同期待ちフラグがどちらか立っているのでフェイズ処理を開始できませんでした。通信同期が正しく行えているのか確認をお願いします。" +
						   $"PlayerUser：{playerUser.GetSetPhaseReadyFlag} || EnemyUser：{enemyUser.GetSetPhaseReadyFlag}");
			return false;
		}

		// 開始可能
		return true;
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

		Debug.Log($"フェイズ遷移可能なので{GetSetPhaseType}から遷移します。" +
				  $"[Player]PhaseType：{playerUser.GetSetPhaseType}, PhaseReadyFlag：{playerUser.GetSetPhaseReadyFlag}" +
				  $"[Enemy]PhaseType：{enemyUser.GetSetPhaseType}, PhaseReadyFlag：{enemyUser.GetSetPhaseReadyFlag}");

		// 遷移可能
		return true;
	}

	//////////////////////////////////
	// ===== GetSetプロパティ ===== //
	//////////////////////////////////
	public Phase GetSetPhase
	{
		get { return m_Phase; }
		set { m_Phase = value; }
	}

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

	public bool IsWaitingForCommunication
	{
		get { return m_IsWaitingForNetWork; }
		private set { m_IsWaitingForNetWork = value; }
	}
}