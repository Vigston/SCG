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
		Instance = this;Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

		auth.SignInAnonymouslyAsync().ContinueWith(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
			{
				Debug.LogError("Firebaseログイン失敗");
				return;
			}

			var user = task.Result;
			UserId = user.UserId;
			IsReady = true;

			Debug.Log("FirebaseユーザーID取得: " + UserId);

			OnUserIdReady?.Invoke(); // ★イベント発火
		});
#endif
	}
}