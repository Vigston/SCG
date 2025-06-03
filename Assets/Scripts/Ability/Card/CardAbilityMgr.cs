using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;

public class CardAbilityMgr : MonoBehaviour
{
	// カード効果の種類を列挙(新しく作ったらここに追加してください)
	public enum AbilityType
	{
		DamageCardAbility,
		GainResourceCardAbility
	}

	// シングルトンインスタンス
	public static CardAbilityMgr instance;

	[SerializeReference]
	private List<CardAbilityBase> activeAbilityList; // 現在実行中のカード効果リスト

	private Queue<CardAbilityBase> abilityQueue = new Queue<CardAbilityBase>(); // 実行待ちのカード効果キュー
	private bool isExecuting = false; // 現在実行中かどうかのフラグ

	private Dictionary<Type, Action<object[]>> m_AbilityTypeActDict;

	private void Awake()
	{
		// インスタンスを生成
		CreateInstance();

		// ステートとアクションを辞書型で紐づける
		GetSetAbilityTypeActDict = new Dictionary<Type, Action<object[]>>()
		{
			{ typeof(DamageCardAbility), Activate_DamageCardAbility },
			{ typeof(GainResourceCardAbility), Activate_GainResourceCardAbility },
		};
	}

	private void Update()
	{
		// キューにアビリティが存在し、現在実行中でない場合に次のアビリティを実行
		if (abilityQueue.Count > 0 && !IsExecuting())
		{
			ExecuteNextAbility().Forget();
		}
	}

	// インスタンスを作成（シングルトンパターン）
	public bool CreateInstance()
	{
		// 既にインスタンスが存在しない場合にのみ作成
		if (!instance)
		{
			instance = this;
		}

		// インスタンスが作成されているか確認
		if (instance) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	// カード効果を実行
	public T ActivateAbility<T>(params object[] args) where T : CardAbilityBase
	{
		for (int i = 0; i < args.Length; i++)
		{
			Debug.Log($"{nameof(ActivateAbility)}" +
					  $"args[{i}]: {args[i]}, Type: {args[i]?.GetType()}");
		}

		Type abilityType = typeof(T);
		// 指定されたタイプのカード効果を動的に生成
		var ability = (T)Activator.CreateInstance(abilityType, args);
		Debug.Log($"{abilityType.Name} を発動しました");

		// 実行待ちのカード効果キューに追加
		abilityQueue.Enqueue(ability);

		NetWorkMgr NetWorkMgr = NetWorkMgr.instance;
		string abilityType_str = abilityType.AssemblyQualifiedName;

		// シングルプレイデバッグモードならRPCを送信しない
		if (!DebugMgr.Instance.isSingleDebug)
		{
			NetWorkMgr.photonView.RPC(nameof(NetWorkMgr.RPC_ActivateAbility_Other), RpcTarget.OthersBuffered, abilityType_str, args);
		}

		return ability;
	}

	// 次のカード効果を実行
	private async UniTask ExecuteNextAbility()
	{
		// 実行中フラグを立てる
		isExecuting = true;

		// キューから次のアビリティを取り出す
		var ability = abilityQueue.Dequeue();

		// 実行中のカード効果リストに追加
		activeAbilityList.Add(ability);

		// カード効果が完了したらリストから削除
		ability.OnCompleted += _ => activeAbilityList.Remove(ability);

		// カード効果を実行
		await ability.ExecuteAsync();

		// 実行中フラグを下ろす
		isExecuting = false;
	}

	// 指定されたカード効果が完了するまで待機
	public async UniTask WaitForAbility(CardAbilityBase ability)
	{
		// カードアビリティが生成されるまで待機
		await UniTask.WaitUntil(() => ability != null);

		// 既に完了しているならはじく
		if (!ability.IsRunning) return;

		// 完了を待つための非同期タスクを作成
		var tcs = new UniTaskCompletionSource();

		// カード効果の完了時にタスクを完了させる
		ability.OnCompleted += _ => tcs.TrySetResult();

		// カード効果が終了するまで待機
		await tcs.Task;
	}

	// キューが空かどうかを確認
	public bool IsQueueEmpty()
	{
		return abilityQueue.Count == 0;
	}

	// 現在実行中かどうかを確認
	public bool IsExecuting()
	{
		return isExecuting;
	}

	///////////////////////////////////////////
	///// AbilityTypeからアクションを実行 /////
	///////////////////////////////////////////
	public void Activate_DamageCardAbility(object[] args)
	{
		// object型からint型に変換
		int id		= Convert.ToInt32(args[0]);
		int damage	= Convert.ToInt32(args[1]);
		ActivateAbility<DamageCardAbility>(id, damage);
	}
	public void Activate_GainResourceCardAbility(object[] args)
	{
		// object型からint型に変換
		int id			= Convert.ToInt32(args[0]);
		int gainGold	= Convert.ToInt32(args[1]);
		ActivateAbility<GainResourceCardAbility>(id, gainGold);
	}

	public Dictionary<Type, Action<object[]>> GetSetAbilityTypeActDict
	{
		get { return m_AbilityTypeActDict; }
		set { m_AbilityTypeActDict = value; }
	}
}
