using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DamageAction : ActionBase
{
	private int damageAmount;

	public DamageAction(int damageAmount)
	{
		this.damageAmount = damageAmount;
	}

	protected override async UniTask Execute()
	{
		Debug.Log($"{this.ToString()} {nameof(Execute)}");
		Debug.Log($"相手に {damageAmount} ダメージ！");
		await UniTask.Delay(1000); // 処理のシミュレーション
	}
}