using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardData
{
	public string name;

	public List<Cost> costList; // JSON用

	public Dictionary<ResourceType, int> cost;
}

[Serializable]
public class Cost
{
	public string Key;
	public int Value;
}

[Serializable]
public class CardDataList
{
	public List<CardData> cards;
}