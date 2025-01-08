using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ActionMgr : MonoBehaviour
{
	// インスタンス
	public static ActionMgr instance;

	private bool isExecuting = false;

	public Queue<IAction> actionQueue = new Queue<IAction>();
	private CancellationTokenSource cancellationTokenSource;

	// 直前に処理を行ったアクションを保持(IsCompletedActionで終了を検知したら削除)
	public Queue<IAction> bufferActionQueue = new Queue<IAction>();

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
		_action.OnActionCompleted += OnActionCompleted;
	}

	// 指定のアクションが終了しているか(終了しているならここでキューから削除)
	public bool IsCompletedAction(IAction _action)
	{
		IAction bufferAction = null;

		if (bufferActionQueue.Contains(_action))
		{
			bufferAction = bufferActionQueue.Dequeue();
		}

		if(bufferAction == null)
		{
			return false;
		}

		return bufferAction == _action;
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
		Debug.Log("ExecuteActions実行開始");
		if (isExecuting) return;

		isExecuting = true;
		cancellationTokenSource = new CancellationTokenSource();

		while (true)
		{
			// アクションがキューに存在していなければ、処理を一旦待機
			if (actionQueue.Count <= 0)
			{
				// キューにアクションがない場合、1フレーム待機してから次を確認
				await UniTask.Yield();
				continue;
			}

			var action = actionQueue.Dequeue();

			try
			{
				Debug.Log($"action.Execute：{action}");
				await action.Execute(cancellationTokenSource.Token);

				bufferActionQueue.Enqueue(action);
			}
			catch (OperationCanceledException)
			{
				Debug.Log("An action was canceled.");
				break;
			}
		}

		cancellationTokenSource.Dispose();
		cancellationTokenSource = null;

		Debug.Log("ExecuteActions実行終了");
	}

	// アクション完了時の処理
	private void OnActionCompleted(IAction _action)
	{
		Debug.Log($"アクション完了：{_action}");
	}
}
