using UnityEngine;
using UnityEngine.UI;

public class WorldToCanvasTester : MonoBehaviour
{
	[Header("=== 設定 ===")]
	public Camera targetCamera;              // UI用カメラ
	public Transform targetWorldObject;      // 位置を確認したい3Dオブジェクト
	public Canvas canvas;                    // 対象のCanvas（Canvas自身！）

	[Header("=== 表示用UI（任意） ===")]
	public RectTransform uiLabel;           // 実際に動かすUI（例：Textなど）
	public Text debugText;                  // 座標表示用テキスト（任意）

	// 計算用 //
	private Vector2 canvasSize;
	private float scaleX;
	private float scaleY;

	void Start()
	{
		if (targetCamera == null || targetWorldObject == null || canvas == null)
		{
			Debug.LogWarning("必要な参照が足りません");
			return;
		}

		canvasSize = canvas.GetComponent<RectTransform>().rect.size; // ← (1080, 1920)になるはず

		scaleX = canvasSize.x / Screen.width;
		scaleY = canvasSize.y / Screen.height;
	}

	void Update()
	{
		// World → Screen
		Vector3 screenPos = targetCamera.WorldToScreenPoint(targetWorldObject.position);

		var canvasLocalPos = new Vector2(
			screenPos.x - (Screen.width / 2f),
			screenPos.y - (Screen.height / 2f)
		);

		var anchoredPos = new Vector2(
			canvasLocalPos.x * scaleX,
			canvasLocalPos.y * scaleY
		);

		if (uiLabel.anchoredPosition == anchoredPos) return;

		Debug.Log($"[targetObject] position：{targetWorldObject.position}");
		Debug.Log($"[targetObject] screenPos：{screenPos}");

		Debug.Log($"Before [Text] anchoredPosition：{uiLabel.anchoredPosition}");

		uiLabel.anchoredPosition = anchoredPos;

		Debug.Log($"After [Text] anchoredPosition：{uiLabel.anchoredPosition}");
	}
}