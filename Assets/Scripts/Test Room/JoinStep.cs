using UnityEngine;
using Cysharp.Threading.Tasks;

public class JoinStep : Step
{
	public JoinStep(StepManager manager) : base(manager) { }

	public override async UniTask Init()
	{
		Debug.Log("JoinStep Init: 参加者確認");
		await UniTask.Delay(100);
	}

	public override async UniTask Main()
	{
		Debug.Log("JoinStep Main: 参加受付中...");
		await UniTask.Delay(100);
	}

	public override async UniTask End()
	{
		Debug.Log("JoinStep End: 参加受付終了");
		await UniTask.Delay(100);
	}

	public override Step GetNextStep()
	{
		return new MainStep(manager);
	}
}