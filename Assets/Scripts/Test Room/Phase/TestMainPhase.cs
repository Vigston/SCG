using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class TestMainPhase : Phase
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

		var cardAbilityManager = CardAbilityManager.instance;
		ICardAbility cardAbility = null;

		// 左シフト+Aでアビリティ発動
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A))
		{
			// ダメージアビリティ発動
			cardAbility = cardAbilityManager.ActivateAbility<DamageCardAbility>(10);
			Debug.Log($"{cardAbility}：カードアビリティ発動！！");
		}

		await cardAbilityManager.WaitForAbility(cardAbility);

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
		// ターン終了
		SwitchState(eState.None);
		await UniTask.CompletedTask;
	}
}