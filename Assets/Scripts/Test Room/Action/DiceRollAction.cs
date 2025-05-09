using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class DiceRollAction : ActionBase
{
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

		AssetManager.Load<GameObject>("Dice_D6_White", prefab =>
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
		});

		//Debug.Log($"{this.ToString()} {nameof(Execute)}");
		//Debug.Log($"サイコロの目: {diceValue}");

		await tcs.Task; // AssetManager.Load の完了を待つ

		foreach (Transform child in dice.transform)
		{
			diceNumObjList.Add(child.gameObject);
		}

		Debug.Log("サイコロを振りました！！");

		foreach(GameObject diceNumObj in diceNumObjList)
		{
			Debug.Log($"{diceNumObj.name}");
		}

		// サイコロが停止するまで待機
		await WaitUntilDiceStops(rb);

		float[] _dicePosY = new float[6];

		for (int i = 0; i < 6; i++)
		{
			//_diseNumber[i]は１～６の判定用からオブジェクト
			_dicePosY[i] = diceNumObjList[i].transform.position.y;
			await UniTask.Yield();
		}

		float maxPos = _dicePosY.Max();
		for (int i = 0; i < 6; i++)
		{
			if (diceNumObjList[i].transform.position.y == maxPos)
			{
				diceResultNum = i + 1;
				break;
			}
			await UniTask.Yield();
		}

		Debug.Log($"サイコロの目：{diceResultNum}");

		// サイコロを削除
		if (dice != null)
		{
			GameObject.Destroy(dice);
			Debug.Log("サイコロを削除しました！");

			// アセットを解放
			AssetManager.Release(dicePrefab);
			Debug.Log("サイコロのアセットを解放しました！");
		}
	}

	// サイコロが停止するまで待つ処理
	private async UniTask WaitUntilDiceStops(Rigidbody rb)
	{
		// Rigidbodyがnullの場合は処理を終了
		if (rb == null)
		{
			//Debug.LogWarning("Rigidbodyがnullです。");
			await UniTask.Yield();
		}

		// 速度が十分に遅くなるまでループ
		while (!rb.IsSleeping())
		{
			await UniTask.Yield();  // 非同期で次のフレームまで待つ
		}

		Debug.Log("サイコロが停止しました！");
	}
}
