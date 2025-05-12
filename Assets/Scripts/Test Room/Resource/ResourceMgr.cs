using System.Collections.Generic;
using UnityEngine;

// 資源管理クラス
public class ResourceMgr
{
	private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

	// 資源追加
	public void AddResource(string name, int quantity)
	{
		if (resources.ContainsKey(name))
		{
			resources[name].AddQuantity(quantity);
		}
		else
		{
			resources[name] = new Resource(name, quantity);
		}
	}

	// 資源を消費
	public bool ConsumeResource(string name, int quantity)
	{
		if (resources.ContainsKey(name) && resources[name].Consume(quantity))
		{
			return true;
		}
		Debug.LogWarning($"リソース {name} が不足しています。");
		return false;
	}

	// 資源の量を取得
	public int GetResourceQuantity(string name)
	{
		return resources.ContainsKey(name) ? resources[name].Quantity : 0;
	}

	// 全資源を取得（デバッグ用）
	public Dictionary<string, Resource> GetAllResources()
	{
		return resources;
	}
}