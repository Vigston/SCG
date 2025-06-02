using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class DiceRollAnim : IAnimation
{
	private Transform transform;	// アニメーションさせる対象
	private int result;              // サイコロの目の値
	private float duration;         // アニメーションの所要時間

	/// <summary>
	/// AssignCardAnim コンストラクタ
	/// </summary>
	/// <param name="transform">アニメーションさせる対象</param>
	/// <param name="result">サイコロの目の値</param>
	/// <param name="duration">アニメーションの所要時間</param>
	public DiceRollAnim(Transform transform, int result, float duration)
	{
		this.transform = transform;
		this.result = result;
		this.duration = duration;

		Debug.Log($"サイコロ結果：{result}");
	}

	public async UniTask PlayAsync()
	{
		// カメラ取得
		var camera = Camera.main;

		// 開始・終了座標
		Vector3 start = transform.position;
		Vector3 up = start + camera.transform.up * 4f; // カメラから見て上方向
		Vector3 end = start;

		// 乱回転用
		Vector3 randomAxis1 = Random.onUnitSphere;
		Vector3 randomAxis2 = Random.onUnitSphere;

		// サイコロの目ごとの回転（例: 1~6の面がどの向きでカメラを向くかを定義）
		Vector3[] faceNormals = new Vector3[]
		{
			Vector3.back,	// 1
			Vector3.down,	// 2
			Vector3.right,	// 3
			Vector3.left,	// 4
			Vector3.up,		// 5
			Vector3.forward	// 6
		};

		int faceIndex = Mathf.Clamp(result - 1, 0, 5);
		Vector3 targetNormal = faceNormals[faceIndex];

		// カメラの正面方向に「面」を向ける回転
		Quaternion targetRot = Quaternion.FromToRotation(targetNormal, camera.transform.forward) * transform.rotation;

		// 上昇＋乱回転
		var seq = DOTween.Sequence();
		seq.Append(transform.DOMove(up, duration * 0.4f).SetEase(Ease.OutQuart));
		seq.Join(transform.DORotate(Random.insideUnitSphere * 720f, duration * 0.4f, RotateMode.FastBeyond360));

		// 落下＋目の方向に回転
		seq.Append(transform.DOMove(end, duration * 0.6f).SetEase(Ease.InQuart));
		seq.Join(transform.DORotateQuaternion(targetRot, duration * 0.6f).SetEase(Ease.OutQuart));

		await seq.AsyncWaitForCompletion();

		// 1秒停止
		await UniTask.Delay(1000);
	}
}