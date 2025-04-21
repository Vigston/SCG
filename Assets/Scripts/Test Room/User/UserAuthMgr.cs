using UnityEngine;
using System;
#if UNITY_IOS || UNITY_ANDROID
using Firebase;
using Firebase.Auth;
#endif
#if UNITY_STANDALONE_WIN
using Steamworks;
#endif

public class UserAuthManager : MonoBehaviour
{
	public static UserAuthManager Instance { get; private set; }
	public string UserId => provider?.UserId;
	public bool IsFirstLogin => provider?.IsFirstLogin　?? false;

	public event Action OnUserIdReady;

	private ISocialAuthProvider provider;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		// プラットフォームごとにプロバイダーを選択
#if UNITY_STANDALONE || UNITY_EDITOR
		provider = SteamAPIMgr.Instance;
#elif UNITY_IOS || UNITY_ANDROID
        provider = FirebaseAPIMgr.Instance;
#endif

		if (provider != null)
		{
			provider.OnUserIdReady += () =>
			{
				Debug.Log("UserAuthManager: UserId Ready: " + provider.UserId);
				OnUserIdReady?.Invoke(); // ★ここで他に伝播
			};

			provider.Initialize(); // ★ここで初期化スタート
		}
		else
		{
			Debug.LogError("プロバイダーが見つかりません！");
		}
	}
}