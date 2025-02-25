using Cysharp.Threading.Tasks;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Phase : MonoBehaviourPunCallbacks
{
	public enum eState
	{
		Start,
		Main,
		End,
		
		// 例外状態
		None = -1,
	}

	// フェイズステート
	[SerializeField, ReadOnly]
	protected eState m_State = eState.None;

	// ステートフレーム
	[SerializeField, ReadOnly]
	protected int m_StateFrame = 0;

	// フェイズ処理
	public async UniTask RunPhase()
	{
		while (GetSetState != eState.End)
		{
			// ステートフレーム数初期化。
			GetSetStateFrame = 0;

			switch (GetSetState)
			{
				case eState.Start:
					await StartState();
					break;
				case eState.Main:
					await MainState();
					break;
				case eState.End:
					await EndState();
					break;
			}

			// ステートフレーム数加算。
			GetSetStateFrame++;
			await UniTask.Yield();
		}
	}

	// フェイズ初期化
	public virtual void InitPhase()
	{
		GetSetState = eState.Start;
	}

	// 各ステート処理
	protected virtual async UniTask StartState()
	{
		Debug.Log($"{this.GetType().Name} StartState");
		GetSetState = eState.Main;
		await UniTask.CompletedTask;
	}
	protected virtual async UniTask MainState()
	{
		Debug.Log($"{this.GetType().Name} MainState");
		GetSetState = eState.End;
		await UniTask.CompletedTask;
	}
	protected virtual async UniTask EndState()
	{
		Debug.Log($"{this.GetType().Name} EndState");
		GetSetState = eState.None;
		await UniTask.CompletedTask;
	}


	// == プロパティ == //
	// フェイズステート
	protected eState GetSetState
	{
		get { return m_State; }
		set { m_State = value; }
	}
	// ステートフレーム
	protected int GetSetStateFrame
	{
		get { return m_StateFrame; }
		set { m_StateFrame = value; }
	}
}