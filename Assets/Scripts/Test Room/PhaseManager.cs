using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class PhaseManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private Phase[] m_Phases;
	[SerializeField]
	private int m_CurrentPhaseIndex = 0;
	[SerializeField]
	private GameObject m_PhaseParent; // 親オブジェクト（===Phase===）
	[SerializeField]
	private int m_SyncDelay;	// 同期待機時間

	private async void Start()
	{
		if (m_PhaseParent == null)
		{
			Debug.LogError($"親フェイズオブジェクト「{nameof(m_PhaseParent)}」が見つかりません！{nameof(PhaseManager)}にアタッチされているか確認してください。");
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
			m_Phases = new Phase[]
			{
				GetPhase(startPhaseObj),
				GetPhase(joinPhaseObj),
				GetPhase(mainPhaseObj),
				GetPhase(endPhaseObj),
			};
		}
		else
		{
			// **非マスタークライアント側：フェーズオブジェクトを検索して取得**
			await SetPhasesForClients();
		}

		// ターンループ処理
		RunTurnCycle().Forget();
	}

	// フェイズオブジェクトの生成
	private GameObject CreatePhasePhotonObject(string prefabName)
	{
		// Photonでフェーズオブジェクトを生成
		GameObject phaseObject = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);

		// 親オブジェクト
		if (m_PhaseParent == null)
		{
			Debug.LogError($"親フェイズオブジェクト「{nameof(m_PhaseParent)}」が見つかりませんでした");
			return phaseObject;
		}

		// 生成されたオブジェクトの親を設定
		phaseObject.transform.SetParent(m_PhaseParent.transform);

		return phaseObject;
	}

	// フェーズリストを取得 (非マスタークライアント側)
	private async UniTask SetPhasesForClients()
	{
		// マスタークライアントならはじく
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogError($"非マスタークライアントの処理がマスタークライアントで呼ばれています：{nameof(SetPhasesForClients)}");
			return;
		}

		Debug.Log("非マスタークライアント側でフェーズオブジェクトの取得開始");

		while (m_Phases == null || m_Phases.Length == 0)
		{
			await UniTask.Delay(m_SyncDelay); // データ同期待機

			if (m_Phases.Length > 0)
			{
				Debug.Log($"フェーズオブジェクトを取得完了（{m_Phases.Length} 個）");
			}

			await UniTask.Yield();
		}

		// **親オブジェクトの設定（念のため）**
		foreach (var phase in m_Phases)
		{
			if (phase != null && phase.transform.parent == null)
			{
				// 生成されたオブジェクトの親を設定
				phase.transform.SetParent(m_PhaseParent.transform);
			}
		}

		await UniTask.Yield();
	}

	// フェイズ取得
	private Phase GetPhase(GameObject phaseObject)
	{

		if (!phaseObject) { return null; }
		// フェイズ取得
		Phase phase = phaseObject.GetComponent<Phase>();

		return phase;
	}

	// ターンループ処理
	private async UniTask RunTurnCycle()
	{
		while (true)
		{
			if(m_Phases == null)
			{
				Debug.LogError($" m_Phases がNULLなので {(double)(m_SyncDelay / 1000)} 秒同期待ちを行います。");
				await UniTask.Delay(m_SyncDelay);
				Debug.Log($"同期待ちが終了したので処理を再開します");
				continue;
			}
			await m_Phases[m_CurrentPhaseIndex].RunPhase();

			m_CurrentPhaseIndex++;
			if (m_CurrentPhaseIndex >= m_Phases.Length)
			{
				m_CurrentPhaseIndex = 0; // 次のターンへ
				Debug.Log("Next Turn");
			}

			await UniTask.Yield();
		}
	}
}