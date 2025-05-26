using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class DiceRollAction : ActionBase
{
	private const int DICE_FACE_COUNT = 6; // サイコロの目の数

	private int diceResultNum;

	public DiceRollAction()
	{
	}
	protected override async UniTask Execute()
	{
		List<GameObject> diceNumObjList = new List<GameObject>();

		GameObject dice = null;
		Rigidbody rb = null;

		// プレハブの参照を保持する変数
		GameObject dicePrefab = null;

		var tcs = new UniTaskCompletionSource();

		AssetManager.Load<GameObject>("Dice_D6_White",
		prefab =>
		{
			dicePrefab = prefab;
			dice = GameObject.Instantiate(prefab) as GameObject;
			dice.transform.position = new Vector3(8, 8, 0);

			rb = dice.GetComponent<Rigidbody>();

			// 前方斜め下への力（控えめ）
			Vector3 throwDirection = new Vector3(0f, 0f, 0f).normalized;
			rb.AddForce(throwDirection * 100f, ForceMode.Impulse); // パワーを調整

			// ランダムな回転力
			Vector3 randomTorque = new Vector3(
				Random.Range(-1f, 1f),
				Random.Range(-1f, 1f),
				Random.Range(-1f, 1f)
			).normalized * 50f;
			rb.AddTorque(randomTorque, ForceMode.Impulse);

			tcs.TrySetResult(); // 非同期処理完了を通知
		},
		error =>
		{
			Debug.LogError($"Dice prefab load failed: {error}");
			tcs.TrySetException(error);
		});

		await tcs.Task; // AssetManager.Load の完了を待つ

		// サイコロのNULLチェック
		if (dice == null && rb == null)
		{
			Debug.LogError("サイコロの生成に失敗しました。");
			return;
		}

		foreach (Transform child in dice.transform)
		{
			diceNumObjList.Add(child.gameObject);
		}

		Debug.Log("サイコロを振りました！！");

		// サイコロが停止するまで待機
		await WaitUntilDiceStops(rb);

		float[] _dicePosY = new float[DICE_FACE_COUNT];

		for (int i = 0; i < DICE_FACE_COUNT; i++)
		{
			var diceNumObj = diceNumObjList[i]; // サイコロの目のオブジェクト

			if(!diceNumObj)
			{
				Debug.LogError($"サイコロ目[{i + 1}]オブジェクトが見つかりませんでした。");
				continue;
			}

			//_diseNumber[i]は１～６の判定用からオブジェクト
			_dicePosY[i] = diceNumObjList[i].transform.position.y;
		}

		float maxPos = _dicePosY.Max();
		for (int i = 0; i < DICE_FACE_COUNT; i++)
		{
			var diceNumObj = diceNumObjList[i]; // サイコロの目のオブジェクト

			if(!diceNumObj)
			{
				Debug.LogError($"サイコロ目[{i + 1}]オブジェクトが見つかりませんでした。");
				continue;
			}

			if (diceNumObj.transform.position.y == maxPos)
			{
				diceResultNum = i + 1;
				break;
			}
		}

		// 範囲外の場合はエラー
		if (diceResultNum <= 0 || diceResultNum > DICE_FACE_COUNT)
		{
			Debug.LogError("サイコロの目の判定に失敗しました。");
			return;
		}

		Debug.Log($"サイコロの目：{diceResultNum}");

		// サイコロを削除、アセット解放
		if (dice != null)
		{
			GameObject.Destroy(dice);           // 削除
			AssetManager.Release(dicePrefab);   // 解放
		}

		// 2秒待機（サイコロの目を確認するための時間）
		await UniTask.Delay(2000);
	}

	// サイコロが停止するまで待つ処理
	private async UniTask WaitUntilDiceStops(Rigidbody rb)
	{
		// 速度が十分に遅くなるまでループ(ゲームオブジェクトが存在するか毎回チェック)
		while (rb != null && rb.gameObject != null && !rb.IsSleeping())
		{
			await UniTask.Yield();  // 非同期で次のフレームまで待つ
		}

		Debug.Log("サイコロが停止しました！");
	}
}
