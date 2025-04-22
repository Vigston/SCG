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

	public async void Initialize()
	{
#if UNITY_IOS || UNITY_ANDROID
		await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			if (task.IsFaulted || task.IsCanceled)
			{
				Debug.LogError("Firebase依存関係の解決に失敗しました: " + task.Exception?.Message);
			}
			else
			{
				Debug.Log("Firebaseの依存関係は正常です");

				// 初期化が成功した場合、Firebaseのデフォルトインスタンスを確認
				if (FirebaseApp.DefaultInstance == null)
				{
					Debug.LogError("Firebaseのインスタンスが初期化されていません");
				}
				else
				{
					Debug.Log("Firebaseが正常に初期化されました");
				}
			}
		});

		try
		{
			FirebaseAuth auth = FirebaseAuth.DefaultInstance;
			var userCredential = await auth.SignInAnonymouslyAsync();
			var user = userCredential?.User;

			if (user == null)
			{
				Debug.LogError("Firebaseユーザーがnullです");
				return;
			}

			UserId = user.UserId;
			IsReady = true;

			Debug.Log("FirebaseユーザーID取得: " + UserId);
			OnUserIdReady?.Invoke();
		}
		catch (Exception ex)
		{
			Debug.LogError("Firebaseログイン中に例外発生: " + ex);
		}
#endif
	}
}