﻿using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class PhaseManager : MonoBehaviourPunCallbacks
{
	public enum PhaseType
	{
		Start,
		Join,
		Main,
		End
	}

	[SerializeField]
	private Phase[] m_Phases;
	[SerializeField]
	private PhaseType m_PhaseType;
	[SerializeField]
	private GameObject m_PhaseParent; // 親オブジェクト（===Phase===）
	[SerializeField]
	private int m_SyncDelay;	// 同期待機時間

	private async void Start()
	{
		Debug.Log($"PhotonNetwork.IsMasterClient：{PhotonNetwork.IsMasterClient}");

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

			// 各フェイズオブジェクトのPhotonViewIdを取得
			int[] phaseViewIDs = new int[m_Phases.Length];

			for (int i = 0; i < m_Phases.Length; i++)
			{
				PhotonView photonView = m_Phases[i]?.GetComponent<PhotonView>();

				if (!photonView)
				{
					Debug.LogWarning($"PhotonViewIdが取得できませんでした。|| m_Phases.Length：{m_Phases.Length}");
					continue;
				}

				// PhotonViewId取得
				phaseViewIDs[i] = photonView.ViewID;
			}

			// フェイズオブジェクトの通信同期を行う
			photonView.RPC(nameof(RPC_SyncPhases_MC), RpcTarget.OthersBuffered, phaseViewIDs);
		}

		// フェイズオブジェクトの生成、通信同期が正常に終了するまで待機
		await WaitSyncCreatePhase();

		// ターンループ処理
		RunTurnCycle().Forget();
	}

	// フェイズオブジェクトの生成
	private GameObject CreatePhasePhotonObject(string prefabName)
	{
		// Photonでフェーズオブジェクトを生成
		GameObject phaseObject = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);

		// 親オブジェクト
		if (!m_PhaseParent)
		{
			Debug.LogWarning($"親フェイズオブジェクト「{nameof(m_PhaseParent)}」が見つかりませんでした");
			return phaseObject;
		}

		// 生成されたオブジェクトの親を設定
		phaseObject.transform.SetParent(m_PhaseParent.transform);

		return phaseObject;
	}

	// フェイズオブジェクトの生成、通信同期が正常に行われるまで待機
	private async UniTask WaitSyncCreatePhase()
	{
		Debug.Log($"{nameof(WaitSyncCreatePhase)}開始");
		// フェイズオブジェクトの生成が行われるまで待機
		await UniTask.WaitUntil(() => m_Phases != null);
		// Indexに異常値が入っている場合は通信同期が正しく行われていないので待機
		await UniTask.WaitUntil(() => m_Phases.Length > 0 && m_Phases.Length > (int)m_PhaseType);

		Debug.Log($"{nameof(WaitSyncCreatePhase)}終了");
	}

	[PunRPC]
	private void RPC_SyncPhases_MC(int[] phaseViewIDs)
	{
		m_Phases = new Phase[phaseViewIDs.Length];

		for (int i = 0; i < phaseViewIDs.Length; i++)
		{
			PhotonView photonView = PhotonView.Find(phaseViewIDs[i]);
			if (!photonView)
			{
				Debug.LogError($"PhotonViewID {phaseViewIDs[i]} のオブジェクトが見つかりませんでした。");
				continue;
			}

			m_Phases[i] = photonView.GetComponent<Phase>();
			m_Phases[i].transform.SetParent(m_PhaseParent.transform);
		}
	}

	// フェイズ取得
	private Phase GetPhase(GameObject phaseObject)
	{

		if (!phaseObject) { return null; }
		// フェイズ取得
		Phase phase = phaseObject.GetComponent<Phase>();

		return phase;
	}

	// 全フェイズ初期化
	private void InitAllPhase()
	{

	}

	// ターンループ処理
	private async UniTask RunTurnCycle()
	{
		Debug.Log("ターンループ処理開始");

		// 全フェイズ初期化
		foreach (var phase in m_Phases)
		{
			// フェイズ初期化
			phase.InitPhase();
		}

		while (true)
		{
			if(m_Phases == null)
			{
				Debug.LogError($" m_Phases がNULLなので {(double)(m_SyncDelay / 1000)} 秒同期待ちを行います。");
				await UniTask.Delay(m_SyncDelay);
				Debug.Log($"同期待ちが終了したので処理を再開します");
				continue;
			}

			if(m_Phases.Length < 0 || m_Phases.Length <= (int)m_PhaseType)
			{
				Debug.LogWarning($"範囲外Index参照エラー || m_Phases.Length：{m_Phases.Length} m_PhaseType：{m_PhaseType}");
				await UniTask.Delay(m_SyncDelay);
				continue;
			}

			// フェイズ処理
			await m_Phases[(int)m_PhaseType].RunPhase();

			// 左シフト+Pでフェイズ移行
			if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
			{
				PhaseType nextPhaseType = m_PhaseType + 1;
				Debug.Log($"フェイズ移行：{m_PhaseType}→{nextPhaseType}");
				// フェイズ初期化
				m_Phases[(int)m_PhaseType].InitPhase();
				m_PhaseType++;
			}

			// 通信同期

			
			if ((int)m_PhaseType >= m_Phases.Length)
			{
				m_PhaseType = PhaseType.Start; // 次のターンへ
				Debug.Log("Next Turn");
			}

			await UniTask.Yield();
		}
	}
}