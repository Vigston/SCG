#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using UnityEngine;
using System;
using Steamworks;

public class SteamAPIMgr : MonoBehaviour, ISocialAuthProvider
{
	public static SteamAPIMgr Instance { get; private set; }

	public string UserId { get; private set; }
	public bool IsFirstLogin { get; private set; }
	public bool IsReady { get; private set; }
	public event Action OnUserIdReady;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Initialize()
	{
		if (SteamManager.Initialized)
		{
			UserId = SteamUser.GetSteamID().ToString();

			int isFirstLogin;
			bool isFirstLoginFlag;

			if (!SteamUserStats.GetStat("FirstLogin", out isFirstLogin) || isFirstLogin == 0)
			{
				Debug.Log($"{this}：初回ログイン");
				isFirstLoginFlag = true;
				if(!SteamUserStats.SetStat("FirstLogin", 1))
				{
					Debug.LogError($"{this}：初回ログインのステータス設定に失敗");
				}
				else
				{
					Debug.Log($"{this}：初回ログインのステータス設定成功");
				}

				SteamUserStats.StoreStats(); // 忘れずに保存！
			}
			else
			{
				Debug.Log($"{this}：再ログイン");
				isFirstLoginFlag = false;
			}

			IsFirstLogin = isFirstLoginFlag;
			IsReady = true;

			Debug.Log("SteamユーザーID取得: " + UserId);

			OnUserIdReady?.Invoke(); // ★イベント発火
		}
		else
		{
			Debug.LogWarning("Steam未初期化");
		}
	}
}
#endif