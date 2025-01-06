using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ActionMgr : MonoBehaviour
{
	// インスタンス
	public static ActionMgr instance;

	private Queue<IAction> actionQueue = new Queue<IAction>();
	private CancellationTokenSource cancellationTokenSource;
	private bool isExecuting = false;

	// イベント
	public event Action<IAction> OnActionCompleted;

	private void Awake()
	{
		// インスタンス生成
		CreateInstance();
	}

	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (instance == null)
		{
			// 作成
			instance = this;
		}

		// インスタンスが作成済みなら終了
		if (instance != null) { return true; }

		Debug.LogError("ActionMgrのインスタンスが生成できませんでした");
		return false;
	}

	// アクション追加
	public void AddAction(IAction _action)
	{
		actionQueue.Enqueue(_action);

		// アクションが追加されるたびに完了通知を監視
		OnActionCompleted += OnActionCompleted;
	}

	// 指定のアクションが終了しているか
	public bool IsCompletedAction(IAction _action)
	{
		return !actionQueue.Contains(_action);
	}

	// 全てのアクションをキャンセル
	public void CancelAllActions()
	{
		if (cancellationTokenSource != null)
		{
			cancellationTokenSource.Cancel();
		}
	}

	public async UniTask ExecuteActions()
	{
		if (isExecuting) return;

		isExecuting = true;
		cancellationTokenSource = new CancellationTokenSource();

		while (actionQueue.Count > 0)
		{
			var action = actionQueue.Dequeue();

			try
			{
				await action.Execute(cancellationTokenSource.Token);

				// アクションが完了したらここで通知
				OnActionCompleted?.Invoke(action); // アクション完了の通知
			}
			catch (OperationCanceledException)
			{
				Debug.Log("An action was canceled.");
				break;
			}
		}

		isExecuting = false;
		cancellationTokenSource.Dispose();
		cancellationTokenSource = null;
	}
}
