// 資源クラス
[System.Serializable]
public class Resource
{
	public string Name { get; private set; }
	public int Quantity { get; private set; }

	public Resource(string name, int quantity)
	{
		Name = name;
		Quantity = quantity;
	}

	// 資源追加
	public void AddQuantity(int amount)
	{
		Quantity += amount;
	}

	// 資源消費
	public bool Consume(int amount)
	{
		// 所持量が足りているか確認
		if (Quantity >= amount)
		{
			Quantity -= amount;
			return true;
		}

		// 足りていない場合は消費できない
		return false;
	}
}