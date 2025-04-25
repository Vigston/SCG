using UnityEngine;
using System.Collections.Generic;

public class DebugLogMgr : MonoBehaviour
{
	private static DebugLogMgr instance;
	private List<string> logMessages = new List<string>();
	private Vector2 scrollPosition = Vector2.zero;

	private GUIStyle backgroundStyle;
	private Texture2D backgroundTexture;

	private Vector2 lastDragPos;
	private bool isDragging = false;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		backgroundTexture = new Texture2D(1, 1);
		backgroundTexture.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 0.8f));
		backgroundTexture.Apply();

		backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = backgroundTexture;
		backgroundStyle.padding = new RectOffset(10, 10, 10, 10);
	}

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type)
	{
		logMessages.Add(logString);
	}

	void OnGUI()
	{
		// スクロールバーの太さを大きくするためのスタイル設定
		GUIStyle scrollBarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
		scrollBarStyle.fixedWidth = 40f; // ← ここで太さを調整！（デフォルトは15くらい）

		GUIStyle thumbStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb);
		thumbStyle.fixedWidth = 40f;

		GUI.skin.verticalScrollbar = scrollBarStyle;
		GUI.skin.verticalScrollbarThumb = thumbStyle;

		Rect bgRect = new Rect(0, 0, Screen.width, Screen.height / 2);
		GUI.Box(bgRect, GUIContent.none, backgroundStyle);

		// ドラッグでスクロール処理
		Event e = Event.current;
		if (e.type == EventType.MouseDown && bgRect.Contains(e.mousePosition))
		{
			isDragging = true;
			lastDragPos = e.mousePosition;
			e.Use();
		}
		else if (e.type == EventType.MouseDrag && isDragging)
		{
			Vector2 delta = e.mousePosition - lastDragPos;
			scrollPosition.y -= delta.y;
			lastDragPos = e.mousePosition;
			e.Use();
		}
		else if (e.type == EventType.MouseUp)
		{
			isDragging = false;
		}

		// 範囲制限
		scrollPosition.y = Mathf.Max(0, scrollPosition.y);

		// スクロールバー非表示で描画
		scrollPosition = GUILayout.BeginScrollView(
			scrollPosition,
			false, true, // 横スクロールバーは無し
			GUI.skin.horizontalScrollbar,
			GUI.skin.verticalScrollbar,
			GUILayout.Width(Screen.width),
			GUILayout.Height(Screen.height / 2)
		);

		foreach (string log in logMessages)
		{
			GUILayout.Label(log);
		}

		GUILayout.EndScrollView();
	}
}