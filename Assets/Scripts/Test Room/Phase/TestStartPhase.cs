using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class TestStartPhase : Phase
{
	// 独自のState列挙型
	public enum StartPhaseState
	{
		StartState,             // 開始
		MainState,              // メイン
		EndState,               // 終了

		TurnEndState = -1,      // ターン終了
	}

	private void Awake()
	{
		// ステートとアクションを辞書型で紐づける
		GetSetActionDict = new Dictionary<Enum, Action>()
		{
			{ StartPhaseState.StartState, StartStateAction },
			{ StartPhaseState.MainState, MainStateAction },
			{ StartPhaseState.EndState, EndStateAction }
		};
	}

	// フェイズ進行中の処理
	public override async UniTask UpdatePhase()
	{
		// ターン終了じゃなければフェイズ更新を行う
		while ((StartPhaseState)GetSetState != StartPhaseState.TurnEndState)
		{
			// 参照取得
			Test_NetWorkMgr test_NetWorkMgr = Test_NetWorkMgr.instance;
			Test_User playerUser = Test_UserMgr.instance.GetSetPlayerUser;
			Test_User enemyUser = Test_UserMgr.instance.GetSetEnemyUser;

			Enum bufferState = GetSetState;

			// 状態遷移の処理はステートマシン側で行われるので、ここでアクションを実行
			if (GetSetActionDict.TryGetValue(GetSetState, out Action stateAction))
			{
				stateAction();
			}

			// フェイズフレーム加算
			GetSetPhaseFrame++;

			// ステート遷移しているならステート遷移時の処理を行って次のループへ
			if ((StartPhaseState)bufferState != (StartPhaseState)GetSetState)
			{
				// 通信同期
				// 非マスタークライアントならマスタークライアントに自分のユーザー情報を送信
				if (!PhotonNetwork.IsMasterClient)
				{
					test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_PushUser_CM), RpcTarget.MasterClient, playerUser.GetSetSide, playerUser.GetSetPhaseType, playerUser.GetSetPhaseReadyFlag, playerUser.GetSetPhaseState, playerUser.GetSetPhaseStateReadyFlag);
				}

				// 次のステートへ遷移可能な状態になるまで待機
				await UniTask.WaitUntil(() => IsSwitchState());

				// 通信同期
				if (PhotonNetwork.IsMasterClient)
				{
					test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncUser_MC), RpcTarget.OthersBuffered, playerUser.GetSetSide, playerUser.GetSetID, playerUser.GetSetPhaseType, playerUser.GetSetPhaseReadyFlag, playerUser.GetSetPhaseState, playerUser.GetSetPhaseStateReadyFlag);
					test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_SyncUser_MC), RpcTarget.OthersBuffered, enemyUser.GetSetSide, enemyUser.GetSetID, enemyUser.GetSetPhaseType, enemyUser.GetSetPhaseReadyFlag, enemyUser.GetSetPhaseState, enemyUser.GetSetPhaseStateReadyFlag);
				}

				// ステート遷移時の処理
				OnSwitchState();
			}
			// ステート遷移していないならステートフレーム加算
			else
			{
				// ステートフレーム加算
				GetSetStateFrame++;
			}

			// 次フレーム待機
			await UniTask.Yield();
		}

		// タスク終了
		await UniTask.CompletedTask;
	}

	/////////////////////
	//////アクション/////
	/////////////////////
	// 状態ごとのアクションを定義
	private void StartStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(StartStateAction)}");
		}

		// メインへ
		SwitchState(StartPhaseState.MainState);
	}

	private void MainStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(MainStateAction)}");
		}

		// 終了へ
		SwitchState(StartPhaseState.EndState);
	}

	private void EndStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(EndStateAction)}");
		}

		// ターン終了
		SwitchState(StartPhaseState.TurnEndState);
	}

	// 初期状態
	public override Enum GetInitState()
	{
		// 開始
		return StartPhaseState.StartState;
	}
}