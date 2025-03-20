using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
	// インスタンス
	public static ActionManager instance;

	[SerializeReference]
	private List<TestIAction> activeActionList;

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

	public T ActivateAction<T>(params object[] args) where T : TestIAction
	{
		var action = (T)Activator.CreateInstance(typeof(T), args);  // Action を動的に生成
		Debug.Log($"{typeof(T).Name} を実行しました");

		activeActionList.Add(action);									// 追加
		action.OnCompleted += _ => activeActionList.Remove(action);		// 終了時にリストから削除
		action.ExecuteAsync().Forget();									// 非同期実行
		return action;
	}

	public async UniTask WaitForAction(TestIAction action)
	{
		if (action == null)
		{
			Debug.LogWarning($"{nameof(WaitForAction)}の引数(action)でNULLが検知されましたので処理を終了しました。");
			return;
		}

		if (!action.IsRunning)
		{
			var tcs = new UniTaskCompletionSource();
			action.OnCompleted += _ => tcs.TrySetResult();
			await tcs.Task;
		}
	}
}