using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public abstract class Phase : MonoBehaviour
{
	[SerializeField]
	private Enum m_State;
	[SerializeField]
	private string m_StateName;
	[SerializeField]
	private int m_PhaseFrame;
	[SerializeField]
	private int m_StateFrame;
	[SerializeField]
	private Dictionary<Enum, Action> m_ActionDict;

	private void Update()
	{
		if (GetSetState == null) return;

		m_StateName = GetSetState.ToString();
	}

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
		GetSetStateFrame = 0;
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
	public void SwitchState(Enum _nextState)
	{
		Debug.Log($"{nameof(SwitchState)}：{_nextState.ToString()}");
		GetSetState = _nextState;

		// Phase終了なら他クライアントに通知
		if(GetEndState().Equals(_nextState))
		{
			Debug.Log($"{nameof(SwitchState)}：{GetEndState().ToString()} == {_nextState.ToString()}");

			PhaseManager phaseManager = PhaseManager.instance;
			Test_NetWorkMgr netWorkMgr = Test_NetWorkMgr.instance;

			int phaseIdx = (int)phaseManager.GetSetPhaseType;
			int nextState = (int)(object)_nextState;

			netWorkMgr.photonView.RPC(nameof(netWorkMgr.RPC_EndPhase_MC), RpcTarget.OthersBuffered, phaseIdx, nextState);
		}
	}

	// ステート遷移時の処理
	public void OnSwitchState()
	{
		// ステートフレーム初期化
		GetSetStateFrame = 0;
	}

	// フェイズ内で状態に対応する初期状態を取得
	public abstract Enum GetInitState();

	// フェイズ内で状態に対応する終了状態を取得
	public abstract Enum GetEndState();

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