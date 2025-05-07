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
using TMPro;
using WebSocketSharp;

public class FirebaseAPIMgr : MonoBehaviour, ISocialAuthProvider
{
	public static FirebaseAPIMgr Instance { get; private set; }

#if UNITY_IOS || UNITY_ANDROID
	private FirebaseAuth auth;
	private FirebaseFirestore db;
#endif

	[SerializeField] private string email;
	[SerializeField] private string password;

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
		//try
		//{
		//	var userCredential = await auth.SignInAnonymouslyAsync();
		//	var user = userCredential?.User;

		//	if (user == null)
		//	{
		//		Debug.LogError("Firebaseユーザーがnullです");
		//		return;
		//	}

		//	UserId = user.UserId;
		//	IsReady = true;

		//	Debug.Log("FirebaseユーザーID取得: " + UserId);

		//	// 初回ログインチェック
		//	await CheckFirstLogin(user);

		//	OnUserIdReady?.Invoke();
		//}
		//catch (Exception ex)
		//{
		//	Debug.LogError("Firebaseログイン中に例外発生: " + ex);
		//}
		SignInWithEmail();
#else
		// 初期化後、Googleログインを呼び出す(Firebaseも中でログイン)
		SignInWithGoogle();
#endif
#endif
	}

	public async void SignInWithEmail()
	{
		if(email.IsNullOrEmpty() || password.IsNullOrEmpty())
		{
			Debug.LogError("EmailまたはPasswordが未記入です");
			return;
		}

		try
		{
			var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
			FirebaseUser user = authResult.User;
			Debug.Log($"ログイン成功: {user.Email} (UID: {user.UserId})");

			UserId = user.UserId;
			IsReady = true;
			Debug.Log("FirebaseユーザーID取得: " + UserId);

			// 初回ログインチェック
			await CheckFirstLogin(user);

			OnUserIdReady?.Invoke();
		}
		catch (FirebaseException firebaseEx)
		{
			Debug.LogError($"Firebaseエラー: {firebaseEx.Message}");

			// エラーコードを知りたい場合
			var authError = (AuthError)firebaseEx.ErrorCode;
			Debug.LogError($"Authエラーコード: {authError}");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"その他のエラー: {ex.Message}");
		}
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
			var user = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);

			if (user != null && user.UserId != null)
			{
				Debug.Log("Firebaseサインイン成功: " + user.UserId);
				UserId = user.UserId;
				IsReady = true;

				// 初回ログインチェック
				await CheckFirstLogin(user);

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