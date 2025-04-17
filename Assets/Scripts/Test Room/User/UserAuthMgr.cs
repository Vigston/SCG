using UnityEngine;
using System;
#if UNITY_IOS || UNITY_ANDROID
using Firebase;
using Firebase.Auth;
#endif
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using Steamworks;
#endif

public class UserAuthMgr : MonoBehaviour
{
	public static UserAuthMgr Instance { get; private set; }
	public string UserId { get; private set; } = "";

#if UNITY_IOS || UNITY_ANDROID
    private FirebaseAuth auth;
#endif

	public Action OnUserIdReady;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		Initialize();
	}

	private void Initialize()
	{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		InitSteam();
#elif UNITY_IOS || UNITY_ANDROID
        InitFirebase();
#else
        Debug.LogWarning("Unsupported platform for UserAuthManager");
#endif
	}

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	private void InitSteam()
	{
		if (SteamManager.Initialized)
		{
			UserId = SteamUser.GetSteamID().ToString();
			Debug.Log("Steam User ID: " + UserId);
			OnUserIdReady?.Invoke();
		}
		else
		{
			Debug.LogError("Steam not initialized!");
		}
	}
#endif

#if UNITY_IOS || UNITY_ANDROID
    private async void InitFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError("Could not resolve Firebase dependencies: " + dependencyStatus);
            return;
        }

        auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser == null)
        {
            await auth.SignInAnonymouslyAsync();
        }

        UserId = auth.CurrentUser.UserId;
        Debug.Log("Firebase User ID: " + UserId);
        OnUserIdReady?.Invoke();
    }
#endif
}