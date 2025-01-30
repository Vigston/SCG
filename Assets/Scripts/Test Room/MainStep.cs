using UnityEngine;
using Cysharp.Threading.Tasks;

public class MainStep : Step
{
	public MainStep(StepManager manager) : base(manager) { }

	public override async UniTask Init()
	{
		Debug.Log("MainStep Init: ゲーム準備");
		await UniTask.Delay(100);
	}

	public override async UniTask Main()
	{
		Debug.Log("MainStep Main: ゲームプレイ中...");
		await UniTask.Delay(100);
	}

	public override async UniTask End()
	{
		Debug.Log("MainStep End: ゲーム終了処理");
		await UniTask.Delay(100);
	}

	public override Step GetNextStep()
	{
		return new EndStep(manager);
	}
}