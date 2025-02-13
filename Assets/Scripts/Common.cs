using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using System;

public static class Common
{
    public static class PrefabPath
    {
		public static readonly string Card = "Prefabs/Card";
		public static readonly string JoinPeopleGameAction = "Prefabs/JoinPeopleGameAction";
		public static readonly string StartPhase = "Prefabs/StartPhase";
		public static readonly string JoinPhase = "Prefabs/JoinPhase";
		public static readonly string MainPhase = "Prefabs/MainPhase";
		public static readonly string EndPhase = "Prefabs/EndPhase";
	}

	// バトルの共通定数
	public static class BattleConst
    {
        public const int ADD_GOLD_EVERY_MERCHANT = 10;
    }

    // BoxCollisionの頂点座標を取得
    /*
     * 0 = 左上(上)
     * 1 = 右上(上)
     * 2 = 右上(下)
     * 3 = 左上(下)
     * 4 = 右下(下)
     * 5 = 左下(下)
     * 6 = 左下(上)
     * 7 = 右下(上)
     */
    public static Vector3[] GetBoxCollideVertices(BoxCollider Col)
    {
        Transform trs = Col.transform;
        Vector3 sc = trs.lossyScale;

        sc.x *= Col.size.x;
        sc.y *= Col.size.y;
        sc.z *= Col.size.z;

        sc *= 0.5f;

        Vector3 cp = trs.TransformPoint(Col.center);

        Vector3 vx = trs.right * sc.x;
        Vector3 vy = trs.up * sc.y;
        Vector3 vz = trs.forward * sc.z;

        Vector3 p1 = -vx + vy + vz;
        Vector3 p2 = vx + vy + vz;
        Vector3 p3 = vx + -vy + vz;
        Vector3 p4 = -vx + -vy + vz;

        Vector3[] vertices = new Vector3[8];

        vertices[0] = cp + p1;
        vertices[1] = cp + p2;
        vertices[2] = cp + p3;
        vertices[3] = cp + p4;

        vertices[4] = cp - p1;
        vertices[5] = cp - p2;
        vertices[6] = cp - p3;
        vertices[7] = cp - p4;

        return vertices;
    }

    // スクリーン座標からワールド座標に変換
    public static Vector3 GetWorldPositionFromScreenPosition(Canvas _canvas, RectTransform _rect)
    {
        //UI座標からスクリーン座標に変換
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, _rect.position);

        //ワールド座標
        Vector3 worldPos = Vector3.zero;

        //スクリーン座標からワールド座標に変換
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_rect, screenPos, _canvas.worldCamera, out worldPos);

        return worldPos;
    }

    // 逆の側を取得
    public static Side GetRevSide(Side _Side)
    {
        // プレイヤー
        if(_Side == Side.eSide_Player)
        {
            return Side.eSide_Enemy;
        }
        // 敵
        else if(_Side == Side.eSide_Enemy)
        {
            return Side.eSide_Player;
        }

        return Side.eSide_None;
    }

    // 操作側が自分か
    public static bool IsMyOperateTurn()
    {
		Side operateSide = BattleUserMgr.instance.GetSetOperateSide;

        return operateSide == Side.eSide_Player;
	}

	// 自分のターンか
	public static bool IsMyTurn()
	{
        Side turnSide       = BattleMgr.instance.GetSetTurnSide;
        Side operateSide    = BattleUserMgr.instance.GetSetOperateSide;

		return turnSide == operateSide;
	}

    // 自分のカードか
    public static bool IsMyCard(BattleCard _battleCard)
    {
		if (_battleCard == null) { return false; }

		Side cardSide = _battleCard.GetSetSide;
		Side operateSide    = BattleUserMgr.instance.GetSetOperateSide;

		return cardSide == operateSide;
	}

	// 自分のターンで自分のカードか
	public static bool IsMyTurnAndMyCard(BattleCard _battleCard)
    {
        if(_battleCard == null) { return false; }

        return IsMyTurn() == IsMyCard(_battleCard);
	}

    // 自分のカードエリアか
    public static bool IsMyCardArea(CardArea _cardArea)
    {
        if (!_cardArea) return false;

        Side cardAreaSide = _cardArea.GetSide();
		Side operateSide = BattleUserMgr.instance.GetSetOperateSide;

        return cardAreaSide == operateSide;
	}
}
