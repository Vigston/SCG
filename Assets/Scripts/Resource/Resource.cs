// 資源クラス
[System.Serializable]
public class Resource
{
	public ResourceType m_Type { get; private set; }
	public int m_Quantity { get; private set; }

	public Resource(ResourceType _type, int _quantity)
	{
		m_Type = _type;
		m_Quantity = _quantity;
	}

	// 資源追加
	public void AddQuantity(int amount)
	{
		m_Quantity += amount;
	}

	// 資源消費
	public bool Consume(int amount)
	{
		// 所持量が足りているか確認
		if (m_Quantity >= amount)
		{
			m_Quantity -= amount;
			return true;
		}

		// 足りていない場合は消費できない
		return false;
	}
}