using Cysharp.Threading.Tasks;
using Photon.Pun;
using static Phase;
using UnityEngine;

public class TestEndPhase : Phase
{
	protected override async UniTask InitState()
	{
		Debug.Log($"{nameof(TestEndPhase)}：{nameof(InitState)}");
		state = eState.Main;
		await UniTask.Yield();
	}

	protected override async UniTask MainState()
	{
		Debug.Log($"{nameof(TestEndPhase)}：{nameof(MainState)}");
		state = eState.End;
		await UniTask.Yield();
	}

	protected override async UniTask EndState()
	{
		Debug.Log($"{nameof(TestEndPhase)}：EndState");
		await UniTask.Yield();
	}
}