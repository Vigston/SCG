using UnityEngine;
using Steamworks;

public class SteamAPIMgr : MonoBehaviour
{
    void Start()
    {
        if(SteamAPI.Init())
        {
			Debug.Log("SteamAPI Initialized");
		}
		else
		{
			Debug.LogError("SteamAPI initialization failed");
		}
	}

	private void OnApplicationQuit()
	{
		Debug.Log($"{this}：{nameof(OnApplicationQuit)}");
		SteamAPI.Shutdown();
	}
}
