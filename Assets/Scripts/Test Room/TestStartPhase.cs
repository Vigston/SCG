using Cysharp.Threading.Tasks;
using Photon.Pun;
using static Phase;
using UnityEngine;

public class TestStartPhase : Phase
{
	protected override async UniTask InitState()
	{
		Debug.Log($"{nameof(TestStartPhase)}：{nameof(InitState)}");
		state = eState.Main;
		await UniTask.Yield();
	}

	protected override async UniTask MainState()
	{
		Debug.Log($"{nameof(TestStartPhase)}：{nameof(MainState)}");
		state = eState.End;
		await UniTask.Yield();
	}

	protected override async UniTask EndState()
	{
		Debug.Log($"{nameof(TestStartPhase)}：EndState");
		await UniTask.Yield();
	}
}