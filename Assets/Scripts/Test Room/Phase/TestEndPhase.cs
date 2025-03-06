using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestEndPhase : Phase
{
	protected override async UniTask StartState()
	{
		// 基底処理実行
		await base.StartState();
		// メインステートへ
		SwitchState(eState.Main);
		await UniTask.CompletedTask;
	}

	protected override async UniTask MainState()
	{
		// 基底処理実行
		await base.MainState();
		// 左シフト+Pでフェイズ移行
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
		{
			// 終了ステートへ
			SwitchState(eState.End);
		}
		await UniTask.CompletedTask;
	}

	protected override async UniTask EndState()
	{
		// 基底処理実行
		await base.EndState();
		await UniTask.CompletedTask;
	}
}