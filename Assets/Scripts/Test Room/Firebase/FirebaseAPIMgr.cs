#if UNITY_IOS || UNITY_ANDROID
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using Google;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FirebaseAPIMgr : MonoBehaviour, ISocialAuthProvider
{
	public static FirebaseAPIMgr Instance { get; private set; }

#if UNITY_IOS || UNITY_ANDROID
	private FirebaseAuth auth;
	private FirebaseFirestore db;
#endif

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
		var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

		if (dependencyStatus != DependencyStatus.Available)
		{
			Debug.LogError("Firebaseの依存関係に問題があります: " + dependencyStatus);
			return;
		}

		auth = FirebaseAuth.DefaultInstance;
		db = FirebaseFirestore.DefaultInstance; // 🔄 ここで初期化

		// エディターの場合はGoogleSignIn.DefaultInstance.SignIn()内でcurrentActivityが参照できないエラーが発生するためGoogleログインは諦めて匿名ログインを行う
#if UNITY_EDITOR
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
			await CheckFirstLogin(user);

			OnUserIdReady?.Invoke();
		}
		catch (Exception ex)
		{
			Debug.LogError("Firebaseログイン中に例外発生: " + ex);
		}
#else
		// 初期化後、Googleログインを呼び出す(Firebaseも中でログイン)
		SignInWithGoogle();
#endif
#endif
	}

	public void SignInWithGoogle()
	{
#if UNITY_IOS || UNITY_ANDROID
		try
		{
			GoogleSignIn.Configuration = new GoogleSignInConfiguration
			{
				WebClientId = "868991135582-lsigt70n1vuat5l0rcmpkrngoeefc94k.apps.googleusercontent.com",
				RequestIdToken = true,  // IDトークンをリクエストする
				UseGameSignIn = false    // ← ここを false に
			};

			GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogError("Googleサインインがキャンセルされました");
					return;
				}

				if (task.IsFaulted)
				{
					Debug.LogError("Googleサインインに失敗しました: " + task.Exception);
					return;
				}

				GoogleSignInUser user = task.Result;

				if (user == null)
				{
					Debug.LogError("Googleサインインに失敗しました。ユーザーがnullです。");
					return;
				}

				if (string.IsNullOrEmpty(user.IdToken))
				{
					Debug.LogError("Googleサインインに失敗しました。ユーザーIDがnullまたは空です。");
					return;
				}

				string idToken = user.IdToken;
				Debug.Log("Googleログイン成功: " + user.DisplayName);
				Debug.Log("IdToken: " + idToken);

				// FirebaseにGoogleトークンでサインイン
				Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
				SignInToFirebase(credential);
			});
		}
		catch (Exception ex)
		{
			Debug.LogError("Googleサインイン中に例外が発生しました: " + ex.Message);
		}
#endif
	}

	private async void SignInToFirebase(Credential credential)
	{
#if UNITY_IOS || UNITY_ANDROID
		if (credential == null)
		{
			Debug.LogError($"Credentialがnullなので{nameof(SignInToFirebase)}を終了します。");
			return;
		}

		Debug.Log($"{nameof(SignInToFirebase)}起動");
		try
		{
			var result = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);

			if (result != null && result.UserId != null)
			{
				Debug.Log("Firebaseサインイン成功: " + result.UserId);
				UserId = result.UserId;
				IsReady = true;
				OnUserIdReady?.Invoke();
			}
			else
			{
				Debug.LogError("SignInToFirebase: resultまたはUserがnull");
			}
		}
		catch (Exception e)
		{
			Debug.LogError("サインインエラー: " + e.Message);
		}
#endif
	}

	public async UniTask CheckFirstLogin(FirebaseUser user)
	{
#if UNITY_IOS || UNITY_ANDROID
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
#endif
	}
}