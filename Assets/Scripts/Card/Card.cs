using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
	public string Name { get; private set; }
	public Dictionary<ResourceType, int> Cost { get; private set; } // 必要な資源の種類と量

	public void Initialize(CardData _cardData)
	{
		Name = _cardData.name;
		Cost = _cardData.cost;
	}
}
