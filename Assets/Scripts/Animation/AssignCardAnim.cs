using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class AssignCardAnim : IAnimation
{
	private Transform transform;	// アニメーションさせる対象
	private Transform target;		// 移動先
	private float axisRadius;		// 楕円の半径
	private float duration;			// アニメーションの所要時間
	private int resolution;			// 軌道を構成する点の数

	/// <summary>
	/// AssignCardAnim コンストラクタ
	/// </summary>
	/// <param name="transform">アニメーションさせる対象</param>
	/// <param name="target">移動先</param>
	/// <param name="majorAxisRadius">楕円の長半径</param>
	/// <param name="axisRadius">楕円の短半径</param>
	/// <param name="duration">アニメーションの所要時間</param>
	/// <param name="resolution">軌道を構成する点の数</param>
	public AssignCardAnim(Transform transform, Transform target, float axisRadius, float duration, int resolution = 50)
	{
		this.transform = transform;
		this.target = target;
		this.axisRadius = axisRadius;
		this.duration = duration;
		this.resolution = resolution;
	}

	public async UniTask PlayAsync()
	{
		// 開始・終了・中心座標の計算
		Vector3 start = transform.position;
		Vector3 end = target.position;
		Vector3 center = (start + end) / 2f;

		// 長軸方向
		Vector3 major = (end - start).normalized;
		// 短軸方向
		Vector3 minor = Vector3.Cross(major, Vector3.forward).normalized;

		float halfLength = (end - start).magnitude / 2f;

		// 楕円の軌道を構成する点の計算
		Vector3[] path = new Vector3[resolution + 1];
		for (int i = 0; i <= resolution; i++)
		{
			float t = (float)i / resolution;
			float theta = Mathf.PI - t * Mathf.PI; // θ=π→0
			Vector3 local = major * (Mathf.Cos(theta) * halfLength) + minor * (Mathf.Sin(theta) * axisRadius);
			path[i] = center + local;
		}

		// アニメーションの実行
		var tcs = new UniTaskCompletionSource();
		transform.DOPath(path, duration, PathType.Linear)
			.SetEase(Ease.InOutSine)
			.OnComplete(() => tcs.TrySetResult());

		await tcs.Task;
	}
}