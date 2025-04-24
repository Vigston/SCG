using UnityEngine;
using System.Collections.Generic;

public class DebugLogMgr : MonoBehaviour
{
	private static DebugLogMgr instance;
	private List<string> logMessages = new List<string>();
	private Vector2 scrollPosition = Vector2.zero;

	private GUIStyle backgroundStyle;
	private Texture2D backgroundTexture;

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
		// 背景の準備
		backgroundTexture = new Texture2D(1, 1);
		backgroundTexture.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 0.8f)); // 半透明の灰色
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

		// 背景描画
		Rect bgRect = new Rect(0, 0, Screen.width, Screen.height / 2);
		GUI.Box(bgRect, GUIContent.none, backgroundStyle);

		// スクロールビュー
		scrollPosition = GUILayout.BeginScrollView(
			scrollPosition,
			false, true, // 横スクロール無し、縦スクロールあり
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