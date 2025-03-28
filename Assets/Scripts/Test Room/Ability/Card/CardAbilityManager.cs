using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;

public class CardAbilityManager : MonoBehaviour
{
	// シングルトンインスタンス
	public static CardAbilityManager instance;

	[SerializeReference]
	private List<CardAbilityBase> activeAbilityList; // 現在実行中のカード効果リスト

	private void Awake()
	{
		// インスタンスを生成
		CreateInstance();
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
		// 指定されたタイプのカード効果を動的に生成
		var ability = (T)Activator.CreateInstance(typeof(T), args);
		Debug.Log($"{typeof(T).Name} を発動しました");

		// 実行中のカード効果リストに追加
		activeAbilityList.Add(ability);

		// カード効果が完了したらリストから削除
		ability.OnCompleted += _ => activeAbilityList.Remove(ability);

		// 非同期でカード効果を実行（例外を投げずに処理を忘れる）
		ability.ExecuteAsync().Forget();

		var netWorkMgr = Test_NetWorkMgr.instance;
		netWorkMgr.photonView.RPC(nameof(netWorkMgr.RPC_ActivateAbility_Other), RpcTarget.OthersBuffered, PhotonDataConverter.Serialize_ObjTypeToByte(typeof(T)));

		return ability;
	}

	// 指定されたカード効果が完了するまで待機
	public async UniTask WaitForAbility(CardAbilityBase ability)
	{
		// nullチェック（無効な能力の場合は警告を出して処理を終了）
		if (ability == null)
		{
			Debug.LogWarning($"{nameof(WaitForAbility)}の引数(ability)でNULLが検知されましたので処理を終了しました。");
			return;
		}

		// 既に完了しているならはじく
		if (!ability.IsRunning) return;

		// 完了を待つための非同期タスクを作成
		var tcs = new UniTaskCompletionSource();

		// カード効果の完了時にタスクを完了させる
		ability.OnCompleted += _ => tcs.TrySetResult();

		// カード効果が終了するまで待機
		await tcs.Task;
	}
}
