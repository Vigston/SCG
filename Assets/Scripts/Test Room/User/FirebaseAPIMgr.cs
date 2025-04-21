#if UNITY_IOS || UNITY_ANDROID
using Firebase;
using Firebase.Auth;
#endif

using System;
using UnityEngine;

public class FirebaseAPIMgr : MonoBehaviour, ISocialAuthProvider
{
	public static FirebaseAPIMgr Instance { get; private set; }

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
#if UNITY_IOS || UNITY_ANDROID
		FirebaseAuth auth = FirebaseAuth.DefaultInstance;

		auth.SignInAnonymouslyAsync().ContinueWith(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
			{
				Debug.LogError("Firebaseログイン失敗");
				return;
			}

			// 修正ポイント：task.Result.User を通してUidにアクセス
			var user = task.Result.User;
			UserId = user.UserId ?? user.UserId; // 古いSDKでは UserId、新しいSDKでは Uid
			if (string.IsNullOrEmpty(UserId)) UserId = user.UserId; // 互換性のための予防策

			IsReady = true;

			Debug.Log("FirebaseユーザーID取得: " + UserId);

			OnUserIdReady?.Invoke(); // ★イベント発火
		});
#endif
	}
}