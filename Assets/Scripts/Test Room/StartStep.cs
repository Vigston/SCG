using UnityEngine;
using Cysharp.Threading.Tasks;

public class StartStep : Step
{
	public StartStep(StepManager manager) : base(manager) { }

	public override async UniTask Init()
	{
		Debug.Log("StartStep Init: 準備中...");
		await UniTask.Delay(100);
	}

	public override async UniTask Main()
	{
		Debug.Log("StartStep Main: スタート処理");
		await UniTask.Delay(100);
	}

	public override async UniTask End()
	{
		Debug.Log("StartStep End: 終了処理");
		await UniTask.Delay(100);
	}

	public override Step GetNextStep()
	{
		return new JoinStep(manager);
	}
}