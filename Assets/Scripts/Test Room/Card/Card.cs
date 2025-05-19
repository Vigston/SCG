using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
	public string Name { get; private set; }
	public Dictionary<string, int> Cost { get; private set; } // 必要な資源の種類と量

	public void Initialize(string name, Dictionary<string, int> cost)
	{
		Name = name;
		Cost = cost;
	}
}
