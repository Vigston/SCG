using Cysharp.Threading.Tasks;
using UnityEngine;

public class EndStep : Step
{
	public EndStep(StepManager manager) : base(manager) { }

	public override async UniTask Init()
	{
		Debug.Log("EndStep Init: リザルト準備");
		await UniTask.Delay(100);
	}

	public override async UniTask Main()
	{
		Debug.Log("EndStep Main: リザルト表示中...");
		await UniTask.Delay(100);
	}

	public override async UniTask End()
	{
		Debug.Log("EndStep End: ステップリセット");
		await UniTask.Delay(100);
	}

	public override Step GetNextStep()
	{
		return new StartStep(manager);
	}
}