using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
	// インスタンス
	public static ActionManager instance;

	private void Awake()
	{
		// インスタンス生成
		CreateInstance();
	}

	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (!instance)
		{
			// 作成
			instance = this;
		}

		// インスタンスが作成済みなら終了
		if (instance) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	public ActionBase ActivateAction<T>(params object[] args) where T : ActionBase
	{
		var action = (T)Activator.CreateInstance(typeof(T), args);  // Action を動的に生成
		Debug.Log($"{typeof(T).Name} を実行しました");

		action.ExecuteAsync().Forget();  // 非同期実行
		return action;
	}

	public async UniTask WaitForAction(ActionBase action)
	{
		if (!action.IsCompleted)
		{
			var tcs = new UniTaskCompletionSource();
			action.OnCompleted += _ => tcs.TrySetResult();
			await tcs.Task;
		}
	}
}