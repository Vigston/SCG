using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using static CardAbilityManager;

public class CardAbilityManager : MonoBehaviour
{
	// カード効果の種類を列挙(新しく作ったらここに追加してください)
	public enum AbilityType
	{
		DamageCardAbility,
		GainResourceCardAbility
	}

	// シングルトンインスタンス
	public static CardAbilityManager instance;

	[SerializeReference]
	private List<CardAbilityBase> activeAbilityList; // 現在実行中のカード効果リスト

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

		// 実行中のカード効果リストに追加
		activeAbilityList.Add(ability);

		// カード効果が完了したらリストから削除
		ability.OnCompleted += _ => activeAbilityList.Remove(ability);

		// 非同期でカード効果を実行（例外を投げずに処理を忘れる）
		ability.ExecuteAsync().Forget();

		Test_NetWorkMgr test_NetWorkMgr = Test_NetWorkMgr.instance;
		string abilityType_str = abilityType.AssemblyQualifiedName;
		test_NetWorkMgr.photonView.RPC(nameof(test_NetWorkMgr.RPC_ActivateAbility_Other), RpcTarget.OthersBuffered, abilityType_str, args);

		return ability;
	}

	// 指定されたカード効果が完了するまで待機
	public async UniTask WaitForAbility(CardAbilityBase ability)
	{
		// カードアビリティが生成されるまで待機
		await UniTask.WaitUntil(() => ability != null);
		//// nullチェック（無効な能力の場合は警告を出して処理を終了）
		//if (ability == null)
		//{
		//	Debug.LogWarning($"{nameof(WaitForAbility)}の引数(ability)でNULLが検知されましたので処理を終了しました。");
		//	return;
		//}

		// 既に完了しているならはじく
		if (!ability.IsRunning) return;

		// 完了を待つための非同期タスクを作成
		var tcs = new UniTaskCompletionSource();

		// カード効果の完了時にタスクを完了させる
		ability.OnCompleted += _ => tcs.TrySetResult();

		// カード効果が終了するまで待機
		await tcs.Task;
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
