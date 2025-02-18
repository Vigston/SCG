using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public abstract class Phase : MonoBehaviourPunCallbacks
{
	public enum eState
	{
		Init,
		Main,
		End
	}

	protected eState state = eState.Init;

	public async UniTask RunPhase()
	{
		while (state != eState.End)
		{
			switch (state)
			{
				case eState.Init:
					await InitState();
					break;
				case eState.Main:
					await MainState();
					break;
				case eState.End:
					await EndState();
					break;
			}
			await UniTask.Yield();
		}
	}

	protected virtual async UniTask InitState()
	{
		Debug.Log($"{this.GetType().Name} InitState");
		state = eState.Main;
		await UniTask.Yield();
	}

	protected virtual async UniTask MainState()
	{
		Debug.Log($"{this.GetType().Name} MainState");
		state = eState.End;
		await UniTask.Yield();
	}

	protected virtual async UniTask EndState()
	{
		Debug.Log($"{this.GetType().Name} EndState");
		await UniTask.Yield();
	}
}