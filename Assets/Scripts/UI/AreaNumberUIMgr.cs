using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AreaNumberUIMgr : MonoBehaviour
{
	public static AreaNumberUIMgr Instance;

	public Camera targetCamera;
	public GameObject areaNumberParent;
	public GameObject labelPrefab;

	private RectTransform[] uiRects;

	private void Awake()
	{
		// インスタンスを作成
		CreateInstance();
	}

	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (Instance == null)
		{
			// 作成
			Instance = this;
		}

		// インスタンスが作成済みなら終了
		if (Instance != null) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	// エディターから実行用
	public void GenerateUIFromEditor()
	{
		StageMgr stageMgr = FindFirstObjectByType<StageMgr>();

		var cardAreaList = stageMgr.GetSetCardAreaList;

		if (cardAreaList == null)
		{
			Debug.Log($"{nameof(cardAreaList)}がNULLです");
		}

		ClearUI(); // 先に消す
		uiRects = new RectTransform[cardAreaList.Count];

		for (int i = 0; i < cardAreaList.Count; i++)
		{
			GameObject obj = Instantiate(labelPrefab, areaNumberParent.transform);
			obj.name = $"AreaNumber_{i + 1}";

			var text = obj.GetComponentInChildren<TextMeshProUGUI>();
			if (text != null)
				text.text = (i + 1).ToString();

			var rect = obj.GetComponent<RectTransform>();
			uiRects[i] = rect;

			// === カードエリア位置に合わせる ===

			RectTransform canvasRect = areaNumberParent.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

			Vector3 worldPos = cardAreaList[i].transform.position + Vector3.up * 0.5f;
			Vector3 screenPos = targetCamera.WorldToScreenPoint(worldPos);

			Debug.Log($"worldPos：{worldPos}");
			Debug.Log($"screenPos：{screenPos}");

			// Canvas空間のローカル位置に変換
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				canvasRect,
				screenPos,
				targetCamera,
				out Vector2 localPos))
			{
				uiRects[i].anchoredPosition = localPos;
			}
		}
	}

	public void ClearUI()
	{
		// 子をすべて削除（Undo登録付き）
		for (int i = areaNumberParent.transform.childCount - 1; i >= 0; i--)
		{
			GameObject.DestroyImmediate(areaNumberParent.transform.GetChild(i).gameObject);
		}
	}
}