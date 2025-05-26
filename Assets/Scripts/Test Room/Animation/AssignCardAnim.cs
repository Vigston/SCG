using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class AssignCardAnim : IAnimation
{
	private Transform transform;
	private Transform target;
	private float ellipseA, ellipseB, duration;
	private int resolution;

	public AssignCardAnim(Transform transform, Transform target, float ellipseA, float ellipseB, float duration, int resolution = 50)
	{
		this.transform = transform;
		this.target = target;
		this.ellipseA = ellipseA;
		this.ellipseB = ellipseB;
		this.duration = duration;
		this.resolution = resolution;
	}

	public async UniTask PlayAsync()
	{
		Vector3 start = transform.position;
		Vector3 end = target.position;
		Vector3 center = (start + end) / 2f;

		Vector3 dir = (end - start).normalized;
		float angle = Mathf.Atan2(dir.y, dir.x);
		Quaternion rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

		Vector3[] path = new Vector3[resolution + 1];
		for (int i = 0; i <= resolution; i++)
		{
			float t = (float)i / resolution;
			float theta = Mathf.Lerp(Mathf.PI, 0, t);
			float x = Mathf.Cos(theta) * ellipseA;
			float y = Mathf.Sin(theta) * ellipseB;

			Vector3 localPos = new Vector3(x, y, 0);
			path[i] = center + rotation * localPos;
		}

		var tcs = new UniTaskCompletionSource();

		transform.DOPath(path, duration, PathType.CatmullRom)
			.SetEase(Ease.InOutSine)
			.OnComplete(() => tcs.TrySetResult());

		await tcs.Task;
	}
}