using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Step
{
	protected StepManager manager;

	public Step(StepManager manager)
	{
		this.manager = manager;
	}

	public virtual async UniTask Init()
	{
		Debug.Log($"[{this.GetType().Name}] Init");
		await UniTask.Yield();
	}

	public virtual async UniTask Main()
	{
		Debug.Log($"[{this.GetType().Name}] Main");
		await UniTask.Yield();
	}

	public virtual async UniTask End()
	{
		Debug.Log($"[{this.GetType().Name}] End");
		await UniTask.Yield();
	}

	public abstract Step GetNextStep();
}