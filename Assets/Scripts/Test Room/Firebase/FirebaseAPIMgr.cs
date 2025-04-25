#if UNITY_IOS || UNITY_ANDROID
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FirebaseAPIMgr : MonoBehaviour, ISocialAuthProvider
{
	public static FirebaseAPIMgr Instance { get; private set; }

	private FirebaseAuth auth;
	private FirebaseFirestore db;

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
		//FirebaseApp.LogLevel = LogLevel.Debug;

		var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

		if (dependencyStatus != DependencyStatus.Available)
		{
			Debug.LogError("Firebaseの依存関係に問題があります: " + dependencyStatus);
			return;
		}

		auth = FirebaseAuth.DefaultInstance;
		db = FirebaseFirestore.DefaultInstance; // 🔄 ここで初期化

		try
		{
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

			// 初回ログインチェック
			await CheckFirstLogin(user); // 🔄 await 忘れずに！

			OnUserIdReady?.Invoke();
		}
		catch (Exception ex)
		{
			Debug.LogError("Firebaseログイン中に例外発生: " + ex);
		}
	}

	public async UniTask CheckFirstLogin(FirebaseUser user)
	{
		if (user == null)
		{
			Debug.LogError("Firebaseユーザーがnullです");
			return;
		}

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Debug.LogWarning("インターネット接続なし！");
			return;
		}

		var userDoc = db.Collection("users").Document(user.UserId);

		try
		{
			var snapshot = await userDoc.GetSnapshotAsync();

			if (!snapshot.Exists)
			{
				Debug.Log("初回ログインです！");
				Dictionary<string, object> userData = new Dictionary<string, object>
				{
					{ "isFirstLogin", true },
					{ "createdAt", Timestamp.GetCurrentTimestamp() }
				};
				await userDoc.SetAsync(userData);
				IsFirstLogin = true;
			}
			else
			{
				Debug.Log("通常ログインです");
				IsFirstLogin = false;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Firestoreアクセス失敗：" + ex);
			return;
		}
	}
}
#endif