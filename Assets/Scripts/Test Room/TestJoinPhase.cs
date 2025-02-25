using Cysharp.Threading.Tasks;
using Photon.Pun;
using static Phase;
using UnityEngine;

public class TestJoinPhase : Phase
{
	protected override async UniTask StartState()
	{
		// 基底処理実行
		await base.StartState();
		await UniTask.CompletedTask;
	}

	protected override async UniTask MainState()
	{
		// 基底処理実行
		await base.MainState();
		await UniTask.CompletedTask;
	}

	protected override async UniTask EndState()
	{
		// 基底処理実行
		await base.EndState();
		await UniTask.CompletedTask;
	}
}