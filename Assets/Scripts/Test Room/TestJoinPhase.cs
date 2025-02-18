using Cysharp.Threading.Tasks;
using Photon.Pun;
using static Phase;
using UnityEngine;

public class TestJoinPhase : Phase
{
	protected override async UniTask InitState()
	{
		Debug.Log($"{nameof(TestJoinPhase)}：{nameof(InitState)}");
		state = eState.Main;
		await UniTask.Yield();
	}

	protected override async UniTask MainState()
	{
		Debug.Log($"{nameof(TestJoinPhase)}：{nameof(MainState)}");
		state = eState.End;
		await UniTask.Yield();
	}

	protected override async UniTask EndState()
	{
		Debug.Log($"{nameof(TestJoinPhase)}：EndState");
		await UniTask.Yield();
	}
}