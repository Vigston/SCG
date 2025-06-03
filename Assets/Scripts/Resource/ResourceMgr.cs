using System.Collections.Generic;
using UnityEngine;

// 資源管理クラス
public class ResourceMgr
{
	private Dictionary<ResourceType, Resource> m_Resources = new Dictionary<ResourceType, Resource>();

	// 資源追加
	public void AddResource(ResourceType _type, int _quantity)
	{
		if (m_Resources.ContainsKey(_type))
		{
			m_Resources[_type].AddQuantity(_quantity);
		}
		else
		{
			m_Resources[_type] = new Resource(_type, _quantity);
		}
	}

	// 資源を消費
	public bool ConsumeResource(ResourceType _type, int _quantity)
	{
		if (m_Resources.ContainsKey(_type) && m_Resources[_type].Consume(_quantity))
		{
			return true;
		}
		Debug.LogWarning($"リソース {_type} が不足しています。");
		return false;
	}

	// 資源の量を取得
	public int GetResourceQuantity(ResourceType _type)
	{
		return m_Resources.ContainsKey(_type) ? m_Resources[_type].m_Quantity : 0;
	}

	// 全資源を取得（デバッグ用）
	public Dictionary<ResourceType, Resource> GetAllResources()
	{
		return m_Resources;
	}
}