using System;
using System.Collections.Generic;
using UnityEngine;

public class CardLoader
{
	private List<CardData> cardDataList;

	// JSONファイルを読み込む
	public void LoadCardData()
	{
		AssetManager.Load<TextAsset>("CardDataTable", cardDataTable =>
		{
			// JSONデータをデシリアライズ
			CardDataList dataList = JsonUtility.FromJson<CardDataList>(cardDataTable.text);
			cardDataList = dataList.cards;

			// JSONのList<Cost>をDictionaryに変換
			foreach (var cardData in cardDataList)
			{
				cardData.cost = new Dictionary<ResourceType, int>();
				foreach (var cost in cardData.costList)
				{
					// KeyをResourceTypeに変換してDictionaryに追加
					if (Enum.TryParse<ResourceType>(cost.Key, out var type))
					{
						cardData.cost[type] = cost.Value;
					}
					else
					{
						Debug.LogError($"ResourceType変換失敗: {cost.Key}（カード名: {cardData.name}）");
					}
				}
			}

			// デバッグ出力
			//foreach (var card in cardDataList)
			//{
			//	string costStr = "";
			//	foreach (var c in card.costs)
			//	{
			//		costStr += $"{c.Key}:{c.Value} ";
			//	}
			//	Debug.Log($"カード名: {card.name}, コスト: {costStr}");
			//}
		},
		error =>
		{
			Debug.LogError($"カード情報の読み込み失敗: {error}");
		});
	}

	// カードデータを取得
	public List<CardData> GetCardDataList()
	{
		return cardDataList;
	}
}