using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetManager
{
	/// <summary>
	/// Addressables でアセットを読み込む（非Mono、非GameObject依存）
	/// </summary>
	public static void Load<T>(string key, Action<T> onSuccess, Action<Exception> onFail = null) where T : UnityEngine.Object
	{
		Addressables.LoadAssetAsync<T>(key).Completed += handle =>
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				onSuccess?.Invoke(handle.Result);
			}
			else
			{
				Debug.LogError($"[AssetManager] 読み込み失敗: {key}\n{handle.OperationException}");
				onFail?.Invoke(handle.OperationException);
			}
		};
	}

	/// <summary>
	/// 読み込んだアセットを解放
	/// </summary>
	public static void Release<T>(T asset) where T : UnityEngine.Object
	{
		Addressables.Release(asset);
		Debug.Log($"[AssetManager] 解放: {asset.name}");
	}
}