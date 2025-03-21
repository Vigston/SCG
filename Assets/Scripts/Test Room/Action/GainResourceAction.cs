using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GainResourceAction : ActionBase
{
	private string resourceType;

	public GainResourceAction(string resourceType)
	{
		this.resourceType = resourceType;
	}

	protected override async UniTask Execute()
	{
		Debug.Log($"{resourceType} を獲得！");
		await UniTask.Delay(1000); // 処理のシミュレーション
	}
}