using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;

public class TestMainPhase : Phase
{
	// 独自のState列挙型
	public enum MainPhaseState
	{
		StartState,             // 開始
		MainState,              // メイン
		EndState,               // 終了

		TurnEndState = -1,      // ターン終了
	}

	DamageCardAbility m_DamageCardAbility;

	private void Awake()
	{
		// ステートとアクションを辞書型で紐づける
		GetSetActionDict = new Dictionary<Enum, Action>()
		{
			{ MainPhaseState.StartState, StartStateAction },
			{ MainPhaseState.MainState, MainStateAction },
			{ MainPhaseState.EndState, EndStateAction }
		};
	}

	// フェイズ進行中の処理
	public override async UniTask UpdatePhase()
	{
		while ((MainPhaseState)GetSetState != MainPhaseState.TurnEndState)
		{
			Enum bufferState = GetSetState;

			// 状態遷移の処理はステートマシン側で行われるので、ここでアクションを実行
			if (GetSetActionDict.TryGetValue(GetSetState, out Action stateAction))
			{
				stateAction();
			}

			// フェイズフレーム加算
			GetSetPhaseFrame++;

			// ステート遷移しているならステート遷移時の処理を行って次のループへ
			if ((MainPhaseState)bufferState != (MainPhaseState)GetSetState)
			{
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
		SwitchState(MainPhaseState.MainState);
	}

	private async void MainStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(MainStateAction)}");

			if(PhotonNetwork.IsMasterClient)
			{
				// カードアビリティテスト
				int abilityId = PhotonNetwork.LocalPlayer.ActorNumber;
				m_DamageCardAbility = CardAbilityManager.instance.ActivateAbility<DamageCardAbility>(abilityId, 10);
			}
		}

		await CardAbilityManager.instance.WaitForAbility(m_DamageCardAbility);

		// 終了へ
		SwitchState(MainPhaseState.EndState);
	}

	private void EndStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(EndStateAction)}");
		}

		// ターン終了
		SwitchState(MainPhaseState.TurnEndState);
	}

	// 初期状態
	public override Enum GetInitState()
	{
		// 開始
		return MainPhaseState.StartState;
	}
}