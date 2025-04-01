using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class Phase : MonoBehaviour
{
	[SerializeField]
	private Enum m_State;
	[SerializeField]
	private int m_PhaseFrame;
	[SerializeField]
	private int m_StateFrame;
	[SerializeField]
	private Dictionary<Enum, Action> m_ActionDict;

	// フェイズ初期化
	public virtual void InitPhase()
	{
		GetSetState = GetInitState();
		GetSetStateFrame = 0;
	}
	// フェイズ進行中の処理
	public abstract UniTask UpdatePhase();
	// フェイズ終了時の処理
	public virtual void EndPhase()
	{
		GetSetState = GetInitState();
		GetSetPhaseFrame = 0;
	}

	// 最初のフェイズフレームか
	public bool IsFirstPhase()
	{
		return GetSetPhaseFrame == 0;
	}
	// 最初のステートフレームか
	public bool IsFirstState()
	{
		return GetSetStateFrame == 0;
	}

	// 指定のステートへ遷移
	public void SwitchState(Enum nextState)
	{
		GetSetState = nextState;
	}

	// ステート遷移時の処理
	public void OnSwitchState()
	{
		// ステートフレーム初期化
		GetSetStateFrame = 0;
	}

	// 次のステートへ遷移可能な状態か
	protected bool IsSwitchState()
	{
		Test_User playerUser = Test_UserMgr.instance.GetSetPlayerUser;
		Test_User enemyUser = Test_UserMgr.instance.GetSetEnemyUser;

		// ユーザーが取得できていないなら不可能
		if (!playerUser || !enemyUser)
		{
			Debug.LogError($"{nameof(IsSwitchState)}でユーザー取得ができていなかったのでフェイズ遷移を行えませんでした。" +
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

		// 自分と相手のユーザーステートが一致しないので不可能
		if (playerUser.GetSetPhaseState != enemyUser.GetSetPhaseState)
		{
			Debug.LogError($"自分と相手のステートが一致しないのでステート遷移を行えませんでした。通信同期が正しく行えているのか確認をお願いします。" +
						   $"PlayerUser：{playerUser.GetSetPhaseState} || EnemyUser：{enemyUser.GetSetPhaseState}");
			return false;
		}

		// 自分と相手のユーザーステート同期待ちフラグが両方立っていないので不可能
		if (!playerUser.GetSetPhaseStateReadyFlag || !enemyUser.GetSetPhaseStateReadyFlag)
		{
			Debug.LogError($"自分と相手のステート同期待ちフラグが両方立っていないのでステート遷移を行えませんでした。通信同期が正しく行えているのか確認をお願いします。" +
						   $"PlayerUser：{playerUser.GetSetPhaseStateReadyFlag} || EnemyUser：{enemyUser.GetSetPhaseStateReadyFlag}");
			return false;
		}

		Debug.Log($"ステート遷移可能なので{GetSetState}から遷移します。" +
				  $"[Player]PhaseType：{playerUser.GetSetPhaseState}, PhaseReadyFlag：{playerUser.GetSetPhaseStateReadyFlag}" +
				  $"[Enemy]PhaseType：{enemyUser.GetSetPhaseState}, PhaseReadyFlag：{enemyUser.GetSetPhaseStateReadyFlag}");

		// 遷移可能
		return true;
	}

	// フェイズ内で状態に対応する初期状態を取得
	public abstract Enum GetInitState();

	public virtual Enum GetSetState
	{
		get { return m_State; }
		set { m_State = value; }
	}
	public virtual int GetSetPhaseFrame
	{
		get { return m_PhaseFrame; }
		set { m_PhaseFrame = value; }
	}
	public virtual int GetSetStateFrame
	{
		get { return m_StateFrame; }
		set { m_StateFrame = value; }
	}
	public virtual Dictionary<Enum, Action> GetSetActionDict
	{
		get { return m_ActionDict; }
		set { m_ActionDict = value; }
	}
}